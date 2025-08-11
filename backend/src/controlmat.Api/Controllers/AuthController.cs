using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using FluentValidation;
using Controlmat.Application.Common.Commands.Auth;
using Controlmat.Application.Common.Queries.Auth;
using Controlmat.Application.Common.Dto;
using System.Security.Claims;

namespace Controlmat.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IMediator mediator, ILogger<AuthController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto credentials)
        {
            try
            {
                var result = await _mediator.Send(new AuthenticateUserCommand.Request
                {
                    Credentials = credentials
                });
                _logger.LogInformation("User {UserName} logged in successfully", credentials.UserName);
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Failed login attempt for user {UserName}", credentials.UserName);
                return Unauthorized(new { message = "Invalid credentials" });
            }
            catch (ValidationException ex)
            {
                return BadRequest(new { message = "Validation failed", errors = ex.Errors.Select(e => e.ErrorMessage) });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error for user {UserName}", credentials.UserName);
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                var result = await _mediator.Send(new GetCurrentUserQuery.Request { UserId = userId });
                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "User not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            _logger.LogInformation("User {UserName} logged out", userName);
            return Ok(new { message = "Logged out successfully" });
        }

        [HttpGet("validate")]
        [Authorize]
        public IActionResult ValidateToken()
        {
            var userId = User.FindFirst("userId")?.Value;
            var userName = User.FindFirst("userName")?.Value;
            var role = User.FindFirst("role")?.Value;

            return Ok(new { isValid = true, userId, userName, role });
        }
    }
}
