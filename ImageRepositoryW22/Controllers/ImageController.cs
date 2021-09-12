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

        /// <summary>
        /// Get image by id.
        /// </summary>
        /// <remarks>
        /// Sample request
        /// 
        ///     GET /Image/Get?id=d0836544-8db0-4458-9ca9-445978ba08a4
        /// </remarks>
        /// <param name="id"></param>
        /// <response code="200">Successfully find and return the image.</response>
        /// <response code="404">Image either does not exist, or the user does not have access to it.</response>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Get(Guid id) {
            var user = await _userRepository.GetUser(GetUserId());
            var imageData = await _imageRepository.Get(user, id);
            if(imageData is null)
            {
                return NotFound();
            }
            return File(imageData.Data, "application/octet-stream", imageData.FileName);
        }

        /// <summary>
        /// Get info on all the user's images.
        /// </summary>
        /// <remarks>
        /// Sample request
        /// 
        ///     GET /Image/GetMine
        /// </remarks>
        /// <response code="200">Successfully return a list with every image's info owned by the user.</response>
        /// <response code="401">Unauthorized request.</response>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetMine()
        {
            var user = await _userRepository.GetUser(GetUserId());
            var images = await _imageRepository.GetMine(user);
            return Ok(images);
        }

        /// <summary>
        /// Get all public images.
        /// </summary>
        /// <remarks>
        /// Sample request
        /// 
        ///     GET /Image/GetAllPublic
        /// </remarks>
        /// <response code="200">Successfully find and return all public images info.</response>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAllPublic()
        {
            var images = await _imageRepository.GetAllPublic();
            return Ok(images);
        }

        /// <summary>
        /// Upload a single image.
        /// </summary>
        /// <remarks>
        /// Sample request
        /// 
        ///     POST /Image/CreateOne
        ///     FormData
        ///     {
        ///         Name : The city
        ///         Description : Skyline
        ///         Private : false
        ///         File : {FILE} (if multiple files are uploaded, only the first in the sequence will be processed.)
        ///     }
        /// </remarks>
        /// <response code="200">Successfully create the image and return it's info.</response>
        /// <response code="400">Failed to create the image. Likely causes are either image size > 100MB or file has a non-image extension. </response>
        /// <response code="401">Unauthorized request.</response>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateOne([FromForm] RequestImage imageInfo)
        {
            var user = await _userRepository.GetUser(GetUserId());
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

        /// <summary>
        /// Upload a single image.
        /// </summary>
        /// <remarks>
        /// Sample request
        /// 
        ///     POST /Image/CreateMany
        ///     FormData
        ///     {
        ///         files : {MULTIPLE FILES}
        ///     }
        /// </remarks>
        /// <response code="200">Successfully created the images and returned their info.</response>
        /// <response code="400">Failed to create the images. Likely causes are either image size > 100MB or file has a non-image extension. If one image fails, they all fail.</response>
        /// <response code="401">Unauthorized request.</response>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateMany([FromForm] List<IFormFile> files)
        {
            var user = await _userRepository.GetUser(GetUserId());
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
        /// <summary>
        /// Update an image's info.
        /// </summary>
        /// <remarks>
        /// Sample request
        /// 
        ///     PATCH /Image/Update
        ///     Raw
        ///     {
        ///         "id": "d0836544-8db0-4458-9ca9-445978ba08a4",
        ///         "name": "new name",
        ///         "description":"new description",
        ///         "private": "true"
        ///     }
        /// </remarks>
        /// <response code="200">Successfully update the image and return it's new info.</response>
        /// <response code="400">Failed to update the image. This will happen if the image to update cannot be found.</response>
        /// <response code="401">Unauthorized request.</response>
        [Authorize]
        [HttpPatch]
        public async Task<IActionResult> Update(ImageUpdate image)
        {
            var user = await _userRepository.GetUser(GetUserId());
            var updated = await _imageRepository.Update(user, image);
            if(updated is not null)
            {
                return Ok(updated);
            }
            return BadRequest("Problem updating image.");
        }

        /// <summary>
        /// Delete an image.
        /// </summary>
        /// <remarks>
        /// Sample request
        /// 
        ///     Delete /Image/Update
        ///     Raw
        ///     [
        ///         "d0836544-8db0-4458-9ca9-445978ba08a4",
        ///         "1d12da58-623d-4148-b684-16a1224169eb"
        ///     ]
        /// </remarks>
        /// <response code="200">Successfully deleted the image(s).</response>
        /// <response code="400">Failed to delete the image(s). This will happen if the image(s) to delete cannot be found. If one image in the list fails to be deleted, the rest will be not be deleted.</response>
        /// <response code="401">Unauthorized request.</response>
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Delete(List<Guid> ids)
        {
            var user = await _userRepository.GetUser(GetUserId());
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

        private Guid GetUserId()
        {
            var id = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == "id");
            if(id is null)
            {
                return Guid.Empty;
            }
            return Guid.Parse(id.Value);
        }
    }
}
