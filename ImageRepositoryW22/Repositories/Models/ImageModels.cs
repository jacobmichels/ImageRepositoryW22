using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
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
        [Required]
        public string FileName {  get; set; }
        public string Description {  get; set; }
        [Required]
        public string Path {  get; set; }
        [Required]
        public ApplicationUser Owner {  get; set; }
        public bool Private { get; set; }
        [Required]
        public string ImageText {  get; set; }
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
    }

    public class ImageInfo
    {
        public Guid Id {  get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Private { get; set; }
        public string FileName { get; set; }
        public string ImageText { get; set; }
    }

    public class ImageUpdate
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        public bool Private { get; set; }
    }

    public class ImageData
    {
        public byte[] Data { get; set; }
        public string FileName {  get; set; }
    }
}
