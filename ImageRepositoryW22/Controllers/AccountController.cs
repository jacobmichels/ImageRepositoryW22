﻿using ImageRepositoryW22.Repositories;
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

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password)
        {
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
        public async Task<IActionResult> Register(string username, string password)
        {
            if(!credentialsValid(username, password))
            {
                return BadRequest("Credentials invalid");
            }

            if(await _userRepository.UserExists(username))
            {
                return Forbid("A user with that name already exists");
            }
            var user = await CreateApplicationUser(username, password);
            var userInserted = await _userRepository.InsertUser(user);

            if (!userInserted)
            {
                return Problem();
            }

            var token = GenerateJWT(user);

            return Ok(token);
        }

        [HttpPost]
        [Authorize]
        //TODO: Decide if this method should exist, and what it should do. Should it invalidate the JWT?
        public async Task<IActionResult> Logout()
        {
            return null;
        }

        [HttpDelete]
        [Authorize]
        //TODO:Should this method invalidate the JWT?
        public async Task<IActionResult> Delete()
        {
            var username = HttpContext.User.Identity.Name;
            var deleted = await _userRepository.DeleteUser(username);
            if (!deleted)
            {
                return BadRequest("User not deleted");
            }

            return Ok();
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

            //set the subject claim to the username of the user
            var claims = new List<Claim>() { new Claim("Username", user.UserName) };

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
            if (password.Length<=6)
            {
                return false;
            }
            return true;
        }
    }
}
