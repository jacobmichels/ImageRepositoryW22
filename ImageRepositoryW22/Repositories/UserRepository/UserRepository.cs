using ImageRepositoryW22.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageRepositoryW22.Repositories.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<UserRepository> _logger;
        public UserRepository(ApplicationDbContext db, ILogger<UserRepository> logger)
        {
            _db = db;
            _logger = logger;
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
