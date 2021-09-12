using ImageRepositoryW22.Repositories;
using ImageRepositoryW22.Repositories.Models;
using ImageRepositoryW22.Repositories.UserRepository;
using ImageRepositoryW22.Utilities.PasswordUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Security.Claims;

namespace ImageRepositoryW22.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AccountController: ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordUtilities _passwordUtils;
        private readonly IConfiguration _config;

        public AccountController(IUserRepository userRepository, IPasswordUtilities passwordUtils, IConfiguration config)
        {
            _userRepository = userRepository;
            _passwordUtils = passwordUtils;
            _config = config;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var userid = GetUserId();
            var user = await _userRepository.GetUser(userid);
            if (user is null)
            {
                return BadRequest(new { ErrorMessage = "You are using the unauthorized token of a deleted user." });
            }
            return Ok(new { Username = user.UserName });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserCredentials credentials)
        {
            string username = credentials.UserName;
            string password = credentials.Password;

            var user = await _userRepository.GetUser(username);
            if(user is null)
            {
                return Forbid();
            }
            if(!await _passwordUtils.VerifyPasswordAysnc(password, user.PasswordHash))
            {
                return Forbid();
            }

            var token = GenerateJWT(user);

            return Ok(token);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserCredentials credentials)
        {
            string username = credentials.UserName;
            string password = credentials.Password;

            if(!credentialsValid(username, password))
            {
                return BadRequest(new { ErrorMessage = "Bad credentials. Make sure your password is at least 6 characters, and your username is not blank." });
            }

            if(await _userRepository.UserExists(username))
            {
                return BadRequest(new { ErrorMessage = "A user with that username already exists." });
            }
            var user = await CreateApplicationUser(username, password);
            var userInserted = await _userRepository.InsertUser(user);

            if (!userInserted)
            {
                return StatusCode(500, new { ErrorMessage = "User not inserted into database. Please try again later." });
            }

            var token = GenerateJWT(user);

            return Ok(token);
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete()
        {
            var deleted = await _userRepository.DeleteUser(GetUserId());
            if (!deleted)
            {
                return BadRequest(new { ErrorMessage = "User not deleted." });
            }

            return Ok(new { Message="User deleted successfully." });
        }

        private async Task<ApplicationUser> CreateApplicationUser(string username, string password)
        {
            var hashedPassword = await _passwordUtils.HashPasswordAsync(password);
            var user = new ApplicationUser(username, hashedPassword);
            return user;
        }

        private string GenerateJWT(ApplicationUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            //set a claim for the id of the user
            var claims = new List<Claim>() { new Claim("id", user.Id.ToString()) };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool credentialsValid(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }
            if (password.Length<6)
            {
                return false;
            }
            return true;
        }
        private Guid GetUserId()
        {
            return Guid.Parse(HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "id").Value);
        }
    }
}
