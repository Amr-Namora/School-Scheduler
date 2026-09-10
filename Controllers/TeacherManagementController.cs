using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SchoolScheduler.Application.Teachers.Commands.ClearTeacherAvailability;
using SchoolScheduler.Application.Teachers.Commands.CreateTeacher;
using SchoolScheduler.Application.Teachers.Commands.DeleteTeacher;
using SchoolScheduler.Application.Teachers.Commands.DeleteTeacherAvailabilitySlot;
using SchoolScheduler.Application.Teachers.Commands.SetTeacherAvailability;
using SchoolScheduler.Application.Teachers.Commands.UnassignSubjectFromTeacher;
using SchoolScheduler.Application.Teachers.Commands.UpdateTeacher;
using SchoolScheduler.Application.Teachers.Commands.AssignSubjectsToTeacher;
using SchoolScheduler.Application.Teachers.Queries.GetAllTeachers;
using SchoolScheduler.Application.Common.Interfaces;
using System;
using System.Threading.Tasks;

namespace SchoolScheduler.Api.Controllers;

[Authorize(Roles = "School")]
[ApiController]
[Route("school/teachers")]
public class TeacherManagementController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly ICurrentSchoolContext _schoolContext;

    public TeacherManagementController(ISender mediator, ICurrentSchoolContext schoolContext)
    {
        _mediator = mediator;
        _schoolContext = schoolContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTeachers()
    {
        var query = new GetAllTeachersQuery(_schoolContext.SchoolId.Value);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTeacher([FromBody] CreateTeacherCommand command)
    {
        var updatedCommand = command with { SchoolId = _schoolContext.SchoolId.Value };
        var id = await _mediator.Send(updatedCommand);
        return Ok(id);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTeacher(Guid id, [FromBody] UpdateTeacherCommand command)
    {
        var updatedCommand = command with { TeacherId = id };
        await _mediator.Send(updatedCommand);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTeacher(Guid id)
    {
        var command = new DeleteTeacherCommand(id);
        await _mediator.Send(command);
        return Ok();
    }

    [HttpPost("{id}/subjects")]
    public async Task<IActionResult> AssignSubjects([FromBody] AssignSubjectsToTeacherCommand command, Guid id)
    {
        var updatedCommand = command with { TeacherId = id };
        await _mediator.Send(updatedCommand);
        return Ok();
    }

    [HttpDelete("{id}/subjects/{subjectId}")]
    public async Task<IActionResult> UnassignSubject(Guid id, Guid subjectId)
    {
        var command = new UnassignSubjectFromTeacherCommand(id, subjectId);
        await _mediator.Send(command);
        return Ok();
    }

    [HttpPost("{id}/availability")]
    public async Task<IActionResult> SetAvailability([FromBody] SetTeacherAvailabilityCommand command, Guid id)
    {
        var updatedCommand = command with { TeacherId = id };
        await _mediator.Send(updatedCommand);
        return Ok();
    }

    [HttpDelete("{id}/availability/{dayOfWeek}/{slotNumber}")]
    public async Task<IActionResult> DeleteAvailabilitySlot(Guid id, SchoolScheduler.Domain.Enums.SchoolDayOfWeek dayOfWeek, int slotNumber)
    {
        var command = new DeleteTeacherAvailabilitySlotCommand(id, dayOfWeek, slotNumber);
        await _mediator.Send(command);
        return Ok();
    }

    [HttpDelete("{id}/availability/clear")]
    public async Task<IActionResult> ClearAvailability(Guid id)
    {
        var command = new ClearTeacherAvailabilityCommand(id);
        await _mediator.Send(command);
        return Ok();
    }
}
