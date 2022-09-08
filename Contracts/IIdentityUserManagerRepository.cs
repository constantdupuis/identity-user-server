using IdentityUserManagement.API.Models.IdentityUsers;
using Microsoft.AspNetCore.Identity;

namespace IdentityUserManagement.API.Contracts
{
    public interface IIdentityUserManagerRepository
    {
        Task<IEnumerable<IdentityError>> Register(RegisterUserDto userDto);
        Task<IEnumerable<IdentityError>> RegisterAdmin(RegisterUserDto userDto);
        IEnumerable<UserDto> GetUsers();
        IEnumerable<RoleDto> GetRoles();
        Task<IEnumerable<IdentityError>> AddRoleAsync(RoleDto createRoleDto);
        Task<IEnumerable<IdentityError>> DeleteRoleAsync(string roleName);
    }
}

