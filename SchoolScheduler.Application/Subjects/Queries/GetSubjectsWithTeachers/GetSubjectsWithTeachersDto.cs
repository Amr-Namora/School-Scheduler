using System;
using System.Collections.Generic;

namespace SchoolScheduler.Application.Subjects.Queries.GetSubjectsWithTeachers;

public record TeacherShortDto(Guid Id, string DisplayName);

public record SubjectTeachersDto(Guid SubjectId, string SubjectName, List<TeacherShortDto> Teachers);
