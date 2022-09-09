using System.ComponentModel.DataAnnotations;

namespace IdentityUserManagement.API.Models.IdentityUsers
{
    public class UserDto
    {
        [Required]
        public String FirstName { get; set; }
        [Required]
        public String LastName { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        public bool? EmailConfirmed { get; set; } = false;
        public string? PhoneNumber { get; set; } = "";
        public bool? PhoneNumberConfirmed { get; set; } = false;
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool? LockoutEnabled { get; set; } = false;
        public int? AccessFailedCount { get; set; } = 0;
    }
}
