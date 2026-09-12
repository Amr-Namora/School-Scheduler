using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SchoolScheduler.Application.Assignments.Commands.CreateClassSubjectAssignment;
using SchoolScheduler.Application.Assignments.Commands.BulkCreateClassSubjectAssignment;
using SchoolScheduler.Application.Assignments.Commands.DeleteClassSubjectAssignment;
using SchoolScheduler.Application.Assignments.Commands.UpdateClassSubjectAssignment;
using SchoolScheduler.Application.Assignments.Queries.GetAllAssignmentsForClassRoom;
using SchoolScheduler.Application.Common.Interfaces;
using System;
using System.Threading.Tasks;

namespace SchoolScheduler.Api.Controllers;

[Authorize(Roles = "School")]
[ApiController]
[Route("school/assignments")]
public class ClassAssignmentController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly ICurrentSchoolContext _schoolContext;

    public ClassAssignmentController(ISender mediator, ICurrentSchoolContext schoolContext)
    {
        _mediator = mediator;
        _schoolContext = schoolContext;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAssignment([FromBody] CreateClassSubjectAssignmentCommand command)
    {
        // Command doesn't take SchoolId, handler resolves it from ClassRoom
        var id = await _mediator.Send(command);
        return Ok(id);
    }

    [HttpPost("bulk")]
    public async Task<IActionResult> CreateBulkAssignments([FromBody] BulkCreateClassSubjectAssignmentCommand command)
    {
        var count = await _mediator.Send(command);
        return Ok(new { Count = count });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAssignment(Guid id, [FromBody] UpdateClassSubjectAssignmentCommand command)
    {
        var updatedCommand = command with { Id = id };
        await _mediator.Send(updatedCommand);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAssignment(Guid id)
    {
        var command = new DeleteClassSubjectAssignmentCommand(id);
        await _mediator.Send(command);
        return Ok();
    }

    [HttpGet("class-room/{classRoomId}")]
    public async Task<IActionResult> GetAssignmentsForClassRoom(Guid classRoomId)
    {
        var query = new GetAllAssignmentsForClassRoomQuery(classRoomId, _schoolContext.SchoolId.Value);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
