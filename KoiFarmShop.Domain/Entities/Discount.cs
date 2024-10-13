﻿using System;
using System.Collections.Generic;

namespace KoiFarmShop.Domain.Entities
{

    public partial class Discount
    {
        public int DiscountId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public double? DiscountRate { get; set; }

        public int? TotalQuantity { get; set; }

        public int? Used { get; set; }

        public DateOnly? CreateDate { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public int? Status { get; set; }

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}