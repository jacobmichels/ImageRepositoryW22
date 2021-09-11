using ImageRepositoryW22.Repositories;
using ImageRepositoryW22.Repositories.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageRepositoryW22.ImageRepository.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ImageRepository> _logger;

        public ImageRepository(ApplicationDbContext db, ILogger<ImageRepository> logger)
        {
            _db = db;
            _logger=logger;
        }
        //TODO: Make sure users cannot upload two files with the same name (or not, decide if this is important).
        public async Task<bool> Create(ApplicationUser user, RequestImage image)
        {
            //Make sure users can't upload large files (> 100MB)
            if(image.File.Length> 100000000)
            {
                _logger.LogWarning($"User: {user.UserName} Tried to upload file with size {image.File.Length}");
                return false;
            }

            //make sure the file extension is an image. This is a repository for images only.
            var fileExtension = Path.GetExtension(image.File.FileName);
            if (!IsValidFileExtension(fileExtension))
            {
                _logger.LogWarning($"User: {user.UserName} Tried to upload file with invalid extension: {fileExtension}");
                return false;
            }

            var databaseImage = BuildDatabaseImage(user, image);
            Directory.CreateDirectory(Path.Join("UserImages", $"{user.Id}"));
            using (var stream = File.Create(databaseImage.Path))
            {
                await image.File.CopyToAsync(stream);
            }

            await _db.Images.AddAsync(databaseImage);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occured while trying to save image insertion to db: {ex.Message}");
                return false;
            }

            return true;
        }

        public async Task<bool> Create(ApplicationUser user, List<RequestImage> images)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Delete(ApplicationUser user, Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<int> Delete(ApplicationUser user, List<Guid> ids)
        {
            throw new NotImplementedException();
        }

        //Return image data only here. Check if user is null, and make sure the image is either public or the user owns the image
        public async Task<DatabaseImage> Get(ApplicationUser user, Guid id)
        {
            throw new NotImplementedException();
        }

        //Don't return image data, just info like name
        public async Task<List<DatabaseImage>> GetAll()
        {
            throw new NotImplementedException();
        }

        //Don't return image data, just info like name
        public async Task<List<DatabaseImage>> GetAll(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public async Task<DatabaseImage> Update(ApplicationUser user, ImageUpdate image, Guid id)
        {
            throw new NotImplementedException();
        }

        private DatabaseImage BuildDatabaseImage(ApplicationUser user, RequestImage image)
        {
            var databaseImage = new DatabaseImage()
            {
                Id=Guid.NewGuid(),
                Description = image.Description,
                Name = image.Name,
                Private = image.Private,
                Owner = user,
            };
            //For security, use user.Id and databaseImage.Id for the path so we don't create a path with untrusted user input.
            databaseImage.Path = Path.Join("UserImages",$"{user.Id}", $"{databaseImage.Id}{Path.GetExtension(image.File.FileName)}");
            return databaseImage;
        }

        private bool IsValidFileExtension(string fileExtension)
        {
            var validExtensions = new List<string>() { ".png", ".jpeg", ".jpg", ".gif", ".webp", ".tiff", ".psd", ".raw", ".bmp", ".heif", ".indd", ".svg", ".pdf" };
            if (string.IsNullOrWhiteSpace(fileExtension) || !validExtensions.Contains(fileExtension))
            {
                return false;
            }
            return true;
        }
    }
}
