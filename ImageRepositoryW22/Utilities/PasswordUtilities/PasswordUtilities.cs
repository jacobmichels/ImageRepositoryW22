using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Konscious.Security.Cryptography;

namespace ImageRepositoryW22.Utilities.PasswordUtilities
{
    public class PasswordUtilities : IPasswordUtilities
    {
        public async Task<string> HashPasswordAsync(string password)
        {
            var argon2 = new Argon2d(Encoding.UTF8.GetBytes(password))
            {
                DegreeOfParallelism = 32,
                MemorySize = 16392,
                Iterations = 48,
                Salt = Encoding.UTF8.GetBytes("caa4e318-3b48-4509-bb41-00aaa13e05d4")
            };

            return Encoding.UTF8.GetString(await argon2.GetBytesAsync(30));
        }

        public async Task<bool> VerifyPasswordAysnc(string password, string storedHash)
        {
            var argon2 = new Argon2d(Encoding.UTF8.GetBytes(password))
            {
                DegreeOfParallelism = 32,
                MemorySize = 16392,
                Iterations = 48,
                Salt = Encoding.UTF8.GetBytes("caa4e318-3b48-4509-bb41-00aaa13e05d4")
            };

            var passwordHash = Encoding.UTF8.GetString(await argon2.GetBytesAsync(30));

            return passwordHash == storedHash;
        }
    }
}
