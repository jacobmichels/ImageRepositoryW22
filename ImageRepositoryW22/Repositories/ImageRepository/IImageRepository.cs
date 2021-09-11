using ImageRepositoryW22.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageRepositoryW22.ImageRepository.Repositories
{
    public interface IImageRepository
    {
        public Task<List<DatabaseImage>> GetAll();
        public Task<List<DatabaseImage>> GetAll(ApplicationUser user);
        public Task<DatabaseImage> Get(ApplicationUser user, Guid id);

        public Task<bool> Create(ApplicationUser user, RequestImage image);
        public Task<bool> Create(ApplicationUser user, List<RequestImage> images);

        public Task<DatabaseImage> Update(ApplicationUser user, RequestImage image, Guid id);

        public Task<bool> Delete(ApplicationUser user, Guid id);
        public Task<int> Delete(ApplicationUser user, List<Guid> ids);

    }
}
