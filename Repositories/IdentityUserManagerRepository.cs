using AutoMapper;
using IdentityUserManagement.API.Configurations;
using IdentityUserManagement.API.Contracts;
using IdentityUserManagement.API.Data;
using IdentityUserManagement.API.Models.IdentityUsers;
using Microsoft.AspNetCore.Identity;

namespace IdentityUserManagement.API.Repositories
{
    public class IdentityUserManagerRepository : IIdentityUserManagerRepository
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApiUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private ApiUser _apiUser;

        public IdentityUserManagerRepository(IMapper mapper, UserManager<ApiUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IEnumerable<UserDto> GetUsers()
        {
            return _mapper.Map<IEnumerable<UserDto>>(_userManager.Users);
        }

        public async Task<IEnumerable<IdentityError>> Register(UserRegisterDto userDto)
        {
            var result =  await RegisterUser(userDto);
            return result.Errors;
        }

        public async Task<IEnumerable<IdentityError>> RegisterAdmin(UserRegisterDto userDto)
        {
            var result = await RegisterUser(userDto);

            if (result.Succeeded)
            {
                var resultAddToRole = await EnrollHasAdmin();
                return resultAddToRole.Errors;
            }

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

        
    }
}
