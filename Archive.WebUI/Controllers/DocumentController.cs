﻿using System.Threading.Tasks;
using Archive.Application.Feature.Document.Draw.Commands.CreateDraw;
using Archive.Application.Feature.Document.Draw.Commands.UpdateDraw;
using Archive.Application.Feature.Document.Queries.GetAllDocuments;
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
        /// <param name="id">Id подразделения</param>
        /// <returns></returns>
        [Authorize(Roles = "Архивариус")]
        [HttpPut("update-drawing/{id}")]
        public async Task<IActionResult> UpdateDraw(UpdateDrawCommand command, string id)
        {
            if (id != command.Id)
                return BadRequest("Ошибка обновления");

            return Ok(await Mediator.Send(command));
        }
    }
}