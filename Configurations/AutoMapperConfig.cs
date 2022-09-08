using AutoMapper;
using IdentityUserManagement.API.Data;
using IdentityUserManagement.API.Models.IdentityUsers;
using Microsoft.AspNetCore.Identity;

namespace IdentityUserManagement.API.Configurations
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<ApiUser, UserDto>().ReverseMap();
            CreateMap<ApiUser, UserRegisterDto>().ReverseMap();
            CreateMap<RoleDto, IdentityRole>().ReverseMap();
        }
    }

}
