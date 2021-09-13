using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageRepositoryW22.Utilities.ControllerUtility
{
    public class ControllerUtility : IControllerUtility
    {
        public Guid GetUserId(HttpContext context)
        {
            var id = context.User.Claims.FirstOrDefault(claim => claim.Type == "id");
            if (id is null)
            {
                return Guid.Empty;
            }
            return Guid.Parse(id.Value);
        }
    }
}
