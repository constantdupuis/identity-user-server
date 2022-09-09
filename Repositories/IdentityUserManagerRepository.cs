using AutoMapper;
using IdentityUserManagement.API.Configurations;
using IdentityUserManagement.API.Contracts;
using IdentityUserManagement.API.Data;
using IdentityUserManagement.API.Models.IdentityUsers;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IdentityUserManagement.API.Repositories
{
    public class IdentityUserManagerRepository : IIdentityUserManagerRepository
    {
        private readonly ILogger<IdentityUserManagerRepository> _logger;
        private readonly IMapper _mapper;
        private readonly UserManager<ApiUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private ApiUser _apiUser;

        public IdentityUserManagerRepository(
            ILogger<IdentityUserManagerRepository> logger, 
            IMapper mapper, 
            UserManager<ApiUser> userManager, 
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public IEnumerable<UserDto> GetUsers()
        {
            return _mapper.Map<IEnumerable<UserDto>>(_userManager.Users);
        }

        public async Task<IEnumerable<IdentityError>> Register(UserRegisterDto userDto)
        {
            _logger.LogInformation($"Attempt to register a user with email ${userDto.Email}");
            var result =  await RegisterUser(userDto);
            return result.Errors;
        }

        public async Task<IEnumerable<IdentityError>> RegisterAdmin(UserRegisterDto userDto)
        {
            _logger.LogInformation($"Attempt to register an administrator user with email ${userDto.Email}");
            var result = await RegisterUser(userDto);

            if (result.Succeeded)
            {
                var resultAddToRole = await EnrollHasAdmin();
                return resultAddToRole.Errors;
            }

            return result.Errors;
        }

        public async Task<AuthResponseDto> Login(UserLoginDto loginDto)
        {
            _logger.LogInformation($"Looking for user with email {loginDto.Email}.");
            _apiUser = await _userManager.FindByEmailAsync(loginDto.Email);
            bool isValidUser = await _userManager.CheckPasswordAsync(_apiUser, loginDto.Password);

            if (_apiUser == null || isValidUser == false)
            {
                _logger.LogWarning($"User with email {_apiUser.Email} was not found.");
                return null;
            }
            var token = await GenerateToken();
            _logger.LogInformation($"Token generated for user with email {loginDto.Email} | Token : [{token}].");
            return new AuthResponseDto
            {
                Token = token,
                UserID = _apiUser.Id,
                RefreshToken = await CreateRefreshToken()
            };
        }

        public async Task<IEnumerable<IdentityError>> UpdateUserAsync(UserDto userDto)
        {
            _logger.LogInformation($"Attempt to update user with email ${userDto.Email}");
            
            var user = await _userManager.FindByEmailAsync(userDto.Email);

            if (user == null)
            {
                var identityResult = IdentityResult.Failed(
                    new IdentityError[] { new IdentityError() { Code = "Not Found", Description = $"Email name [{userDto.Email}] not found" } }
                    );
                return identityResult.Errors;
            }

            _mapper.Map(userDto, user);

            var result = await _userManager.UpdateAsync(user);
            return result.Errors;
        }

        public async Task<IEnumerable<IdentityError>> DeleteUserAsync(UserDto userDto)
        {
            _logger.LogInformation($"Attempt to delete user with email ${userDto.Email}");
            var user = _mapper.Map<ApiUser>(userDto);
            var result = await _userManager.DeleteAsync(user);
            return result.Errors;
        }

        public async Task<IEnumerable<IdentityError>> DeleteUserAsync(string email)
        {
            _logger.LogInformation($"Attempt to delete user with email ${email}");
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                var identityResult = IdentityResult.Failed(
                    new IdentityError[] { new IdentityError() { Code = "Not Found", Description = $"Email name [{email}] not found" } }
                    );
                return identityResult.Errors;
            }

            var result = await _userManager.DeleteAsync(user);
            return result.Errors;
        }

        public IEnumerable<RoleDto> GetRoles()
        {
            return _mapper.Map<IEnumerable<RoleDto>>(_roleManager.Roles);
        }

        public async Task<IEnumerable<IdentityError>> AddRoleAsync(RoleDto createRoleDto)
        {
            if (string.IsNullOrEmpty(createRoleDto.NormalizedName))
            {
                createRoleDto.NormalizedName = createRoleDto.Name.ToUpper();
            }

            var identityRole = _mapper.Map<IdentityRole>(createRoleDto);
            var result = await _roleManager.CreateAsync(identityRole);
            return result.Errors;
        }

        public async Task<IEnumerable<IdentityError>> DeleteRoleAsync(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                var identityResult = IdentityResult.Failed( 
                    new IdentityError[] { new IdentityError() { Code = "Not Found", Description = $"Role name [{roleName}] not found" } }
                    );
                return identityResult.Errors;
            }

            var result = await _roleManager.DeleteAsync(role);

            return result.Errors;
        }

        public async Task<IEnumerable<IdentityError>> UpdateRoleAsync( RoleUpdateDto roleUpdate)
        {
            var role = await _roleManager.FindByNameAsync(roleUpdate.CurrentRoleName);
            if (role == null)
            {
                var identityResult = IdentityResult.Failed(
                    new IdentityError[] { new IdentityError() { Code = "Not Found", Description = $"Role name [{roleUpdate.CurrentRoleName}] not found" } }
                    );
                return identityResult.Errors;
            }

            role.Name = roleUpdate.NewRoleName;
            if (string.IsNullOrEmpty(roleUpdate.NewRoleNormalizedName))
            {
                roleUpdate.NewRoleNormalizedName = roleUpdate.NewRoleName.ToUpper();
            }
            role.NormalizedName = roleUpdate.NewRoleNormalizedName;

            var result = await _roleManager.UpdateAsync(role);

            return result.Errors;
        }

        private async Task<IdentityResult> RegisterUser(UserRegisterDto userDto)
        {
            _apiUser = _mapper.Map<ApiUser>(userDto);
            _apiUser.UserName = _apiUser.Email;
            var result = await _userManager.CreateAsync(_apiUser, userDto.Password);

            if (result.Succeeded)
            {
                var resultAddToRole = await _userManager.AddToRoleAsync(_apiUser, BaseRoleNames.User);
                return resultAddToRole;
            }

            return result;
        }

        private async Task<IdentityResult> EnrollHasAdmin()
        {
            var resultAddToRole = await _userManager.AddToRoleAsync(_apiUser, BaseRoleNames.Administrator);
            return resultAddToRole;
        }
        public Task<bool> UserExists(string userEMail)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> RoleExists(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            return role != null;
        }

        private async Task<string> GenerateToken()
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"])
                );
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var roles = await _userManager.GetRolesAsync(_apiUser);
            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
            var userClaims = await _userManager.GetClaimsAsync(_apiUser);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, _apiUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, _apiUser.Email),
                new Claim("uid", _apiUser.Id), // own claim

            }.Union(userClaims).Union(roleClaims);

            // TODO check that all parameters are available from configuration file

            _logger.LogInformation("Token duration {0}min", _configuration["JwtSettings:DurationInMinutes"]);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    Convert.ToInt32(_configuration["JwtSettings:DurationInMinutes"])
                    ),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> CreateRefreshToken()
        {
            await _userManager.RemoveAuthenticationTokenAsync(_apiUser, AppJwtConstants.LoginProvider, AppJwtConstants.RefreshToken);
            var newRefreshToken = await _userManager.GenerateUserTokenAsync(_apiUser, AppJwtConstants.LoginProvider, AppJwtConstants.RefreshToken);
            var resutl = await _userManager.SetAuthenticationTokenAsync(_apiUser, AppJwtConstants.LoginProvider, AppJwtConstants.RefreshToken, newRefreshToken);
            return newRefreshToken;
        }
    }
}
