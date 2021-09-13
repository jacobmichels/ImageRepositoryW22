using Microsoft.AspNetCore.Http;
using System;

namespace ImageRepositoryW22.Utilities.ControllerUtility
{
    public interface IControllerUtility
    {
        public Guid GetUserId(HttpContext context);
    }
}