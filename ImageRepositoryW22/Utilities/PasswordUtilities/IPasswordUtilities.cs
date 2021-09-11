using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageRepositoryW22.Utilities.PasswordUtilities
{
    public interface IPasswordUtilities
    {
        public Task<string> HashPasswordAsync(string password);

        public Task<bool> VerifyPasswordAysnc(string password, string storedHash);
    }
}
