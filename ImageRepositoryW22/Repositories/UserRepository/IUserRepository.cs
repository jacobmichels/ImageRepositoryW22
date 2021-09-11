using ImageRepositoryW22.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageRepositoryW22.Repositories.UserRepository
{
    public interface IUserRepository
    {
        public Task<ApplicationUser> GetUser(string username);
        public Task<bool> UserExists(string username);
        public Task<bool> InsertUser(ApplicationUser user);
        public Task<bool> DeleteUser(string username);
    }
}
