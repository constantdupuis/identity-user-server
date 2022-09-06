using IdentityUserManagement.API.Models.IdentityUsers;
using Microsoft.AspNetCore.Identity;

namespace IdentityUserManagement.API.Contracts
{
    public interface IIdentityUserManagerRepository
    {
        Task<IEnumerable<IdentityError>> Register(RegisterUserDto userDto);
        Task<IEnumerable<IdentityError>> RegisterAdmin(RegisterUserDto userDto);
        IEnumerable<UserDto> GetUsers();
    }
}
