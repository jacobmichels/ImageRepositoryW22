using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ImageRepositoryW22.Repositories.Models
{
    public class DatabaseImage
    {
        [Key]
        public Guid Id {  get; set; }
        [Required]
        public string Name {  get; set; }
        public string Description {  get; set; }
        [Required]
        public string Path {  get; set; }
        [Required]
        public ApplicationUser Owner {  get; set; }
        public bool Private { get; set; }
    }

    public class RequestImage
    {
        [Required]
        public string Name {  get; set; }
        public string Description { get; set; }
        [Required]
        public bool Private { get; set; }
        [Required]
        public IFormFile File { get; set; }
        [Required]
        public string Extension { get; set; }
    }

    public class ImageUpdate
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public bool Private { get; set; }
    }
}
