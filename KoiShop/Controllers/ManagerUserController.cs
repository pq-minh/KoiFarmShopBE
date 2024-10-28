﻿using KoiShop.Application.Command.DeleteUser;
using KoiShop.Application.Command.UpdatePointUser;
using KoiShop.Application.Queries.GetAllUser;
using KoiShop.Application.Queries.GetQuotation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KoiShop.Controllers
{
    [ApiController]
    [Route("api/manageruser")]
    public class ManagerUserController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public  async Task<IActionResult> GetAllUser(int pageNumber = 1, int pageSize = 10)
        {
            var query = new GetAllUserQuery(pageNumber, pageSize);
            var result = await mediator.Send(query);
            return Ok(result);
        }

        [HttpPost("updateuser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdatePointUserCommand updatePointUserCommand)
        {
            var result = await mediator.Send(updatePointUserCommand);
            return Ok(result);
        }

        [HttpPost("deleteuser")]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUserCommand deleteUserCommand)
        {
            var result = await mediator.Send(deleteUserCommand);
            return Ok(result);
        }

    }
   
}