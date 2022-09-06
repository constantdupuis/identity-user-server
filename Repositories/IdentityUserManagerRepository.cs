using AutoMapper;
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
        private ApiUser _apiUser;

        public IdentityUserManagerRepository(IMapper mapper, UserManager<ApiUser> userManager)
        {
            _mapper = mapper;
            _userManager = userManager;
        }

        public IEnumerable<UserDto> GetUsers()
        {
            return _mapper.Map<IEnumerable<UserDto>>(_userManager.Users);
        }

        public async Task<IEnumerable<IdentityError>> Register(RegisterUserDto userDto)
        {
            var result =  await RegisterUser(userDto);
            return result.Errors;
        }

        public async Task<IEnumerable<IdentityError>> RegisterAdmin(RegisterUserDto userDto)
        {
            var result = await RegisterUser(userDto);

            if (result.Succeeded)
            {
                var resultAddToRole = await EnrollHasAdmin();
                return resultAddToRole.Errors;
            }

            return result.Errors;
        }

        private async Task<IdentityResult> RegisterUser(RegisterUserDto userDto)
        {
            _apiUser = _mapper.Map<ApiUser>(userDto);
            _apiUser.UserName = _apiUser.Email;
            var result = await _userManager.CreateAsync(_apiUser, userDto.Password);

            if (result.Succeeded)
            {
                var resultAddToRole = await _userManager.AddToRoleAsync(_apiUser, "User");
                return resultAddToRole;
            }

            return result;
        }

        private async Task<IdentityResult> EnrollHasAdmin()
        {
            var resultAddToRole = await _userManager.AddToRoleAsync(_apiUser, "Administrator");
            return resultAddToRole;
        }
    }
}
