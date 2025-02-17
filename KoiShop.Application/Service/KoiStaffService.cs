﻿using AutoMapper;
using KoiShop.Application.Dtos.KoiDtos;
using KoiShop.Application.Interfaces;
using KoiShop.Application.Users;
using KoiShop.Domain.Entities;
using KoiShop.Domain.Respositories;
using KoiShop.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KoiShop.Application.Users.UserContext;

namespace KoiShop.Application.Service
{
    public class KoiStaffService : IKoiStaffService
    {
        private readonly IMapper _mapper;
        private readonly IFishRepository _koiRepository;
        private readonly FirebaseService _firebaseService;
        List<string> koiStatus = new() { "OnSale", "Sold", "Pending", "Cancel" };

        public KoiStaffService(IFishRepository koiRepository, FirebaseService firebaseService, IMapper mapper)
        {
            _koiRepository = koiRepository;
            _firebaseService = firebaseService;
            _mapper = mapper;
        }


        // Koi Methods =============================================================================================
        public async Task<IEnumerable<Koi>> GetAllKoiStaff()
        {
            return await _koiRepository.GetAllFish<Koi>();
        }

        public async Task<Koi?> GetKoiById(int koiId)
        {
            return await _koiRepository.GetFishByIdFromType<Koi>(koiId, Variables.STATUS_FISH_ALL);
        }

        public async Task<bool> AddFish(AddKoiDto koiDto)
        {

            if (koiDto == null) return false;

            if (!koiStatus.Contains(koiDto.Status)) return false;

            var allCates = await _koiRepository.GetAllFishCategories();

            bool exist = false;
            foreach (var cate in allCates)
            {
                if (cate.FishTypeId == koiDto.FishTypeId)
                    exist = true;
            }

            if (!exist)
                return false;


            var koiImageUrl = await _firebaseService.UploadFileToFirebaseStorage(koiDto.KoiImage, "KoiFishImage");
            var cerImageUrl = await _firebaseService.UploadFileToFirebaseStorage(koiDto.Certificate, "KoiFishCertificate");


            var koi = new Koi
            {
                FishTypeId = koiDto.FishTypeId,
                Name = koiDto.Name,
                Origin = koiDto.Origin,
                Description = koiDto.Description,
                Gender = koiDto.Gender,
                Image = koiImageUrl,
                Age = koiDto.Age,
                Weight = koiDto.Weight,
                Size = koiDto.Size,
                Personality = koiDto.Personality,
                Status = koiDto.Status,
                Price = koiDto.Price,
                Certificate = cerImageUrl
            };

            return await _koiRepository.AddFish(koi);
        }

        public async Task<bool> UpdateFish(UpdateKoiDto koiDto)
        {

            if (koiDto == null) return false;

            var currentKoi = await _koiRepository.GetFishByIdFromType<Koi>(koiDto.KoiId, Variables.STATUS_FISH_ALL);
            if (currentKoi == null) return false;

            if (koiDto.FishTypeId.HasValue)
            {
                var allCates = await _koiRepository.GetAllFishCategories();

                bool exist = false;
                foreach (var cate in allCates)
                {
                    if (cate.FishTypeId == koiDto.FishTypeId)
                        exist = true;
                }

                if (exist)
                    currentKoi.FishTypeId = koiDto.FishTypeId;
                else
                    return false;
            }

            if (!string.IsNullOrEmpty(koiDto.Name)) currentKoi.Name = koiDto.Name;
            if (!string.IsNullOrEmpty(koiDto.Origin)) currentKoi.Origin = koiDto.Origin;
            if (!string.IsNullOrEmpty(koiDto.Description)) currentKoi.Description = koiDto.Description;
            if (!string.IsNullOrEmpty(koiDto.Gender)) currentKoi.Gender = koiDto.Gender;
            if (koiDto.Age.HasValue) currentKoi.Age = koiDto.Age;
            if (koiDto.Weight.HasValue) currentKoi.Weight = koiDto.Weight;
            if (koiDto.Size.HasValue) currentKoi.Size = koiDto.Size;
            if (!string.IsNullOrEmpty(koiDto.Personality)) currentKoi.Personality = koiDto.Personality;
            if (!string.IsNullOrEmpty(koiDto.Status) && koiStatus.Contains(koiDto.Status)) currentKoi.Status = koiDto.Status;
            if (koiDto.Price.HasValue) currentKoi.Price = koiDto.Price;

            if (koiDto.KoiImage != null)
            {
                var koiImage = await UpdateImage(koiDto.KoiImage, currentKoi.Image, "KoiFishImage");
                if (string.IsNullOrEmpty(koiImage))
                    return false;
                else
                    currentKoi.Image = koiImage;
            }


            if (koiDto.Certificate != null)
            {
                var koiCer = await UpdateImage(koiDto.Certificate, currentKoi.Certificate, "KoiFishCertificate");
                if (string.IsNullOrEmpty(koiCer))
                    return false;
                else
                    currentKoi.Certificate = koiCer;
            }

            return await _koiRepository.UpdateFish(currentKoi);
        }


        public async Task<string> UpdateImage(IFormFile imageFile, string oldImagePath, string directory)
        {
            if (imageFile == null)
            {
                return null;
            }

            // sóa ảnh cũ 
            string imagePath = _firebaseService.GetRelativeFilePath(oldImagePath);
            if (!string.IsNullOrEmpty(imagePath))
            {
                await _firebaseService.DeleteFileInFirebaseStorage(imagePath);
            }

            // up ảnh mới
            string newImageUrl = await _firebaseService.UploadFileToFirebaseStorage(imageFile, directory);
            if (string.IsNullOrEmpty(newImageUrl))
            {
                return null;
            }

            return newImageUrl;
        }


        public async Task<bool> UpdateFishStatus(int koiId, string status)
        {

            if (!koiStatus.Contains(status)) return false;

            var koi = await _koiRepository.GetFishByIdFromType<Koi>(koiId, Variables.STATUS_FISH_ALL);
            if (koi == null) return false;

            koi.Status = status;

            return await _koiRepository.UpdateFish(koi);
        }

        // FishCategory Methods ======================================================================================
        public async Task<IEnumerable<FishCategory>> GetAllFishCategory()
        {
            return await _koiRepository.GetAllFishCategories();
        }

        public async Task<IEnumerable<Koi>> GetKoisInFishCategory(int fishTypeId)
        {
            return await _koiRepository.GetFishCategory<Koi>(fishTypeId);
        }

    }
}
