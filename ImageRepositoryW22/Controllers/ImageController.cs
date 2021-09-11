using ImageRepositoryW22.ImageRepository.Repositories;
using ImageRepositoryW22.Repositories.Models;
using ImageRepositoryW22.Repositories.UserRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageRepositoryW22.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ImageController: ControllerBase
    {
        private readonly IImageRepository _imageRepository;
        private readonly IUserRepository _userRepository;
        public ImageController(IImageRepository imageRepository, IUserRepository userRepository)
        {
            _imageRepository = imageRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid id) {
            return null;
        }

        [HttpGet]
        public async Task<IActionResult> GetMine()
        {
            return null;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllPublic()
        {
            return null;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOne([FromForm] RequestImage imageInfo)
        {
            return null;
        }

        [HttpPatch]
        public async Task<IActionResult> Update(RequestImage image, Guid id)
        {
            return null;
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteOne(Guid id)
        {
            return null;
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteMany(List<Guid> id)
        {
            return null;
        }
    }
}
