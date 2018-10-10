using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edison.Api.Helpers;
using Edison.Core.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Edison.Api.Controllers
{
    [Authorize(AuthenticationSchemes = "Backend,B2CWeb")]
    [Route("api/Users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public UsersController()
        {
        }
    }
}
