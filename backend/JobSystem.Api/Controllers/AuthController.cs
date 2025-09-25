using JobSystem.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResult>> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (request.Password != request.ConfirmPassword)
                {
                    return BadRequest("Passwords do not match");
                }

                var result = await _authService.RegisterAsync(request);
                
                if (result.Success)
                {
                    return Ok(new { 
                        token = result.Token, 
                        user = new { 
                            result.User?.Id, 
                            result.User?.Email, 
                            result.User?.FirstName, 
                            result.User?.LastName 
                        },
                        message = result.Message 
                    });
                }

                return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return StatusCode(500, "An error occurred during registration");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResult>> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.LoginAsync(request);
                
                if (result.Success)
                {
                    return Ok(new { 
                        token = result.Token, 
                        user = new { 
                            result.User?.Id, 
                            result.User?.Email, 
                            result.User?.FirstName, 
                            result.User?.LastName 
                        },
                        message = result.Message 
                    });
                }

                return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, "An error occurred during login");
            }
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (request.NewPassword != request.ConfirmNewPassword)
                {
                    return BadRequest("New passwords do not match");
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var result = await _authService.ChangePasswordAsync(userId, request);
                
                if (result)
                {
                    return Ok(new { message = "Password changed successfully" });
                }

                return BadRequest("Failed to change password. Please check your current password.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return StatusCode(500, "An error occurred while changing password");
            }
        }
    }
}