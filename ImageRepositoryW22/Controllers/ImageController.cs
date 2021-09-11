﻿using ImageRepositoryW22.ImageRepository.Repositories;
using ImageRepositoryW22.Repositories.Models;
using ImageRepositoryW22.Repositories.UserRepository;
using Microsoft.AspNetCore.Authorization;
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
        private readonly string _username;
        public ImageController(IImageRepository imageRepository, IUserRepository userRepository)
        {
            _imageRepository = imageRepository;
            _userRepository = userRepository;
            _username = HttpContext.User.Identity.Name;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get(Guid id) {
            var user = await _userRepository.GetUser(_username);
            var image = await _imageRepository.Get(user, id);
            return Ok(image);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetMine()
        {
            var user = await _userRepository.GetUser(_username);
            var images = await _imageRepository.GetAll(user);
            return Ok(images);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllPublic()
        {
            var images = await _imageRepository.GetAll();
            return Ok(images);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateOne([FromForm] RequestImage imageInfo)
        {
            var user = await _userRepository.GetUser(_username);
            var created = await _imageRepository.Create(user, imageInfo);
            if (created)
            {
                return Ok();
            }
            return BadRequest("Image was not created. Make sure it does not have the same name as another one of your images.");
        }

        [Authorize]
        [HttpPatch]
        public async Task<IActionResult> Update(ImageUpdate image, Guid id)
        {
            var user = await _userRepository.GetUser(_username);
            var updated = await _imageRepository.Update(user, image, id);
            if(updated is not null)
            {
                return Ok(updated);
            }
            return BadRequest("Problem updating image.");
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteOne(Guid id)
        {
            var user = await _userRepository.GetUser(_username);
            var deleted = await _imageRepository.Delete(user, id);
            if (deleted)
            {
                return Ok();
            }
            return BadRequest("Image not deleted");
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteMany(List<Guid> ids)
        {
            var user = await _userRepository.GetUser(_username);
            var deleted = await _imageRepository.Delete(user, ids);
            return Ok($"Deleted {deleted} images.");
        }
    }
}
