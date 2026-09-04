using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using SchoolScheduler.Infrastructure.Identity;
using SchoolScheduler.Application.Schools.Commands.CreateSchool;
using SchoolScheduler.Application.LinkRequests.Commands.CreateLinkRequestsForNewUser;
using System.Net;

namespace SchoolScheduler.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ISender _mediator;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IJwtTokenService jwtTokenService,
        ISender mediator)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtTokenService = jwtTokenService;
        _mediator = mediator;
    }

    [HttpPost("school/signup")]
    public async Task<IActionResult> SchoolSignup([FromBody] SchoolSignupRequest request)
    {
        var user = new ApplicationUser { UserName = request.Email, Email = request.Email };
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.Select(e => e.Description));
        }
        Console.WriteLine("1. User created");

        await _userManager.AddToRoleAsync(user, "School");
        Console.WriteLine("2. Role assigned");

        var schoolId = await _mediator.Send(new CreateSchoolCommand(request.SchoolName, user.Id, request.LecturesPerDay));
        Console.WriteLine("3. MediatR finished");
        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtTokenService.GenerateToken(user, roles);

        return Ok(new { Token = token, SchoolId = schoolId });
    }

    [HttpPost("teacher/signup")]
    public async Task<IActionResult> TeacherSignup([FromBody] TeacherSignupRequest request)
    {
        var user = new ApplicationUser { UserName = request.Email, Email = request.Email, FullName = request.FullName };
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors.Select(e => e.Description));
        }

        await _userManager.AddToRoleAsync(user, "Teacher");

        await _mediator.Send(new CreateLinkRequestsForNewUserCommand(user.Id, request.Email));

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtTokenService.GenerateToken(user, roles);

        return Ok(new { Token = token });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Unauthorized("Invalid email or password.");
        }

        var result = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!result)
        {
            return Unauthorized("Invalid email or password.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtTokenService.GenerateToken(user, roles);

        return Ok(new { Token = token });
    }
}

public record SchoolSignupRequest(string Email, string Password, string SchoolName, int LecturesPerDay);
public record TeacherSignupRequest(string Email, string Password, string FullName);
public record LoginRequest(string Email, string Password);
