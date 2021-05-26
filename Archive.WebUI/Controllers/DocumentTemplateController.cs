using System.Threading.Tasks;
using Archive.Application.Feature.DocumentTemplate.Commands.CreateDocumentTemplate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Archive.WebUI.Controllers
{
    public class DocumentTemplateController : ApiController
    {
        [AllowAnonymous]
        [HttpPost("create")]
        public async Task<IActionResult> CreateTemplate(CreateDocumentTemplateCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}