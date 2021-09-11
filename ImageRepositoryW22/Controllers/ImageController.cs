using ImageRepositoryW22.ImageRepository.Repositories;
using ImageRepositoryW22.Repositories.Models;
using ImageRepositoryW22.Repositories.UserRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using static ImageRepositoryW22.Utilities.Enums.ImageRepositoryEnums;

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

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get(Guid id) {
            var user = await _userRepository.GetUser(GetUserName());
            var imageData = await _imageRepository.Get(user, id);
            if(imageData is null)
            {
                return NotFound();
            }
            return File(imageData.Data, "application/octet-stream", imageData.FileName);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetMine()
        {
            var user = await _userRepository.GetUser(GetUserName());
            var images = await _imageRepository.GetMine(user);
            return Ok(images);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllPublic()
        {
            var images = await _imageRepository.GetAllPublic();
            return Ok(images);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateOne([FromForm] RequestImage imageInfo)
        {
            var user = await _userRepository.GetUser(GetUserName());
            var createdStatus = await _imageRepository.Create(user, imageInfo);
            if (createdStatus == ImageCreateStatus.Success)
            {
                return Ok(new { Message = "Image created." });
            }
            else if (createdStatus == ImageCreateStatus.FileTooLarge)
            {
                return BadRequest(new { ErrorMessage = "Image was not created due to the file being too large. Only files under 100MB allowed." });
            }
            else if (createdStatus == ImageCreateStatus.BadExtension)
            {
                return BadRequest(new { ErrorMessage = "Image was not created due to the file extension not being a recognized image extension." });
            }
            else
            {
                return StatusCode(500,new { ErrorMessage = "Image was not saved due to a database error. Please try again later." });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateMany([FromForm] List<IFormFile> files)
        {
            var user = await _userRepository.GetUser(GetUserName());
            var createdStatus = await _imageRepository.Create(user, files);
            if (createdStatus == ImageBulkCreateStatus.AllSuccess)
            {
                return Ok(new { Message = "Images successfully created." });
            }
            else if (createdStatus == ImageBulkCreateStatus.AllFail)
            {
                return BadRequest(new { ErrorMessage = "All images failed to be created. Please check their sizes and file extensions." });
            }
            else if (createdStatus == ImageBulkCreateStatus.AtLeastOneFail)
            {
                return BadRequest(new { ErrorMessage = "At least one image failed to be created. Please check their sizes and file extensions." });
            }
            else
            {
                return StatusCode(500, new { ErrorMessage = "Images were not saved due to a database error. Please try again later." });
            }
        }

        [Authorize]
        [HttpPatch]
        public async Task<IActionResult> Update(ImageUpdate image)
        {
            var user = await _userRepository.GetUser(GetUserName());
            var updated = await _imageRepository.Update(user, image);
            if(updated is not null)
            {
                return Ok(updated);
            }
            return BadRequest("Problem updating image.");
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Delete(List<Guid> ids)
        {
            var user = await _userRepository.GetUser(GetUserName());
            var deletedStatus = await _imageRepository.Delete(user, ids);
            if(deletedStatus == ImageBulkDeleteStatus.AllSuccess)
            {
                return Ok(new { Message= "Images successfully deleted." });
            }
            else if(deletedStatus == ImageBulkDeleteStatus.AtLeastOneFail)
            {
                return Ok(new { Message = "Some images were not deleted. Please check that you own the images you are trying to delete." });
            }
            else if(deletedStatus == ImageBulkDeleteStatus.AllFail)
            {
                return NotFound(new { ErrorMessage = "No images were deleted. Please check that you own the images you are trying to delete." });
            }
            else
            {
                return StatusCode(500, new { ErrorMessage = "Image was not deleted due to a database error.Please try again later." });
            }
        }

        private string GetUserName()
        {
            return HttpContext.User.Claims.FirstOrDefault(claim => claim.Type=="Username").Value;
        }
    }
}
