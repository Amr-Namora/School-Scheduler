using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SchoolScheduler.Application.Schools.Commands.ConfigureWorkWeek;
using SchoolScheduler.Application.Schools.Commands.CreateSchool;
using SchoolScheduler.Application.Schools.Commands.SoftDeleteSchool;
using SchoolScheduler.Application.Schools.Queries.GetSchoolSetupStatus;
using SchoolScheduler.Application.Slots.Commands.CreateBreakSlot;
using SchoolScheduler.Application.Slots.Commands.CreateLectureSlot;
using SchoolScheduler.Application.Slots.Commands.DeleteBreakSlot;
using SchoolScheduler.Application.Slots.Commands.DeleteLectureSlot;
using SchoolScheduler.Application.Slots.Commands.UpdateBreakSlot;
using SchoolScheduler.Application.Slots.Commands.UpdateLectureSlot;
using SchoolScheduler.Application.Common.Interfaces;
using System;
using System.Threading.Tasks;

namespace SchoolScheduler.Api.Controllers;

[Authorize(Roles = "School")]
[ApiController]
[Route("school/setup")]
public class SchoolSetupController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly ICurrentSchoolContext _schoolContext;

    public SchoolSetupController(ISender mediator, ICurrentSchoolContext schoolContext)
    {
        _mediator = mediator;
        _schoolContext = schoolContext;
    }

    [HttpPost("work-week")]
    public async Task<IActionResult> ConfigureWorkWeek([FromBody] ConfigureWorkWeekCommand command)
    {
        var updatedCommand = command with { SchoolId = _schoolContext.SchoolId.Value };
        await _mediator.Send(updatedCommand);
        return Ok();
    }

    [HttpPost("lecture-slots")]
    public async Task<IActionResult> CreateLectureSlot([FromBody] CreateLectureSlotCommand command)
    {
        var updatedCommand = command with { SchoolId = _schoolContext.SchoolId.Value };
        var id = await _mediator.Send(updatedCommand);
        return Ok(id);
    }

    [HttpPut("lecture-slots/{id}")]
    public async Task<IActionResult> UpdateLectureSlot(Guid id, [FromBody] UpdateLectureSlotCommand command)
    {
        var updatedCommand = command with { SlotId = id };
        await _mediator.Send(updatedCommand);
        return Ok();
    }

    [HttpDelete("lecture-slots/{id}")]
    public async Task<IActionResult> DeleteLectureSlot(Guid id)
    {
        var command = new DeleteLectureSlotCommand(id);
        await _mediator.Send(command);
        return Ok();
    }

    [HttpPost("break-slots")]
    public async Task<IActionResult> CreateBreakSlot([FromBody] CreateBreakSlotCommand command)
    {
        var updatedCommand = command with { SchoolId = _schoolContext.SchoolId.Value };
        var id = await _mediator.Send(updatedCommand);
        return Ok(id);
    }

    [HttpPut("break-slots/{id}")]
    public async Task<IActionResult> UpdateBreakSlot(Guid id, [FromBody] UpdateBreakSlotCommand command)
    {
        var updatedCommand = command with { SlotId = id };
        await _mediator.Send(updatedCommand);
        return Ok();
    }

    [HttpDelete("break-slots/{id}")]
    public async Task<IActionResult> DeleteBreakSlot(Guid id)
    {
        var command = new DeleteBreakSlotCommand(id);
        await _mediator.Send(command);
        return Ok();
    }

    [HttpGet("status")]
    public async Task<IActionResult> GetSetupStatus()
    {
        var query = new GetSchoolSetupStatusQuery(_schoolContext.SchoolId.Value);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> SoftDeleteSchool()
    {
        var command = new SoftDeleteSchoolCommand(_schoolContext.SchoolId.Value);
        await _mediator.Send(command);
        return Ok();
    }
}
