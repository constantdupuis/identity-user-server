using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityUserManagement.API.Data
{
    public class IdentityDBContext : IdentityDbContext<ApiUser>
    {
        public IdentityDBContext(DbContextOptions options) : base(options)
        {
        }
    }
}
