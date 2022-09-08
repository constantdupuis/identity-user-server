using IdentityUserManagement.API.Models.IdentityUsers;
using Microsoft.AspNetCore.Identity;

namespace IdentityUserManagement.API.Contracts
{
    public interface IIdentityUserManagerRepository
    {
        Task<IEnumerable<IdentityError>> Register(UserRegisterDto userDto);
        Task<IEnumerable<IdentityError>> RegisterAdmin(UserRegisterDto userDto);
        IEnumerable<UserDto> GetUsers();
        IEnumerable<RoleDto> GetRoles();
        Task<IEnumerable<IdentityError>> AddRoleAsync(RoleDto createRoleDto);
        Task<IEnumerable<IdentityError>> DeleteRoleAsync(string roleName);
    }
}

