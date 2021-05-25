using System.Threading.Tasks;
using Archive.Application.Feature.Role.Queries.GetAllRoles;
using Microsoft.AspNetCore.Mvc;

namespace Archive.WebUI.Controllers
{
    public class RoleController : ApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            return Ok(await Mediator.Send(new GetAllRolesQuery()));
        }
    }
}