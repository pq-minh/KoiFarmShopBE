﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShop.Application.Dtos
{
    public class OrderDtos
    {
        public int OrderId { get; set; }

        public float? TotalAmount { get; set; }

        public DateTime? CreateDate { get; set; }

        public string? OrderStatus { get; set; }

        public int? DiscountId { get; set; }

        public string? ShippingAddress { get; set; }

        public string? PhoneNumber { get; set; }
    }
    public class OrderDtoV1
    {
        public List<CartDtoV2> Carts { get; set; }
        public string Method { get; set; }
        public int DiscountId { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }
}