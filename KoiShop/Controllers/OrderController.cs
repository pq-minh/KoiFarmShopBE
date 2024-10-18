﻿using KoiShop.Application.Dtos;
using KoiShop.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KoiShop.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        //[HttpGet]
        //public async Task<IActionResult> GetOrderDetail()
        //{
        //    var result = await _orderService.GetOrderDetail();
        //    return Ok(result);
        //}
        [HttpGet]
        public async Task<IActionResult> GetOrder()
        {
            var result = await _orderService.GetOrder();
            return Ok(result);
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var result = await _orderService.GetOrderDetailById(id);
            return Ok(result);
        }
        [HttpPost]
        public async Task<IActionResult> AddOrder([FromBody] OrderDtoV1 orderDto)
        {
            var carts = orderDto.Carts;
            var method = orderDto.Method;
            var discountId = orderDto.DiscountId;
            var phoneNumber = orderDto.PhoneNumber;
            var address = orderDto.Address;
            var result = await _orderService.AddOrders(carts, method, discountId, phoneNumber, address);
            switch (result)
            {
                case OrderEnum.Success:
                    return Created();
                case OrderEnum.Fail:
                    return BadRequest("Have bug in progrecing");
                case OrderEnum.FailAdd:
                    return BadRequest("Add have bug!!");
                case OrderEnum.FailUpdateCart:
                    return BadRequest("Update cart have bug");
                case OrderEnum.FailUpdateFish:
                    return BadRequest("Fail update fish");
                case OrderEnum.FailAddPayment:
                    return BadRequest("Add payment have bug");
                case OrderEnum.NotLoggedInYet:
                    return Unauthorized("User is not login");
                case OrderEnum.InvalidParameters:
                    return BadRequest("Miss a important parameter");
                case OrderEnum.InvalidTypeParameters:
                    return BadRequest("PhoneNumBer or address is not authenticated");
                case OrderEnum.UserNotAuthenticated:
                    return Unauthorized("User is not authenticated.");
                default:
                    return BadRequest("Unexpected error!");
            }
        }
    }
}
