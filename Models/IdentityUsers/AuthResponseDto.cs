﻿namespace IdentityUserManagement.API.Models.IdentityUsers
{
    public class AuthResponseDto
    {
        public string UserID { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
