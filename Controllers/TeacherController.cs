using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SchoolScheduler.Application.Teachers.Queries.GetAvailabilityForTeacher;
using SchoolScheduler.Application.Teachers.Queries.GetSubjectsForTeacher;
using SchoolScheduler.Application.Teachers.Queries.GetMyAvailability;
using SchoolScheduler.Application.Teachers.Queries.GetMySubjects;
using SchoolScheduler.Application.Timetable.Queries.GetMyTimetable;
using SchoolScheduler.Application.Common.Interfaces;
using System;
using System.Threading.Tasks;

namespace SchoolScheduler.Api.Controllers;

[ApiController]
[Route("teacher")]
public class TeacherController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly ICurrentUserContext _userContext;
    private readonly ICurrentSchoolContext _schoolContext;

    public TeacherController(ISender mediator, ICurrentUserContext userContext, ICurrentSchoolContext schoolContext)
    {
        _mediator = mediator;
        _userContext = userContext;
        _schoolContext = schoolContext;
    }

    // --- School-side views ---
    [HttpGet("/school/teachers/{teacherId}/availability")]
    [Authorize(Roles = "School")]
    public async Task<IActionResult> GetTeacherAvailability(Guid teacherId)
    {
        var query = new GetAvailabilityForTeacherQuery(_schoolContext.SchoolId.Value, teacherId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("/school/teachers/{teacherId}/subjects")]
    [Authorize(Roles = "School")]
    public async Task<IActionResult> GetTeacherSubjects(Guid teacherId)
    {
        var teacher = await _mediator.Send(new SchoolScheduler.Application.Teachers.Queries.GetTeacherById.GetTeacherByIdQuery(teacherId));
        if (teacher == null || teacher.SchoolId != _schoolContext.SchoolId.Value)
        {
            return NotFound("Teacher not found in this school.");
        }

        var query = new GetSubjectsForTeacherQuery(teacherId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    // --- Teacher's own views ---
    [HttpGet("me/availability")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetMyAvailability()
    {
        var query = new GetMyAvailabilityQuery(_userContext.UserId!);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("me/subjects")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetMySubjects()
    {
        var query = new GetMySubjectsQuery(_userContext.UserId!);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("me/timetable")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> GetMyTimetable()
    {
        var query = new GetMyTimetableQuery(_userContext.UserId!);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
