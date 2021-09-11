﻿using ImageRepositoryW22.Repositories;
using ImageRepositoryW22.Repositories.Models;
using Microsoft.EntityFrameworkCore;
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
            _logger = logger;
        }
        //TODO: Make sure users cannot upload two files with the same name (or not, decide if this is important).
        public async Task<bool> Create(ApplicationUser user, RequestImage image)
        {
            //Make sure users can't upload large files (> 100MB)
            if (image.File.Length > 100000000)
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
            foreach (var image in images)
            {
                //Make sure users can't upload large files (> 100MB)
                if (image.File.Length > 100000000)
                {
                    images.Remove(image);
                    _logger.LogWarning($"User: {user.UserName} Tried to upload file with size {image.File.Length}");
                    continue;
                }
                //Make sure the file extension is an image. This is a repository for images only.
                var fileExtension = Path.GetExtension(image.File.FileName);
                if (!IsValidFileExtension(fileExtension))
                {
                    _logger.LogWarning($"User: {user.UserName} Tried to upload file with invalid extension: {fileExtension}");
                    images.Remove(image);
                    continue;
                }

                var databaseImage = BuildDatabaseImage(user, image);
                Directory.CreateDirectory(Path.Join("UserImages", $"{user.Id}"));
                using (var stream = File.Create(databaseImage.Path))
                {
                    await image.File.CopyToAsync(stream);
                }
                await _db.Images.AddAsync(databaseImage);
            }

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occured while trying to save bulk image insertion to db: {ex.Message}");
                return false;
            }

            return true;

        }

        public async Task<bool> Delete(ApplicationUser user, Guid id)
        {
            var image = await _db.Images.FirstOrDefaultAsync(image => image.Id==id && image.Owner.UserName==user.UserName);
            if(image is null)
            {
                return false;
            }
            _db.Images.Remove(image);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occured while trying to save image deletion to db: {ex.Message}");
                return false;
            }
            return true;

        }

        public async Task<int> Delete(ApplicationUser user, List<Guid> ids)
        {
            var images = await _db.Images.Where(image => ids.Contains(image.Id) && image.Owner.UserName == user.UserName).ToListAsync();
            if(images is null || images.Count == 0)
            {
                return 0;
            }
            int count = 0;
            foreach(var image in images)
            {
                count++;
                _db.Remove(image);
            }

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occured while trying to save bulk image deletion to db: {ex.Message}");
                return -1;
            }

            return count;
        }

        //Return image data only here. Check if user is null, and make sure the image is either public or the user owns the image
        public async Task<ImageData> Get(ApplicationUser user, Guid id)
        {
            var image = await _db.Images.FirstOrDefaultAsync(image => image.Id == id);
            if (image == null)
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

        public async Task<ImageInfo> Update(ApplicationUser user, ImageInfo newData)
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
            };
            //For security, use user.Id and databaseImage.Id for the path so we don't create a path with untrusted user input.
            databaseImage.Path = Path.Join("UserImages", $"{user.Id}", $"{databaseImage.Id}{Path.GetExtension(image.File.FileName)}");
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
                Private = image.Private
            };
        }
        private async Task<ImageData> MapToImageData(DatabaseImage image)
        {
            return new ImageData()
            {
                Info = MapToImageInfo(image),
                Data = await File.ReadAllBytesAsync(image.Path)
            };
        }
    }
}
