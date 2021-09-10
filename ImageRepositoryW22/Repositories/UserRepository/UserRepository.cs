using ImageRepositoryW22.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageRepositoryW22.Repositories.UserRepository
{
    public class UserRepository : IUserRepository
    {
        public Task<ApplicationUser> CreateUser(string username, string password)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteUser(string username)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> GetUser(string username)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UserExists(string username)
        {
            throw new NotImplementedException();
        }
    }
}
