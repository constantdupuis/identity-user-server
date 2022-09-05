namespace IdentityUserManagement.API.Models.IdentityUsers
{
    public class UserDto
    {
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; } = false;
        public string PhoneNumber { get; set; } = "";
        public bool PhoneNumberConfirmed { get; set; } = false;
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; } = false;
        public int AccessFailedCount { get; set; } = 0;
    }
}
