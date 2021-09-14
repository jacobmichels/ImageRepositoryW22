using ImageRepositoryW22.ImageRepository.Repositories;
using ImageRepositoryW22.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using static ImageRepositoryW22.Utilities.Enums.ImageRepositoryEnums;

namespace ImageRepositoryW22.Repositories.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IImageRepository _imageRepository;
        private readonly ILogger<UserRepository> _logger;
        
        //Get required services from dependency injection.
        public UserRepository(ApplicationDbContext db, ILogger<UserRepository> logger, IImageRepository imageRepository)
        {
            _db = db;
            _logger = logger;
            _imageRepository = imageRepository;
        }

        //Input a user.
        //
        //Add the user to the database. Checks to ensure this is not a duplicate user have already been executed by the controller.
        //
        //Return a boolean representing the result of the operation.
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

        //Input a guid.
        //
        //Find the user referenced by the input guid. The input guid is already validated by the Authorize method on the calling controller.
        //Remove all the images the user owns from the database and the filesystem. Then remove the user from the database.
        //
        //Return a boolean representing the result of the operation.
        public async Task<bool> DeleteUser(Guid id)
        {
            var user = await GetUser(id);

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
                if (deleteStatus == ImageBulkDeleteStatus.Fail || deleteStatus == ImageBulkDeleteStatus.DatabaseError)
                {
                    return false;
                }
            }
            if(Directory.Exists(Path.Join("UserImages", $"{user.Id}"))){
                Directory.Delete(Path.Join("UserImages", $"{user.Id}"));
            }
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

        //Input a username.
        //
        //Find the user referenced by the input username. This method is only used by the login endpoint.
        //
        //Return the user if found, otherwise return null.
        public async Task<ApplicationUser> GetUser(string username)
        {
            var user = await _db.Users.FirstOrDefaultAsync(user => user.UserName == username);
            return user;
        }

        //Input a guid.
        //
        //Find the user referenced by the input guid.
        //
        //Return the user if found, otherwise return null.
        public async Task<ApplicationUser> GetUser(Guid id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(user => user.Id == id);
            return user;
        }

        //Input a username.
        //
        //Find the user referenced by the input username.
        //This method is used by the register endpoint to make sure two users cannot have the same username.
        //
        //Return true if a user is found, otherwise return false.
        public async Task<bool> UserExists(string username)
        {
            var user = await _db.Users.FirstOrDefaultAsync(user => user.UserName == username);
            return user != null;
        }

        //Input a guid.
        //
        //Find the user referenced by the input guid.
        //This method is used by the authorization service to validate JWTs.
        //
        //Return true if a user is found, otherwise return false.
        public async Task<bool> UserExists(Guid id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(user => user.Id == id);
            return user != null;
        }
    }
}
