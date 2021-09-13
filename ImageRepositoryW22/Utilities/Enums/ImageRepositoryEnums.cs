using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageRepositoryW22.Utilities.Enums
{
    public static class ImageRepositoryEnums
    {
        public enum ImageCreateStatus
        {
            Success,
            FileTooLarge,
            BadExtension,
            DatabaseError
        }

        public enum ImageBulkCreateStatus
        {
            Success,
            Fail,
            DatabaseError
        }

        public enum ImageDeleteStatus
        {
            Success,
            ImageNotFound,
            DatabaseError
        }

        public enum ImageBulkDeleteStatus
        {
            Success,
            Fail,
            DatabaseError
        }
    }
}
