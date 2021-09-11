using ImageRepositoryW22.Repositories;
using ImageRepositoryW22.Repositories.Models;
using ImageRepositoryW22.Repositories.UserRepository;
using ImageRepositoryW22.Utilities.PasswordUtilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageRepositoryW22.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AccountController: ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordUtilities _passwordUtils;

        public AccountController(IUserRepository userRepository, IPasswordUtilities passwordUtils)
        {
            _userRepository = userRepository;
            _passwordUtils = passwordUtils;
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
            //TODO: Generate JWT and return it
            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        //TODO: Make sure users cannot register with a blank or whitespace only username or password
        public async Task<IActionResult> Register(string username, string password)
        {
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

            return Ok();
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
    }
}
