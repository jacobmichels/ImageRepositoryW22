using ImageRepositoryW22.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageRepositoryW22.ImageRepository.Repositories
{
    public class ImageRepository : IImageRepository
    {
        public bool Create(ApplicationUser user, RequestImage image)
        {
            throw new NotImplementedException();
        }

        public bool Create(ApplicationUser user, List<RequestImage> images)
        {
            throw new NotImplementedException();
        }

        public bool Delete(ApplicationUser user, Guid id)
        {
            throw new NotImplementedException();
        }

        public int Delete(ApplicationUser user, List<Guid> ids)
        {
            throw new NotImplementedException();
        }

        //Return image data only here.
        public DatabaseImage Get(ApplicationUser user, Guid id)
        {
            throw new NotImplementedException();
        }

        //Don't return image data, just info like name
        public List<DatabaseImage> GetAll()
        {
            throw new NotImplementedException();
        }

        //Don't return image data, just info like name
        public List<DatabaseImage> GetAll(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        public DatabaseImage Update(ApplicationUser user, RequestImage image, Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
