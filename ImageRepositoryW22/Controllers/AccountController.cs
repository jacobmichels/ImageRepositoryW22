using ImageRepositoryW22.Repositories;
using ImageRepositoryW22.Repositories.UserRepository;
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

        public AccountController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password)
        {
            return null;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(string username, string password)
        {
            return null;

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout(string username, string password)
        {
            return null;

        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(string username, string password)
        {
            return null;

        }
    }
}
