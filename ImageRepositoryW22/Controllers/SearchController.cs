using ImageRepositoryW22.ImageRepository.Repositories;
using ImageRepositoryW22.Repositories.UserRepository;
using ImageRepositoryW22.Utilities.ControllerUtility;
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
    public class SearchController: ControllerBase
    {
        private readonly IImageRepository _imageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IControllerUtility _controllerUtility;

        public SearchController(IImageRepository imageRepository, IUserRepository userRepository, IControllerUtility controllerUtility)
        {
            _imageRepository = imageRepository;
            _userRepository = userRepository;
            _controllerUtility = controllerUtility;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SearchAllPublic(string text)
        {
            return Ok(await _imageRepository.SearchAllPublic(text));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SearchMine(string text)
        {
            var user = await _userRepository.GetUser(_controllerUtility.GetUserId(HttpContext));
            return Ok(await _imageRepository.SearchMine(user, text));
        }
    }
}
