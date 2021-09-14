using ImageRepositoryW22.Repositories;
using ImageRepositoryW22.Repositories.Models;
using ImageRepositoryW22.Utilities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static ImageRepositoryW22.Utilities.Enums.ImageRepositoryEnums;

namespace ImageRepositoryW22.ImageRepository.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ImageRepository> _logger;

        public ImageRepository(ApplicationDbContext db, ILogger<ImageRepository> logger)
        {
            _db = db;
            _logger = logger;
        }
        public async Task<(ImageCreateStatus, ImageInfo)> Create(ApplicationUser user, RequestImage image)
        {
            //Make sure users can't upload large files (> 100MB)
            if (image.File.Length > 100000000)
            {
                _logger.LogWarning($"User: {user.UserName} Tried to upload file with size {image.File.Length}");
                return (ImageCreateStatus.FileTooLarge,null);
            }

            //make sure the file extension is an image. This is a repository for images only.
            var fileExtension = Path.GetExtension(image.File.FileName);
            if (!IsValidFileExtension(fileExtension))
            {
                _logger.LogWarning($"User: {user.UserName} Tried to upload file with invalid extension: {fileExtension}");
                return (ImageCreateStatus.BadExtension,null);
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
                return (ImageCreateStatus.DatabaseError,null);
            }

            return (ImageCreateStatus.Success,MapToImageInfo(databaseImage));
        }

        public async Task<(ImageBulkCreateStatus,List<ImageInfo>)> Create(ApplicationUser user, List<IFormFile> images)
        {
            //validation loop
            foreach (var image in images)
            {
                //Make sure users can't upload large files (> 100MB)
                if (image.Length > 100000000)
                {
                    _logger.LogWarning($"User: {user.UserName} Tried to upload file with size {image.Length}");
                    return (ImageBulkCreateStatus.Fail,null);
                }
                //Make sure the file extension is an image. This is a repository for images only.
                var fileExtension = Path.GetExtension(image.FileName);
                if (!IsValidFileExtension(fileExtension))
                {
                    _logger.LogWarning($"User: {user.UserName} Tried to upload file with invalid extension: {fileExtension}");
                    return (ImageBulkCreateStatus.Fail,null);
                }
            }

            //once we know all images are ok to be created, we create them.
            var imagesToReturn = new List<DatabaseImage>();
            foreach (var image in images)
            {
                var databaseImage = BuildDatabaseImageBulk(user, image);
                Directory.CreateDirectory(Path.Join("UserImages", $"{user.Id}"));
                using (var stream = File.Create(databaseImage.Path))
                {
                    await image.CopyToAsync(stream);
                }

                await _db.Images.AddAsync(databaseImage);
                imagesToReturn.Add(databaseImage);
            }

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occured while trying to save bulk image insertion to db: {ex.Message}");
                return (ImageBulkCreateStatus.DatabaseError,null);
            }

            return (ImageBulkCreateStatus.Success, MapListToImageInfo(imagesToReturn));
        }

        public async Task<ImageBulkDeleteStatus> Delete(ApplicationUser user, List<Guid> ids)
        {
            if(ids is null || ids.Count == 0)
            {
                return ImageBulkDeleteStatus.Fail;
            }

            //validation loop
            foreach (var id in ids)
            {
                var image = await Get(user, id);
                if (image is null)
                {
                    return ImageBulkDeleteStatus.Fail;
                }
            }

            //now since we know every id in the list is valid, we can delete their associated images
            foreach(var id in ids)
            {
                var image = await _db.Images.FirstOrDefaultAsync(x => x.Id == id);

                File.Delete(image.Path);

                _db.Remove(image);
            }

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occured while trying to save bulk image deletion to db: {ex.Message}");
                return ImageBulkDeleteStatus.DatabaseError;
            }

            return ImageBulkDeleteStatus.Success;
        }

        //Return image data only here. Check if user is null, and make sure the image is either public or the user owns the image
        public async Task<ImageData> Get(ApplicationUser user, Guid id)
        {
            var image = await _db.Images.FirstOrDefaultAsync(image => image.Id == id);
            if (image == null)
            {
                return null;
            }
            if(user is null && image.Private == true)
            {
                return null;
            }
            //If the image is private, make sure the user is the owner.
            if (image.Private == true && image.Owner.UserName != user.UserName)
            {
                return null;
            }
            return await MapToImageData(image);
        }

        public async Task<List<ImageInfo>> GetAllPublic()
        {
            var databaseImages = await _db.Images.Where(image => image.Private == false).ToListAsync();
            return MapListToImageInfo(databaseImages);
        }

        //Don't return image data, just info like name
        public async Task<List<ImageInfo>> GetMine(ApplicationUser user)
        {
            var databaseImages = await _db.Images.Where(image => image.Owner.UserName == user.UserName).ToListAsync();
            return MapListToImageInfo(databaseImages);
        }

        public async Task<ImageInfo> Update(ApplicationUser user, ImageUpdate newData)
        {
            var image = await _db.Images.FirstOrDefaultAsync(image => image.Id == newData.Id && image.Owner.UserName == user.UserName);
            if (image is null)
            {
                return null;
            }
            _db.Images.Remove(image);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occured while trying to save image removal for update to db: {ex.Message}");
                return null;
            }

            image.Private = newData.Private;
            image.Name = newData.Name;
            image.Description = newData.Description;

            await _db.Images.AddAsync(image);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occured while trying to save image update to db: {ex.Message}");
                return null;
            }

            return MapToImageInfo(image);
        }

        private DatabaseImage BuildDatabaseImage(ApplicationUser user, RequestImage image)
        {
            var databaseImage = new DatabaseImage()
            {
                Id = Guid.NewGuid(),
                Description = image.Description,
                Name = image.Name,
                Private = image.Private,
                Owner = user,
                FileName=image.File.FileName
            };
            //For security, use user.Id and databaseImage.Id for the path so we don't create a path with untrusted user input.
            databaseImage.Path = Path.Join("UserImages", $"{user.Id}", $"{databaseImage.Id}{Path.GetExtension(image.File.FileName)}");
            return databaseImage;
        }

        private DatabaseImage BuildDatabaseImageBulk(ApplicationUser user, IFormFile image)
        {
            var databaseImage = new DatabaseImage()
            {
                Id = Guid.NewGuid(),
                Name = image.FileName,
                Private = true,
                Owner = user,
                FileName=image.FileName
            };
            //For security, use user.Id and databaseImage.Id for the path so we don't create a path with untrusted user input.
            databaseImage.Path = Path.Join("UserImages", $"{user.Id}", $"{databaseImage.Id}{Path.GetExtension(image.FileName)}");
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

        private List<ImageInfo> MapListToImageInfo(List<DatabaseImage> images)
        {
            var imageInfoList = new List<ImageInfo>();
            foreach (var image in images)
            {
                var imageInfo = MapToImageInfo(image);
                imageInfoList.Add(imageInfo);
            }

            return imageInfoList;
        }

        private ImageInfo MapToImageInfo(DatabaseImage image)
        {
            return new ImageInfo()
            {
                Id = image.Id,
                Description = image.Description,
                Name = image.Name,
                Private = image.Private,
                FileName = image.FileName,
            };
        }
        private async Task<ImageData> MapToImageData(DatabaseImage image)
        {
            return new ImageData()
            {
                Data = await File.ReadAllBytesAsync(image.Path),
                FileName = image.FileName
            };
        }
    }
}
