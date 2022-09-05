using AutoMapper;
using IdentityUserManagement.API.Data;
using IdentityUserManagement.API.Models.IdentityUsers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IdentityUserManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityUsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IdentityDBContext _context;

        public IdentityUsersController( IMapper mapper, IdentityDBContext context)
        {
            _mapper = mapper;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers() {
            
        }
    }
}
