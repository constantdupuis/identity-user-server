using System.ComponentModel.DataAnnotations;

namespace IdentityUserManagement.API.Models.IdentityUsers
{
    public class UserRegisterDto : UserLoginDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
    }
}
