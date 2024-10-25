﻿using KoiShop.Application.Dtos;
using KoiShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShop.Application.Interfaces
{
    public interface IRequestCareService
    {
        Task<IEnumerable<Request>> GetKoiOrBatchCare();
        Task<IEnumerable<OrderDetailDtos>> GetCurrentOrderdetail();
        Task<IEnumerable<RequestCareDtos>> GetAllRequestCareByStaff();
        Task<IEnumerable<RequestCareDtos>> GetAllRequestCareByCustomer();
        Task<bool> AddKoiOrBatchToPackage(List<OrderDetailDtoV1> orderDetails);
        Task<bool> AddKoiOrBatchToRequest(List<OrderDetailDtoV1> orderDetails, DateTime endDate);
        Task<bool> UpdateKoiOrBatchToCare(int? id);
    }
}
