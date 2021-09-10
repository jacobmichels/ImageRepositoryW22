using ImageRepositoryW22.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageRepositoryW22.ImageRepository.Repositories
{
    public interface IImageRepository
    {
        public List<Image> GetAll();
        public List<Image> GetAll(ApplicationUser user);
        public Image Get(ApplicationUser user, Guid id);

        public bool Create(ApplicationUser user, Image image);
        public bool Create(ApplicationUser user, List<Image> images);

        public Image Update(ApplicationUser user, Image image);

        public bool Delete(ApplicationUser user, Guid id);
        public int Delete(ApplicationUser user, List<Guid> ids);

    }
}
