using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImageRepositoryW22.Repository.Models
{
    public class ApplicationUser
    {
        [Key]
        public Guid Id {  get; set; }
        [Required]
        public string UserName {  get; set; }
        [Required]
        public string PasswordHash {  get; set; }

        public ApplicationUser(string username, string passwordhash)
        {
            UserName = username;
            PasswordHash = passwordhash;
        }

        public ApplicationUser()
        {
        }
    }
}
