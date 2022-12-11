using BookStore.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Route("Administration/[controller]/[Action]/{id?}")]
    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    public class BaseController : Controller
    {
        protected string GetCurrentUserId()
        {
            return this.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
