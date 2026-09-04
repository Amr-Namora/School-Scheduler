using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SchoolScheduler.Application.Timetable.Commands.GenerateTimetable;
using SchoolScheduler.Application.Timetable.Queries.GetClassTimetable;
using SchoolScheduler.Application.Common.Interfaces;
using System;
using System.Threading.Tasks;

namespace SchoolScheduler.Api.Controllers;

[Authorize(Roles = "School")]
[ApiController]
[Route("school/timetable")]
public class SchoolTimetableController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly ICurrentSchoolContext _schoolContext;

    public SchoolTimetableController(ISender mediator, ICurrentSchoolContext schoolContext)
    {
        _mediator = mediator;
        _schoolContext = schoolContext;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate()
    {
        var command = new GenerateTimetableCommand(_schoolContext.SchoolId.Value);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("class/{classRoomId}")]
    public async Task<IActionResult> GetClassTimetable(Guid classRoomId)
    {
        var query = new GetClassTimetableQuery(classRoomId, _schoolContext.SchoolId.Value);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
