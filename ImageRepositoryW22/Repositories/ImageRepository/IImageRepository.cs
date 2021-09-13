using ImageRepositoryW22.Repositories.Models;
using ImageRepositoryW22.Utilities.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static ImageRepositoryW22.Utilities.Enums.ImageRepositoryEnums;

namespace ImageRepositoryW22.ImageRepository.Repositories
{
    public interface IImageRepository
    {
        public Task<List<ImageInfo>> GetAllPublic();
        public Task<List<ImageInfo>> GetMine(ApplicationUser user);
        public Task<ImageData> Get(ApplicationUser user, Guid id);

        public Task<ImageCreateStatus> Create(ApplicationUser user, RequestImage image);
        public Task<ImageBulkCreateStatus> Create(ApplicationUser user, List<IFormFile> images);

        public Task<ImageInfo> Update(ApplicationUser user, ImageUpdate image);

        public Task<ImageBulkDeleteStatus> Delete(ApplicationUser user, List<Guid> ids);

        public Task<List<ImageInfo>> SearchMine(ApplicationUser user, string text);
        public Task<List<ImageInfo>> SearchAllPublic(string text);
    }
}
