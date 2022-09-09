using System.ComponentModel.DataAnnotations;

namespace IdentityUserManagement.API.Models.IdentityUsers
{
    public class RoleUpdateDto
    {
        [Required]
        public string CurrentRoleName { get; set; }
        [Required]
        public string NewRoleName { get; set; }
        public string? NewRoleNormalizedName { get; set; }
    }
}
