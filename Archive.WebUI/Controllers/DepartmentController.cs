using System.Threading.Tasks;
using Archive.Application.Feature.Department.Commands.CreateDepartment;
using Archive.Application.Feature.Department.Commands.DeleteDepartment;
using Archive.Application.Feature.Department.Commands.UpdateDepartment;
using Archive.Application.Feature.Department.Queries.GetAllDepartments;
using Microsoft.AspNetCore.Mvc;

namespace Archive.WebUI.Controllers
{
    public class DepartmentController : ApiController
    {
        /// <summary>
        /// Получить все подразделения
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllNomenclatures()
        {
            return Ok(await Mediator.Send(new GetAllDepartmentsQuery()));
        }

        /// <summary>
        /// Добавить новое подразделение
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateNomenclature(CreateDepartmentCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// Изменить подразделение
        /// </summary>
        /// <param name="command">Объект</param>
        /// <param name="id">Id подразделения</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNomenclature(UpdateDepartmentCommand command, string id)
        {
            if (id != command.Id)
                return BadRequest("Ошибка обновления");

            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// Удалить подразделение
        /// </summary>
        /// <param name="id">Id подразделения</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNomenclature(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest("Не указан идентификатор");

            return Ok(await Mediator.Send(new DeleteDepartmentCommand() {Id = id}));
        }
    }
}