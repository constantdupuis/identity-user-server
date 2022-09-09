using AutoMapper;
using IdentityUserManagement.API.Contracts;
using IdentityUserManagement.API.Data;
using IdentityUserManagement.API.Models.IdentityUsers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System.Data;

namespace IdentityUserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityManagerController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IdentityDBContext _context;
        private readonly IIdentityUserManagerRepository _identityUsersManager;

        public IdentityManagerController(
            ILogger<IdentityManagerController> logger,
            IMapper mapper,
            IdentityDBContext context,
            IIdentityUserManagerRepository identityUsersManager)
        {
            _logger = logger;
            _mapper = mapper;
            _context = context;
            _identityUsersManager = identityUsersManager;
        }

        [HttpGet("users")]
        [Authorize(Roles = "Administrator")]
        public ActionResult<IEnumerable<UserDto>> GetUsers() {
            return Ok(_identityUsersManager.GetUsers());
        }

        [HttpPost("users/register")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> RegisterUser([FromBody] UserRegisterDto registerUserDto)
        {
            _logger.LogInformation($"Registration attempt for {registerUserDto.Email}");
            try
            {
                var errors = await _identityUsersManager.Register(registerUserDto);
                if (errors.Any())
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(RegisterUser)} - User Registration attempt for {registerUserDto.Email}");
                return Problem($"Something Went Wrong in the {nameof(RegisterUser)}. Please contact support.", statusCode: 500);
            }
        }

        [HttpPost("users/registerAdmin")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> RegisterAdmin([FromBody] UserRegisterDto registerUserDto)
        {
            _logger.LogInformation($"Administrator registration attempt for {registerUserDto.Email}");

            try
            {
                var errors = await _identityUsersManager.RegisterAdmin(registerUserDto);
                if (errors.Any())
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(RegisterAdmin)} - User Registration attempt for {registerUserDto.Email}");
                return Problem($"Something Went Wrong in the {nameof(RegisterAdmin)}. Please contact support.", statusCode: 500);
            }
        }

        // POST : api/account/login
        [HttpPost("users/login")]
        public async Task<ActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            _logger.LogInformation($"Login Attempt for {loginDto.Email}");
            try
            {
                var authResponse = await _identityUsersManager.Login(loginDto);
            if (authResponse == null)
            {
                return Unauthorized();
            }
            return Ok(authResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(Login)} - User Login attempt for {loginDto.Email}");
                return Problem($"Something Went Wrong in the {nameof(Login)}. Please contact support.", statusCode: 500);
            }
        }

        [HttpDelete("users/{userEmail}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> DeleteUser(string userEmail)
        {
            _logger.LogInformation($"Delete user attempt for {userEmail}");

            try
            {
                var errors = await _identityUsersManager.DeleteUserAsync(userEmail);
                if (errors.Any())
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(DeleteUser)} - User deletion attempt for {userEmail}");
                return Problem($"Something Went Wrong in the {nameof(DeleteUser)}. Please contact support.", statusCode: 500);
            }
        }

        [HttpPut("users/{email}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult> UpdateUser(string email, [FromBody] UserDto userDto)
        {
            if (email != userDto.Email)
            {
                return BadRequest($"Email doesn't match {email} != {userDto.Email}");
            }

            try
            {
                var errors = await _identityUsersManager.UpdateUserAsync(userDto);
                if (errors.Any())
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }
                return Ok();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(UpdateUser)} - User was probably change before our update, for role {userDto.Email}");
                return Problem($"Something Went Wrong in the {nameof(UpdateUser)}. Role was changed or deleted meanwhile.", statusCode: 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(UpdateUser)} - User update attempt for {userDto.Email}");
                return Problem($"Something Went Wrong in the {nameof(UpdateUser)}. Please contact support.", statusCode: 500);
            }
        }
        

        [HttpGet("roles")]
        [Authorize(Roles = "Administrator")]
        public ActionResult<IEnumerable<RoleDto>> GetRoles() {
            return Ok(_identityUsersManager.GetRoles());
        }

        [HttpPost("roles")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<RoleDto>> PostRole([FromBody] RoleDto createRoleDto)
        {
            try
            {
                var errors = await _identityUsersManager.AddRoleAsync(createRoleDto);
                if (errors.Any())
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(PostRole)} - Role Creation attempt for {createRoleDto.Name}");
                return Problem($"Something Went Wrong in the {nameof(PostRole)}. Please contact support.", statusCode: 500);
            }
        }

        [HttpDelete("roles/{roleName}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<RoleDto>> DeleteRole(string roleName)
        {
            try
            {
                var errors = await _identityUsersManager.DeleteRoleAsync(roleName);
                if (errors.Any())
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(DeleteRole)} - Role Deletion attempt for {roleName}");
                return Problem($"Something Went Wrong in the {nameof(DeleteRole)}. Please contact support.", statusCode: 500);
            }
        }

        [HttpPut("roles/{roleName}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<RoleUpdateDto>> UpdateRole(string roleName, [FromBody] RoleUpdateDto roleUpdate)
        {
            if (roleName != roleUpdate.CurrentRoleName)
            {
                return BadRequest($"RoleName doesn't match {roleName} != {roleUpdate.CurrentRoleName}");
            }

            try
            {
                var errors = await _identityUsersManager.UpdateRoleAsync(roleUpdate);
                if (errors.Any())
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }
                return Ok();
            }
            catch (DbUpdateConcurrencyException ex )
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(UpdateRole)} - Role was probably change before our update, for role {roleUpdate.CurrentRoleName}");
                return Problem($"Something Went Wrong in the {nameof(UpdateRole)}. Role was changed or deleted meanwhile.", statusCode: 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Something Went Wrong in the {nameof(UpdateRole)} - Role Update attempt for {roleUpdate.CurrentRoleName}");
                return Problem($"Something Went Wrong in the {nameof(UpdateRole)}. Please contact support.", statusCode: 500);
            }
        }
    }
}
