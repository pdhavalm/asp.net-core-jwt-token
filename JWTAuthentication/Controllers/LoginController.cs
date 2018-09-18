using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTAuthentication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private IConfiguration _config;
        public LoginController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login([FromBody]UserModel model)
        {
            IActionResult response = Unauthorized();
            var user = AuthenticateUser(model);

            if (user != null)
            {
                var tokenString = GenerateJWTToken(user);
                response = Ok(new { token = tokenString });
            }
            return response;
        }

        private string GenerateJWTToken(UserModel model)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, model.FullName),
                new Claim(JwtRegisteredClaimNames.Sub, model.Email),
                new Claim("DateOfJoining", model.DateOfJoin.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
       
        private UserModel AuthenticateUser(UserModel model)
        {
            // UserModel user = null;
            if (model.Email.ToLower() == "test@gmail.com" && model.Password.ToLower() == "123")
            {
                model = new UserModel { Email = "testuser@gmail.com", FullName = "abc xyz", DateOfJoin = DateTime.Now.AddYears(-6).ToString() };
            }
            return model;
        }

        [AllowAnonymous]
        public class UserModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string FullName { get; set; }
            public string DateOfJoin { get; set; }
        }
    }
}