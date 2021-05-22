using System.Threading.Tasks;
using Archive.Application.Feature.Nomenclature.Commands.CreateNomenclature;
using Archive.Application.Feature.Nomenclature.Commands.DeleteNomenclature;
using Archive.Application.Feature.Nomenclature.Commands.UpdateNoomenclature;
using Archive.Application.Feature.Nomenclature.Queries.GetAllNomenclatures;
using Archive.Application.Feature.Nomenclature.Queries.GetNomenclatureName;
using Microsoft.AspNetCore.Mvc;

namespace Archive.WebUI.Controllers
{
    public class NomenclatureController : ApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAllNomenclatures()
        {
            return Ok(await Mediator.Send(new GetAllNomenclaturesQuery()));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNomenclatureName(string id)
        {
            return Ok(await Mediator.Send(new GetNomenclatureNameQuery {Id = id}));
        }

        [HttpPost]
        public async Task<IActionResult> CreateNomenclature(CreateNomenclatureCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNomenclature(UpdateNomenclatureCommand command, string id)
        {
            if (id != command.Id)
                return BadRequest("Ошибка обновления");

            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNomenclature(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest("Не указан идентификатор");

            return Ok(await Mediator.Send(new DeleteNomenclatureCommand {Id = id}));
        }
    }
}