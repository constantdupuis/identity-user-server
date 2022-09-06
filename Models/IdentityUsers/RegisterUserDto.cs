using System.ComponentModel.DataAnnotations;

namespace IdentityUserManagement.API.Models.IdentityUsers
{
    public class RegisterUserDto : LoginUserDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
    }
}
