using ImageRepositoryW22.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageRepositoryW22.ImageRepository.Repositories
{
    public interface IImageRepository
    {
        public List<DatabaseImage> GetAll();
        public List<DatabaseImage> GetAll(ApplicationUser user);
        public DatabaseImage Get(ApplicationUser user, Guid id);

        public bool Create(ApplicationUser user, RequestImage image);
        public bool Create(ApplicationUser user, List<RequestImage> images);

        public DatabaseImage Update(ApplicationUser user, RequestImage image, Guid id);

        public bool Delete(ApplicationUser user, Guid id);
        public int Delete(ApplicationUser user, List<Guid> ids);

    }
}
