using System;
using System.Collections.Generic;
using System.Linq;
using Google.OrTools.Sat;
using SchoolScheduler.Domain.Entities;
using SchoolScheduler.Infrastructure.Persistence.Solver;
using SchoolScheduler.Application.Common;

namespace SchoolScheduler.Infrastructure.Persistence.Solver;

public class CpSatTimetableSolver
{
    public SolverOutput Solve(SolverInput input)
    {
        var model = new CpModel();

        // Variables: V[classroom, day, slot] = assignmentIndex (0 = empty)
        var vars = new Dictionary<(int c, int d, int s), IntVar>();

        var classrooms = input.ClassRooms;
        var days = input.WorkingDays.OrderBy(d => (int)d.DayOfWeek).ToList();
        var slots = input.LectureSlots.OrderBy(s => s.SlotNumber).ToList();

        for (int c = 0; c < classrooms.Count; c++)
        {
            for (int d = 0; d < days.Count; d++)
            {
                for (int s = 0; s < slots.Count; s++)
                {
                    var assignmentsForClass = input.Assignments
                        .Where(a => a.ClassRoomId == classrooms[c].Id)
                        .ToList();

                    // Domain: 0 to number of assignments for this classroom
                    var v = model.NewIntVar(0, assignmentsForClass.Count, $"v_{c}_{d}_{s}");
                    vars[(c, d, s)] = v;
                }
            }
        }

        // Mapping: (classroom, assignmentIndex) -> (SubjectId, TeacherId)
        var classroomAssignments = classrooms.Select(c =>
            input.Assignments.Where(a => a.ClassRoomId == c.Id).ToList()
        ).ToList();

        // 1. Teacher Double-Booking: For each (day, slot), a teacher can be in at most one classroom
        for (int d = 0; d < days.Count; d++)
        {
            for (int s = 0; s < slots.Count; s++)
            {
                var teachers = input.Teachers;
                foreach (var teacher in teachers)
                {
                    var teacherSlots = new List<LinearExpr>();
                    for (int c = 0; c < classrooms.Count; c++)
                    {
                        var v = vars[(c, d, s)];
                        // Find indices of assignments for this classroom that are taught by this teacher
                        var indices = classroomAssignments[c]
                            .Select((a, idx) => a.TeacherId == teacher.Id ? idx + 1 : -1)
                            .Where(idx => idx != -1)
                            .ToList();

                        foreach (var idx in indices)
                        {
                            // Create boolean variable: V_{c,d,s} == idx
                            var isSelected = model.NewBoolVar($"is_{c}_{d}_{s}_{idx}");
                            model.Add(v == idx).OnlyEnforceIf(isSelected);
                            model.Add(v != idx).OnlyEnforceIf(isSelected.Not());
                            teacherSlots.Add(isSelected);
                        }
                    }
                    if (teacherSlots.Count > 0)
                    {
                        model.Add(LinearExpr.Sum(teacherSlots) <= 1);
                    }
                }
            }
        }

        // 2. Teacher Availability
        for (int d = 0; d < days.Count; d++)
        {
            for (int s = 0; s < slots.Count; s++)
            {
                var dayOfWeek = days[d].DayOfWeek;
                var slotNumber = slots[s].SlotNumber;

                for (int c = 0; c < classrooms.Count; c++)
                {
                    var v = vars[(c, d, s)];
                    var assignments = classroomAssignments[c];

                    for (int i = 0; i < assignments.Count; i++)
                    {
                        var assignment = assignments[i];
                        var teacher = input.Teachers.First(t => t.Id == assignment.TeacherId);

                        var isAvailable = input.Availabilities.Any(a =>
                            a.TeacherId == teacher.Id &&
                            a.DayOfWeek == dayOfWeek &&
                            a.SlotNumber == slotNumber);

                        if (!isAvailable)
                        {
                            // V_{c,d,s} cannot be i + 1
                            model.Add(v != i + 1);
                        }
                    }
                }
            }
        }

        // 3. Subject Quota: Sum of occurrences per class per week == WeeklyQuota
        for (int c = 0; c < classrooms.Count; c++)
        {
            var assignments = classroomAssignments[c];
            foreach (var assignment in assignments)
            {
                var subjectSlots = new List<LinearExpr>();
                for (int d = 0; d < days.Count; d++)
                {
                    for (int s = 0; s < slots.Count; s++)
                    {
                        var v = vars[(c, d, s)];
                        // Find index of this assignment
                        var idx = assignments.FindIndex(a => a.Id == assignment.Id) + 1;
                        var isSelected = model.NewBoolVar($"quota_{c}_{d}_{s}_{idx}");
                        model.Add(v == idx).OnlyEnforceIf(isSelected);
                        model.Add(v != idx).OnlyEnforceIf(isSelected.Not());
                        subjectSlots.Add(isSelected);
                    }
                }
                model.Add(LinearExpr.Sum(subjectSlots) == assignment.WeeklyQuota);
            }
        }

        // 4. Subject Daily Limit: Sum of occurrences per class per day <= 2
        for (int c = 0; c < classrooms.Count; c++)
        {
            var assignments = classroomAssignments[c];
            for (int d = 0; d < days.Count; d++)
            {
                foreach (var assignment in assignments)
                {
                    var dailySlots = new List<LinearExpr>();
                    for (int s = 0; s < slots.Count; s++)
                    {
                        var v = vars[(c, d, s)];
                        var idx = assignments.FindIndex(a => a.Id == assignment.Id) + 1;
                        var isSelected = model.NewBoolVar($"daily_{c}_{d}_{s}_{idx}");
                        model.Add(v == idx).OnlyEnforceIf(isSelected);
                        model.Add(v != idx).OnlyEnforceIf(isSelected.Not());
                        dailySlots.Add(isSelected);
                    }
                    model.Add(LinearExpr.Sum(dailySlots) <= 2);
                }
            }
        }

        // 5. Fatigue Limit: No 3+ consecutive slots for a teacher
        for (int d = 0; d < days.Count; d++)
        {
            for (int s = 0; s <= slots.Count - 3; s++)
            {
                foreach (var teacher in input.Teachers)
                {
                    var consecutiveSlots = new List<LinearExpr>();
                    for (int slotOffset = 0; slotOffset < 3; slotOffset++)
                    {
                        var currentSlot = s + slotOffset;
                        var teacherInSlot = new List<LinearExpr>();
                        for (int c = 0; c < classrooms.Count; c++)
                        {
                            var v = vars[(c, d, currentSlot)];
                            var indices = classroomAssignments[c]
                                .Select((a, idx) => a.TeacherId == teacher.Id ? idx + 1 : -1)
                                .Where(idx => idx != -1)
                                .ToList();

                            foreach (var idx in indices)
                            {
                                var isSelected = model.NewBoolVar($"fatigue_{c}_{d}_{currentSlot}_{idx}");
                                model.Add(v == idx).OnlyEnforceIf(isSelected);
                                model.Add(v != idx).OnlyEnforceIf(isSelected.Not());
                                teacherInSlot.Add(isSelected);
                            }
                        }
                        // Teacher is in some classroom in this slot
                        var teacherPresence = model.NewBoolVar($"presence_{d}_{currentSlot}_{teacher.Id}");
                        model.Add(LinearExpr.Sum(teacherInSlot) == 1).OnlyEnforceIf(teacherPresence);
                        model.Add(LinearExpr.Sum(teacherInSlot) == 0).OnlyEnforceIf(teacherPresence.Not());
                        consecutiveSlots.Add(teacherPresence);
                    }
                    model.Add(LinearExpr.Sum(consecutiveSlots) <= 2);
                }
            }
        }

        var solver = new CpSolver();
        var status = solver.Solve(model);

        if (status == CpSolverStatus.Optimal || status == CpSolverStatus.Feasible)
        {
            var entries = new List<TimetableEntry>();
            for (int c = 0; c < classrooms.Count; c++)
            {
                for (int d = 0; d < days.Count; d++)
                {
                    for (int s = 0; s < slots.Count; s++)
                    {
                        var val = (int)solver.Value(vars[(c, d, s)]);
                        if (val > 0)
                        {
                            var assignment = classroomAssignments[c][val - 1];
                            entries.Add(TimetableEntry.CreateScheduled(
                                Guid.NewGuid(),
                                classrooms[c].SchoolId,
                                classrooms[c].Id,
                                assignment.SubjectId,
                                assignment.TeacherId,
                                days[d].DayOfWeek,
                                slots[s].SlotNumber
                            ));
                        }
                        else
                        {
                            entries.Add(TimetableEntry.CreateEmpty(
                                Guid.NewGuid(),
                                classrooms[c].SchoolId,
                                classrooms[c].Id,
                                days[d].DayOfWeek,
                                slots[s].SlotNumber
                            ));
                        }
                    }
                }
            }
            return new SolverOutput { Success = true, Entries = entries };
        }

        // Infeasibility Analysis (Simplified)
        // In a real scenario, we'd use a more sophisticated approach, but for now:
        return new SolverOutput { Success = false, FailureCategory = "General Constraints" };
    }
}
