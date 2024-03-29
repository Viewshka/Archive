﻿using System.Threading.Tasks;
using Archive.Application.Feature.Document.DestructionAkt.GenerateDestructionAkt;
using Archive.Application.Feature.Document.DocumentUsageList.GenerateDocumentUsageList;
using Archive.Application.Feature.Document.Draw.Commands.CreateDraw;
using Archive.Application.Feature.Document.Draw.Commands.UpdateDraw;
using Archive.Application.Feature.Document.KitConstructDoc.Commands.CreateKitConstructDoc;
using Archive.Application.Feature.Document.KitConstructDoc.Commands.UpdateKitCreateConstructDoc;
using Archive.Application.Feature.Document.Queries.GetAkts;
using Archive.Application.Feature.Document.Queries.GetAllDocuments;
using Archive.Application.Feature.Document.Queries.GetDocumentHistory;
using Archive.Application.Feature.Document.Queries.GetDocumentsByNomenclature;
using Archive.Application.Feature.Document.Queries.GetEmployeeDocuments;
using Archive.Application.Feature.Document.Queries.GetInventory;
using Archive.Application.Feature.Document.ReturnDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

        [HttpGet("employee")]
        public async Task<IActionResult> GetDocuments()
        {
            return Ok(await Mediator.Send(new GetEmployeeDocumentsQuery()));
        }
        
        [HttpGet("by-nomenclature-{nomenclatureId}")]
        public async Task<IActionResult> GetDocumentsByNomenclature(string nomenclatureId)
        {
            return Ok(await Mediator.Send(new GetDocumentsByNomenclatureQuery {NomenclatureId = nomenclatureId}));
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

        /// <summary>
        /// Вернуть документ
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}/return")]
        public async Task<IActionResult> ReturnDocument(string id)
        {
            return Ok(await Mediator.Send(new ReturnDocumentCommand {Id = id}));
        }

        [HttpGet("{documentId}/generate")]
        public async Task<IActionResult> GenerateDocument(string documentId)
        {
            var stream = await Mediator.Send(new GenerateDocumentUsageListCommand {DocumentId = documentId});

            Response.Headers.Append("content-disposition", "inline; filename=file.pdf");
            var result = File(stream, "application/pdf");
            return result;
        }

        [HttpGet("akt")]
        public async Task<IActionResult> GetAkts()
        {
            return Ok(await Mediator.Send(new GetAktsQuery()));
        }

        [HttpGet("inventory")]
        public async Task<IActionResult> GetInventories()
        {
            return Ok(await Mediator.Send(new GetInventoryQuery()));
        }

        [HttpPost("destruction-akt")]
        public async Task<IActionResult> GenerateInventory(GenerateDestructionAktCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}