using BlogsAPI.Helpers;
using BlogsAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlogsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly AppSettings _appSettings;

        public AuthorizationController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        
        [HttpPost]
        public IActionResult Authenticate([FromBody] AuthenticateModel model)
        {
            if (model.Password.ToLower() != "admin")
                return null;

            //Build the claims with multiple Roles
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, model.Username));
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));
           
            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var user = new User
            {
                Username = model.Username,
                FirstName = "Admin",
                LastName = "User",
                Password = null,
                Role = "Admin",
                Token = tokenHandler.WriteToken(token)
            };

           return Ok(user);
        }
    
    }
}
