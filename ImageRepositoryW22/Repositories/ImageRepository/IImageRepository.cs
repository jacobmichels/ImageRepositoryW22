using ImageRepositoryW22.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageRepositoryW22.ImageRepository.Repositories
{
    public interface IImageRepository
    {
        public Task<List<ImageInfo>> GetAllPublic();
        public Task<List<ImageInfo>> GetMine(ApplicationUser user);
        public Task<ImageData> Get(ApplicationUser user, Guid id);

        public Task<bool> Create(ApplicationUser user, RequestImage image);
        public Task<bool> Create(ApplicationUser user, List<RequestImage> images);

        public Task<ImageInfo> Update(ApplicationUser user, ImageInfo image);

        public Task<bool> Delete(ApplicationUser user, Guid id);
        public Task<int> Delete(ApplicationUser user, List<Guid> ids);

    }
}
