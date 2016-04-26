using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Web.Http;
using TicketDesk.Domain;
using TicketDesk.Localization.Controllers;
using TicketDesk.Web.Client.Models;
using TicketDesk.Web.Identity;
using System.Web.Http.Cors;

namespace TicketDesk.Web.Client.Controllers
{
    public class ApiAccountController : ApiController
    {
        private TicketDeskUserManager UserManager { get; set; }
        private TicketDeskSignInManager SignInManager { get; set; }
        private TdDomainContext DomainContext { get; set; }

        public ApiAccountController(
            TicketDeskUserManager userManager,
            TicketDeskSignInManager signInManager,
            TdDomainContext domainContext)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            DomainContext = domainContext;
        }

        [AllowAnonymous]
        //[Route("Login")]
        public async Task<IHttpActionResult> Login(UserSignInViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true);
            switch (result)
            {
                case SignInStatus.Success:
                    return Ok();
                case SignInStatus.LockedOut:
                    return BadRequest("Lockout");
                default:
                    ModelState.AddModelError("", Strings.InvalidLoginAttempt);
                    return BadRequest();
            }

        }
    }
}