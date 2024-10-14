﻿using KoiShop.Domain.Entities;
using KoiShop.Domain.Respositories;
using KoiShop.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KoiShop.Application.Users.UserContext;

namespace KoiShop.Infrastructure.Respositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly KoiShopV1DbContext _koiShopV1DbContext;
        private readonly IUserStore<User> _userStore;
        private readonly IUserContext _userContext;
        public OrderRepository(KoiShopV1DbContext koiShopV1DbContext, IUserStore<User> userStore, IUserContext userContext)
        {
            _koiShopV1DbContext = koiShopV1DbContext;
            _userStore = userStore;
            _userContext = userContext;
        }

        public async Task<IEnumerable<OrderDetail>> GetOrderDetail()
        {
            var userId = _userContext.GetCurrentUser().Id;
            var orders = await _koiShopV1DbContext.Orders.Where(o => o.UserId == userId).ToListAsync();
            if (orders == null)
            {
                return Enumerable.Empty<OrderDetail>();
            }
            var orderdetail = await _koiShopV1DbContext.OrderDetails.Where(od => orders.Select(o => o.OrderId).Contains((int)od.OrderId)).ToListAsync();
            return orderdetail;
        }
        public async Task<bool> AddToOrderDetailFromCart(List<CartItem> carts)
        {
            var user = _userContext.GetCurrentUser();
            var order = await _koiShopV1DbContext.Orders.Where(o => o.UserId == user.Id).OrderByDescending(o => o.CreateDate).FirstOrDefaultAsync();
            if (order == null)
            {
                return false;
            }
            var shoppingcart = await _koiShopV1DbContext.ShoppingCarts.Where(sc => sc.UserId == user.Id).FirstOrDefaultAsync();
            if (shoppingcart == null)
            {
                return false;
            }
            var exsitCart = await _koiShopV1DbContext.CartItems.Where(c => c.ShoppingCartId == shoppingcart.ShoppingCartID).
                ToListAsync();
            if (exsitCart == null)
            {
                return false;
            }
            foreach (var cart in carts)
            {
                if ((cart.KoiId.HasValue && !cart.BatchKoiId.HasValue) || (!cart.KoiId.HasValue && cart.BatchKoiId.HasValue))
                {
                    var orderDetail = new OrderDetail
                    {
                        KoiId = cart.KoiId,
                        BatchKoiId = cart.BatchKoiId,
                        ToTalQuantity = cart.Quantity,
                        OrderId = order.OrderId
                    };
                    _koiShopV1DbContext.OrderDetails.Add(orderDetail);
                }
            }
            await _koiShopV1DbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> AddToOrderDetailFromShop(CartItem cart, int orderId)
        {
            var user = _userContext.GetCurrentUser();
            if ((cart.KoiId.HasValue && !cart.BatchKoiId.HasValue) || (!cart.KoiId.HasValue && cart.BatchKoiId.HasValue))
            {
                var orderDetail = new OrderDetail
                {
                    KoiId = cart.KoiId,
                    BatchKoiId = cart.BatchKoiId,
                    ToTalQuantity = cart.Quantity,
                    OrderId = orderId
                };
                _koiShopV1DbContext.Add(orderDetail);
                await _koiShopV1DbContext.SaveChangesAsync();
                return true;
            }
            else
                return false;
        }
        public async Task<bool> AddToOrder(List<CartItem> carts)
        {
            var user = _userContext.GetCurrentUser();
            var count = carts.Count();
            int quantity = 0;
            float totalPrice = 0;
            float totalAmount = 0;
            var order = new Order
            {
                TotalAmount = 0,
                CreateDate = DateTime.Now,
                OrderStatus = "Pending",
                UserId = user.Id
            };
            _koiShopV1DbContext.Orders.Add(order);
            await _koiShopV1DbContext.SaveChangesAsync();
            foreach (var cart in carts)
            {
                var cartItem = await _koiShopV1DbContext.CartItems.FirstOrDefaultAsync(ci =>
                (cart.KoiId.HasValue && ci.KoiId == cart.KoiId) ||
                (cart.BatchKoiId.HasValue && ci.BatchKoiId == cart.BatchKoiId));
                if (cartItem != null)
                {
                    quantity = (int)cart.Quantity;
                    totalPrice = quantity * (float)cartItem.UnitPrice;
                    totalAmount += totalPrice;
                }
            }
            order.TotalAmount = totalAmount;
            _koiShopV1DbContext.Orders.Update(order);
            await _koiShopV1DbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> UpdateCartAfterBuy(List<CartItem> carts)
        {
            var user = _userContext.GetCurrentUser();
            var order = await _koiShopV1DbContext.Orders.Where(o => o.UserId == user.Id).OrderByDescending(o => o.CreateDate).FirstOrDefaultAsync();
            if (order == null)
            {
                return false;
            }
            var shoppingcart = await _koiShopV1DbContext.ShoppingCarts.Where(sc => sc.UserId == user.Id).FirstOrDefaultAsync();
            if (shoppingcart == null)
            {
                return false;
            }
            var existCart = await _koiShopV1DbContext.CartItems.Where(c => c.ShoppingCartId == shoppingcart.ShoppingCartID).
                ToListAsync();
            if (existCart == null)
            {
                return false;
            }
            var existOrderDetail = await _koiShopV1DbContext.OrderDetails.Where(od => od.OrderId == order.OrderId).ToListAsync();
            if (existOrderDetail == null)
            {
                return false;
            }
            foreach (var cartItem in existCart)
            {
                var match = existOrderDetail.FirstOrDefault(eod =>
           (cartItem.KoiId.HasValue && eod.KoiId == cartItem.KoiId) ||
           (cartItem.BatchKoiId.HasValue && eod.BatchKoiId == cartItem.BatchKoiId));

                if (match != null)
                {
                    cartItem.Status = "Bought";
                    _koiShopV1DbContext.CartItems.Update(cartItem);
                }
            }
            await _koiShopV1DbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddPayment(string method)
        {
            var userId = _userContext.GetCurrentUser();
            var order = await _koiShopV1DbContext.Orders.Where(od => od.UserId == userId.Id).OrderByDescending(od => od.CreateDate).FirstOrDefaultAsync();
            if (order == null)
            {
                return false;
            }
            string status;
            if (string.Equals(method, "online", StringComparison.OrdinalIgnoreCase))
            {
                status = "Complete";
            }
            else if (string.Equals(method, "offline", StringComparison.OrdinalIgnoreCase))
            {
                status = "Pending";
            }
            else
            {
                return false;
            }
            var payment = new Payment
            {
                CreateDate = DateTime.Now,
                PaymenMethod = method,
                Status = status,
                OrderId = order.OrderId
            };
            _koiShopV1DbContext.Payments.Add(payment);
            await _koiShopV1DbContext.SaveChangesAsync();
            return true;
        }
    }
}
