using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SchoolScheduler.Application.LinkRequests.Commands.AcceptTeacherLinkRequest;
using SchoolScheduler.Application.LinkRequests.Commands.DeclineTeacherLinkRequest;
using SchoolScheduler.Application.LinkRequests.Queries.GetPendingLinkRequestsForUser;
using SchoolScheduler.Application.Common.Interfaces;
using System;
using System.Threading.Tasks;

namespace SchoolScheduler.Api.Controllers;

[Authorize(Roles = "Teacher")]
[ApiController]
[Route("teacher/link-requests")]
public class LinkRequestController : ControllerBase
{
    private readonly ISender _mediator;
    private readonly ICurrentUserContext _userContext;

    public LinkRequestController(ISender mediator, ICurrentUserContext userContext)
    {
        _mediator = mediator;
        _userContext = userContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetPendingRequests()
    {
        var query = new GetPendingLinkRequestsForUserQuery(_userContext.UserId!);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost("{id}/accept")]
    public async Task<IActionResult> AcceptRequest(Guid id)
    {
        var command = new AcceptTeacherLinkRequestCommand(id, _userContext.UserId!);
        await _mediator.Send(command);
        return Ok();
    }

    [HttpPost("{id}/decline")]
    public async Task<IActionResult> DeclineRequest(Guid id)
    {
        var command = new DeclineTeacherLinkRequestCommand(id, _userContext.UserId!);
        await _mediator.Send(command);
        return Ok();
    }
}
