using AutoMapper;
using IdentityUserManagement.API.Data;
using IdentityUserManagement.API.Models.IdentityUsers;

namespace IdentityUserManagement.API.Configurations
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<ApiUser, UserDto>().ReverseMap();
            CreateMap<ApiUser, RegisterUserDto>().ReverseMap();
        }
    }

}
