namespace IdentityUserManagement.API.Models.IdentityUsers
{
    public class RoleUpdateDto
    {
        public string CurrentRoleName { get; set; }
        public string NewRoleName { get; set; }
        public string NewRoleNormalizedName { get; set; }
    }
}
