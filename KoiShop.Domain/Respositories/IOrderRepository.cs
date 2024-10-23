﻿using KoiShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShop.Domain.Respositories
{
    public interface IOrderRepository
    {
        Task<IEnumerable<OrderDetail>> GetOrderDetail();
        Task<IEnumerable<Order>> GetOrder();
        Task<IEnumerable<OrderDetail>> GetOrderDetailById(int orderId);
        Task<bool> AddToOrder(List<CartItem> carts, int? discountId, string? phoneNumber, string? address);
        Task<bool> AddPayment(string method);
        Task<bool> AddToOrderDetailFromShop(CartItem cart, int orderId);
        Task<bool> AddToOrderDetailFromCart(List<CartItem> carts);
        Task<bool> UpdateCartAfterBuy(List<CartItem> carts);
        Task<bool> UpdateKoiAndBatchStatus(List<CartItem> carts);
        Task<bool> UpdatePayment();
        Task<int> GetCurentOrderId();
        Task<IEnumerable<T>> GetKoiOrBatch<T>(int orderId);

        // ===========================================================================================
        Task<IEnumerable<Order>> GetOrders(string status, DateTime startDate, DateTime endDate);
        Task<IEnumerable<Order>> GetOrders(DateTime startDate, DateTime endDate);
        Task<IEnumerable<OrderDetail>> GetOrderDetails(string status, DateTime startDate, DateTime endDate);
        Task<IEnumerable<OrderDetail>> GetOrderDetails(DateTime startDate, DateTime endDate);

    }
}
