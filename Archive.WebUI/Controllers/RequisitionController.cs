﻿using System.Threading.Tasks;
using Archive.Application.Feature.Requisition.Commands.CreateRequisition;
using Archive.Application.Feature.Requisition.Commands.DeleteRequisition;
using Archive.Application.Feature.Requisition.Commands.UpdateRequisition;
using Archive.Application.Feature.Requisition.Queries.GetRequisitions;
using Microsoft.AspNetCore.Mvc;

namespace Archive.WebUI.Controllers
{
    public class RequisitionController : ApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAllRequisition()
        {
            return Ok(await Mediator.Send(new GetRequisitionsQuery()));
        }

        [HttpPost]
        public async Task<IActionResult> CreateRequisition(CreateRequisitionCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRequisition(UpdateRequisitionCommand command, string id)
        {
            if (id != command.Id)
                return BadRequest("Ошибка обновления");

            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequisition(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest("Не указан идентификатор");
            
            return Ok(await Mediator.Send(new DeleteRequisitionCommand {Id = id}));
        }
    }
}