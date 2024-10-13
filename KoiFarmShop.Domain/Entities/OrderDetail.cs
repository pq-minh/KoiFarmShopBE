﻿using KoiFarmShop.Domain.Entities;
using System;
using System.Collections.Generic;

namespace KoiFarmShop.Domain.Entities
{

    public partial class OrderDetail
    {
        public int OrderDetailsId { get; set; }

        public int? OrderId { get; set; }

        public int? KoiId { get; set; }

        public int? BatchKoiId { get; set; }

        public int? ToTalQuantity { get; set; }

        public virtual BatchKoi? BatchKoi { get; set; }

        public virtual Koi? Koi { get; set; }

        public virtual Order? Order { get; set; }
    }
}