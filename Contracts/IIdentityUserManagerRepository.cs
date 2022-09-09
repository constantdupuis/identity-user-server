using IdentityUserManagement.API.Models.IdentityUsers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityUserManagement.API.Contracts
{
    public interface IIdentityUserManagerRepository
    {
        Task<IEnumerable<IdentityError>> Register(UserRegisterDto userDto);
        Task<IEnumerable<IdentityError>> RegisterAdmin(UserRegisterDto userDto);
        IEnumerable<UserDto> GetUsers();
        IEnumerable<RoleDto> GetRoles();
        Task<IEnumerable<IdentityError>> UpdateUserAsync( UserDto userDto);
        Task<IEnumerable<IdentityError>> DeleteUserAsync(UserDto userDto);
        Task<IEnumerable<IdentityError>> DeleteUserAsync(string userEmail);
        Task<IEnumerable<IdentityError>> AddRoleAsync(RoleDto createRoleDto);
        Task<IEnumerable<IdentityError>> DeleteRoleAsync(string roleName);
        Task<IEnumerable<IdentityError>> UpdateRoleAsync(RoleUpdateDto roleUpdate);
        Task<bool> UserExists(string userEMail);
        Task<bool> RoleExists(string roleName);
    }
}

