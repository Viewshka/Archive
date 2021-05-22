﻿using System.Threading.Tasks;
using Archive.Application.Feature.User.Queries.GetAllUsers;
using Archive.Application.Feature.User.Queries.GetCurrentUser;
using Archive.Application.Feature.User.Queries.GetUsersForAutoComplete;
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
        
        [HttpGet("users")]
        public async Task<IActionResult> GetUsersForAutoComplete()
        {
            return Ok(await Mediator.Send(new GetUsersForAutocompleteQuery()));
        }
        
        [Authorize(Roles = "Архивариус")]
        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await Mediator.Send(new GetAllUsersQuery()));
        }
    }
}