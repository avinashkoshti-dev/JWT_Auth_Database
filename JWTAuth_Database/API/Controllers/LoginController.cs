using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly JwtauthenticationContext dbContext;

        public LoginController(IConfiguration config, JwtauthenticationContext DbContext)
        {
            _config = config;
            dbContext = DbContext;
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login([FromBody] UserInfo userLogin)
        {
            var user = Authenticate(userLogin);
            if (user != null)
            {
                var token = GenerateToken(user);
                return Ok(token);
            }

            return NotFound("user not found");
        }

        //To authenticate user
        private UserInfo? Authenticate(UserInfo userLogin)
        {
            var currentUser = dbContext.UserInfos.FirstOrDefault(x => x.Email == userLogin.Email.ToLower() && x.Password == userLogin.Password.ToLower());
            if (currentUser != null)
            {
                return currentUser;
            }
            return null;
        }

        // To generate token
        private string GenerateToken(UserInfo user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.Role,user.UserName),
                new Claim(ClaimTypes.Email,user.Email)
            };
            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
