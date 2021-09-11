using ImageRepositoryW22.ImageRepository.Repositories;
using ImageRepositoryW22.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ImageRepositoryW22.Utilities.Enums.ImageRepositoryEnums;

namespace ImageRepositoryW22.Repositories.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IImageRepository _imageRepository;
        private readonly ILogger<UserRepository> _logger;
        public UserRepository(ApplicationDbContext db, ILogger<UserRepository> logger, IImageRepository imageRepository)
        {
            _db = db;
            _logger = logger;
            _imageRepository = imageRepository;
        }

        public async Task<bool> InsertUser(ApplicationUser user)
        {
            await _db.Users.AddAsync(user);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception occured while trying to save insertion to db: {ex.Message}");
                return false;
            }
            return true;
        }

        public async Task<bool> DeleteUser(string username)
        {
            var user = await GetUser(username);

            //Remove all of the users image, then remove the user.
            var images = await _imageRepository.GetMine(user);

            if(images is not null && images.Count > 0)
            {
                var idsToDelete = new List<Guid>();
                foreach (var image in images)
                {
                    idsToDelete.Add(image.Id);
                }

                var deleteStatus = await _imageRepository.Delete(user, idsToDelete);
                if (deleteStatus == ImageBulkDeleteStatus.AllFail || deleteStatus == ImageBulkDeleteStatus.AtLeastOneFail || deleteStatus == ImageBulkDeleteStatus.DatabaseError)
                {
                    return false;
                }
            }
            System.IO.Directory.Delete(System.IO.Path.Join("UserImages",$"{user.Id}"));
            _db.Users.Remove(user);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError($"Exception occured while trying to save deletion to db: {ex.Message}");
                return false;
            }
            return true;
        }

        public async Task<ApplicationUser> GetUser(string username)
        {
            var user = await _db.Users.FirstOrDefaultAsync(user => user.UserName == username);
            return user;
        }

        public async Task<bool> UserExists(string username)
        {
            var user = await _db.Users.FirstOrDefaultAsync(user => user.UserName == username);
            return user != null;
        }
    }
}
