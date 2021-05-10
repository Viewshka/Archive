using System.Threading.Tasks;
using Archive.Application.Feature.Document.Draw.Commands.CreateDraw;
using Archive.Application.Feature.Document.Draw.Commands.UpdateDraw;
using Archive.Application.Feature.Document.KitConstructDoc.Commands.CreateKitConstructDoc;
using Archive.Application.Feature.Document.KitConstructDoc.Commands.UpdateKitCreateConstructDoc;
using Archive.Application.Feature.Document.Queries.GetAllDocuments;
using Archive.Application.Feature.Document.Queries.GetDocumentHistory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Archive.WebUI.Controllers
{
    
    public class DocumentController : ApiController
    {
        /// <summary>
        /// Получить все документы
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllDocuments()
        {
            return Ok(await Mediator.Send(new GetAllDocumentsQuery()));
        }

        /// <summary>
        /// Добавить новый документ (чертеж)
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Authorize(Roles = "Архивариус")]
        [HttpPost("create-drawing")]
        public async Task<IActionResult> CreateDraw(CreateDrawingCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// Изменить документ (Чертеж)
        /// </summary>
        /// <param name="command">Объект</param>
        /// <param name="id">Id документа</param>
        /// <returns></returns>
        [Authorize(Roles = "Архивариус")]
        [HttpPut("update-drawing/{id}")]
        public async Task<IActionResult> UpdateDraw(UpdateDrawCommand command, string id)
        {
            if (id != command.Id)
                return BadRequest("Ошибка обновления");

            return Ok(await Mediator.Send(command));
        }
        
        /// <summary>
        /// Добавить новый документ (Комплект КД)
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Authorize(Roles = "Архивариус")]
        [HttpPost("create-kit-construct-documents")]
        public async Task<IActionResult> CreateKitConstructDoc(CreateKitConstructDocCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// Изменить документ (Комплект КД)
        /// </summary>
        /// <param name="command">Объект</param>
        /// <param name="id">Id документа</param>
        /// <returns></returns>
        [Authorize(Roles = "Архивариус")]
        [HttpPut("update-kit-construct-documents/{id}")]
        public async Task<IActionResult> UpdateKitConstructDoc(UpdateKitConstructDocCommand command, string id)
        {
            if (id != command.Id)
                return BadRequest("Ошибка обновления");

            return Ok(await Mediator.Send(command));
        }
        
        /// <summary>
        /// Получить историю использования документа
        /// </summary>
        /// <returns></returns>
        [HttpGet("{documentId}/history")]
        public async Task<IActionResult> GetDocumentHistory(string documentId)
        {
            return Ok(await Mediator.Send(new GetDocumentHistoryQuery {DocumentId = documentId}));
        }
    }
}