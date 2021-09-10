using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ImageRepositoryW22.Repositories.Models
{
    public class Image
    {
        [Key]
        public Guid Id {  get; set; }
        [Required]
        public string Name {  get; set; }
        [Required]
        public string Description {  get; set; }
        [Required]
        public string Path {  get; set; }
        [Required]
        public ApplicationUser Owner {  get; set; }
        public bool Private { get; set; }
    }
}
