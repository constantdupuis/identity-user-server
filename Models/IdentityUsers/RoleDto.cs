using System.ComponentModel.DataAnnotations;

namespace IdentityUserManagement.API.Models.IdentityUsers
{
    public class RoleDto
    {
        [Required]
        public string Name { get; set; }
        public string? NormalizedName { get; set; }
    }
}
