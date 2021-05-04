using System.Threading.Tasks;
using Archive.Application.Feature.User.Quries.GetCurrentUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Archive.WebUI.Controllers
{
    public class UserController : ApiController
    {
        [AllowAnonymous]
        [HttpGet("current-user")]
        public async Task<IActionResult> GetCurrentUser()
        {
            return Ok(await Mediator.Send(new GetCurrentUserQuery()));
        }
    }
}