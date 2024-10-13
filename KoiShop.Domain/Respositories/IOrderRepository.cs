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
        Task<List<Order>> GetCompletedOrdersAsync(DateTime startDate, DateTime endDate);
    }

}