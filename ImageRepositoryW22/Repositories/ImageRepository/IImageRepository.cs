﻿using ImageRepositoryW22.Repositories.Models;
using ImageRepositoryW22.Utilities.Enums;
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
        public Task<ImageBulkCreateStatus> Create(ApplicationUser user, List<RequestImage> images);

        public Task<ImageInfo> Update(ApplicationUser user, ImageInfo image);

        public Task<ImageBulkDeleteStatus> Delete(ApplicationUser user, List<Guid> ids);

    }
}
