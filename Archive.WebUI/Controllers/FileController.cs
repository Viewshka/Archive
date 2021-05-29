using System.Linq;
using System.Threading.Tasks;
using Archive.Application.Feature.File.Commands.FileUpload;
using Archive.Application.Feature.File.Queries;
using Archive.Application.Feature.Requisition.Queries.GetRequisitionPreview;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Archive.WebUI.Controllers
{
    public class FileController : ApiController
    {
        private readonly IWebHostEnvironment _environment;

        public FileController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpPost("upload/{documentId}")]
        public async Task<IActionResult> UploadFile(string documentId)
        {
            if (string.IsNullOrWhiteSpace(documentId) && !Request.Form.Files.Any())
                return NoContent();

            var file = Request.Form.Files.FirstOrDefault();

            if (file != null)
                await Mediator.Send(new FileUploadCommand
                {
                    DocumentId = documentId,
                    WebRootPath = _environment.WebRootPath,
                    File = file
                });

            return Ok();
        }

        [HttpGet("{documentId}")]
        public async Task<IActionResult> Preview(string documentId)
        {
            var memoryStream = await Mediator.Send(new GetPreviewQuery
                {Id = documentId, WebRootPath = _environment.WebRootPath});

            Response.Headers.Append("content-disposition", "inline; filename=file.pdf");
            var result = File(memoryStream, "application/pdf");
            return result;
        }
        
        [HttpGet("requisition/{requisitionId}")]
        public async Task<IActionResult> PreviewRequisition(string requisitionId)
        {
            var memoryStream = await Mediator.Send(new GetRequisitionPreviewQuery
                {Id = requisitionId, WebRootPath = _environment.WebRootPath});

            Response.Headers.Append("content-disposition", "inline; filename=file.pdf");
            var result = File(memoryStream, "application/pdf");
            return result;
        }
    }
}