using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SchoolScheduler.Application.Grades.Commands.CreateGrade;
using SchoolScheduler.Application.Grades.Commands.DeleteGrade;
using SchoolScheduler.Application.Grades.Commands.UpdateGrade;
using SchoolScheduler.Application.Grades.Queries.GetAllGrades;
using SchoolScheduler.Application.Grades.Queries.GetGradeById;
using SchoolScheduler.Application.ClassRooms.Commands.CreateClassRoom;
using SchoolScheduler.Application.ClassRooms.Commands.DeleteClassRoom;
using SchoolScheduler.Application.ClassRooms.Commands.UpdateClassRoom;
using SchoolScheduler.Application.ClassRooms.Queries.GetAllClassRooms;
using SchoolScheduler.Application.ClassRooms.Queries.GetClassRoomById;
using SchoolScheduler.Application.Subjects.Commands.CreateSubject;
using SchoolScheduler.Application.Subjects.Commands.DeleteSubject;
using SchoolScheduler.Application.Subjects.Commands.UpdateSubject;
using SchoolScheduler.Application.Subjects.Queries.GetSubjectsWithTeachers;
using SchoolScheduler.Application.Subjects.Queries.GetAllSubjects;
using SchoolScheduler.Application.Subjects.Queries.GetSubjectById;
using SchoolScheduler.Application.Common.Interfaces;
using System;
using System.Threading.Tasks;

namespace SchoolScheduler.Api.Controllers;

[Authorize(Roles = "School")]
[ApiController]
[Route("school/management")]
public class SchoolManagementController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly ICurrentSchoolContext _schoolContext;

    public SchoolManagementController(ISender mediator, ICurrentSchoolContext schoolContext)
    {
        _mediator = mediator;
        _schoolContext = schoolContext;
    }

    // --- Grades ---
    [HttpPost("grades")]
    public async Task<IActionResult> CreateGrade([FromBody] CreateGradeCommand command)
    {
        var updatedCommand = command with { SchoolId = _schoolContext.SchoolId.Value };
        var id = await _mediator.Send(updatedCommand);
        return Ok(id);
    }

    [HttpPut("grades/{id}")]
    public async Task<IActionResult> UpdateGrade(Guid id, [FromBody] UpdateGradeCommand command)
    {
        var updatedCommand = command with { GradeId = id };
        await _mediator.Send(updatedCommand);
        return Ok();
    }

    [HttpDelete("grades/{id}")]
    public async Task<IActionResult> DeleteGrade(Guid id)
    {
        var command = new DeleteGradeCommand(id);
        await _mediator.Send(command);
        return Ok();
    }

    [HttpGet("grades")]
    public async Task<IActionResult> GetAllGrades()
    {
        var query = new GetAllGradesQuery(_schoolContext.SchoolId.Value);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("grades/{id}")]
    public async Task<IActionResult> GetGrade(Guid id)
    {
        var query = new GetGradeByIdQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // --- Class Rooms ---
    [HttpPost("class-rooms")]
    public async Task<IActionResult> CreateClassRoom([FromBody] CreateClassRoomCommand command)
    {
        var updatedCommand = command with { SchoolId = _schoolContext.SchoolId.Value };
        var id = await _mediator.Send(updatedCommand);
        return Ok(id);
    }

    [HttpPut("class-rooms/{id}")]
    public async Task<IActionResult> UpdateClassRoom(Guid id, [FromBody] UpdateClassRoomCommand command)
    {
        var updatedCommand = command with { ClassRoomId = id };
        await _mediator.Send(updatedCommand);
        return Ok();
    }

    [HttpDelete("class-rooms/{id}")]
    public async Task<IActionResult> DeleteClassRoom(Guid id)
    {
        var command = new DeleteClassRoomCommand(id);
        await _mediator.Send(command);
        return Ok();
    }

    [HttpGet("class-rooms")]
    public async Task<IActionResult> GetAllClassRooms([FromQuery] Guid? gradeId)
    {
        var query = new GetAllClassRoomsQuery(_schoolContext.SchoolId.Value, gradeId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("class-rooms/{id}")]
    public async Task<IActionResult> GetClassRoom(Guid id)
    {
        var query = new GetClassRoomByIdQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // --- Subjects ---
    [HttpPost("subjects")]
    public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectCommand command)
    {
        var updatedCommand = command with { SchoolId = _schoolContext.SchoolId.Value };
        var id = await _mediator.Send(updatedCommand);
        return Ok(id);
    }

    [HttpPut("subjects/{id}")]
    public async Task<IActionResult> UpdateSubject(Guid id, [FromBody] UpdateSubjectCommand command)
    {
        var updatedCommand = command with { SubjectId = id };
        await _mediator.Send(updatedCommand);
        return Ok();
    }

    [HttpDelete("subjects/{id}")]
    public async Task<IActionResult> DeleteSubject(Guid id)
    {
        var command = new DeleteSubjectCommand(id);
        await _mediator.Send(command);
        return Ok();
    }

    [HttpGet("/school/subjects/teachers")]
    public async Task<IActionResult> GetSubjectsWithTeachers()
    {
        var query = new GetSubjectsWithTeachersQuery(_schoolContext.SchoolId.Value);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("subjects")]
    public async Task<IActionResult> GetAllSubjects()
    {
        var query = new GetAllSubjectsQuery(_schoolContext.SchoolId.Value);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("subjects/{id}")]
    public async Task<IActionResult> GetSubject(Guid id)
    {
        var query = new GetSubjectByIdQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
