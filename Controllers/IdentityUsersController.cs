using AutoMapper;
using IdentityUserManagement.API.Contracts;
using IdentityUserManagement.API.Data;
using IdentityUserManagement.API.Models.IdentityUsers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;

namespace IdentityUserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityUsersController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IdentityDBContext _context;
        private readonly IIdentityUserManagerRepository _identityUsersManager;

        public IdentityUsersController( 
            ILogger<IdentityUsersController> logger, 
            IMapper mapper, 
            IdentityDBContext context, 
            IIdentityUserManagerRepository identityUsersManager )
        {
            _logger = logger;
            _mapper = mapper;
            _context = context;
            _identityUsersManager = identityUsersManager;
        }

        [HttpGet("users")]
        public ActionResult<IEnumerable<UserDto>> GetUsers() {
            return Ok(_identityUsersManager.GetUsers());
        }

        [HttpPost("registerUser")]
        public async Task<ActionResult> RegisterUser([FromBody] RegisterUserDto registerUserDto)
        {
            _logger.LogInformation($"Registration Attempt for {registerUserDto.Email}");
            var errors = await _identityUsersManager.Register(registerUserDto);
            try
            {
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

        [HttpGet("roles")]
        public ActionResult<IEnumerable<RoleDto>> GetRoles() {
            return Ok(_identityUsersManager.GetRoles());
        }

        [HttpPost("roles")]
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
    }
}
