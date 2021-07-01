using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Dtos;
using DAL.Models.Application;
using DAL.UnitOfWorks.DiscountCodeService;
using DAL.Core.Enums;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DiscountCodeAdminController : BaseController
    {
        private readonly IDiscountCodeService _discountCodeService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostEnvironment;

        public DiscountCodeAdminController(
            IDiscountCodeService discountCodeService,
            IMapper mapper,
            IWebHostEnvironment hostEnvironment
        )
        {
            _discountCodeService = discountCodeService;
            _mapper = mapper;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<ActionResult<List<DiscountCodeDto>>> DiscountCodeList()
        {
            IQueryable<DiscountCodeEntity> discountCodeEntities = await _discountCodeService.GetListAllDiscountCodeAsync();
            List<DiscountCodeDto> listDiscountCodeDtos = new List<DiscountCodeDto>();
            if(discountCodeEntities != null)
            {
                foreach (DiscountCodeEntity item in discountCodeEntities.ToList())
                {
                    DiscountCodeDto discountCodeDto = _mapper.Map<DiscountCodeDto>(item);
                    if (discountCodeDto != null)
                    {
                        listDiscountCodeDtos.Add(discountCodeDto);
                    }
                }
            }

            return View(listDiscountCodeDtos);
        }

        #region Create
        [HttpGet]
        public IActionResult CreateDiscountCode()
        {
            List<SelectListItem> listStatus = new List<SelectListItem>()
            {
                new SelectListItem(){ Value = (int)DiscountCodeStatus.Active + "", Text = "Mở khóa" },
                new SelectListItem(){ Value = (int)DiscountCodeStatus.NotActive + "", Text = "Khóa" }
            };

            ViewBag.ListStatus = listStatus;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateDiscountCode(DiscountCodeDto discountCodeDto)
        {
            if (ModelState.IsValid)
            {
               
                DiscountCodeEntity discountCodeEntity = _mapper.Map<DiscountCodeEntity>(discountCodeDto);
                await _discountCodeService.AddDiscountCodeAsync(discountCodeEntity);
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                       .Where(y => y.Count > 0)
                                       .ToList();
            }
            return RedirectToAction("DiscountCodeList");
        }
        #endregion

        #region Edit
        [HttpGet]
        public async Task<ActionResult<DiscountCodeDto>> EditDiscountCode(int discountCodeId)
        {
            DiscountCodeEntity discountCodeEntity = await _discountCodeService.GetDiscountCodeAsync(discountCodeId);
            DiscountCodeDto discountCodeDto = _mapper.Map<DiscountCodeDto>(discountCodeEntity);

            List<SelectListItem> listStatus = new List<SelectListItem>()
            {
                new SelectListItem(){ Value = (int)DiscountCodeStatus.Active + "", Text = "Mở khóa" },
                new SelectListItem(){ Value = (int)DiscountCodeStatus.NotActive + "", Text = "Khóa" }

            };

            ViewBag.ListStatus = listStatus;
            return View(discountCodeDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditDiscountCode(DiscountCodeDto discountCodeDto)
        {
            if (ModelState.IsValid)
            {
                // Update DiscountCode
                DiscountCodeEntity discountCodeInput = _mapper.Map<DiscountCodeEntity>(discountCodeDto);
                await _discountCodeService.UpdateDiscountCodeAsync(discountCodeInput);
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                       .Where(y => y.Count > 0)
                                       .ToList();
            }
            return RedirectToAction("DiscountCodeList");
        }
        #endregion
        public async Task<ActionResult> DeleteDiscountCode(int discountCodeId)
        {
            if (discountCodeId > 0)
            {
                DiscountCodeEntity discountCodeEntity = await _discountCodeService.GetDiscountCodeAsync(discountCodeId);
                // Xóa discountCode
                await _discountCodeService.DeleteDiscountCodeAsync(discountCodeEntity);
            }
            return RedirectToAction("DiscountCodeList");
        }
        public async Task<IActionResult> ChangeStatus(int discountCodeId)
        {
            DiscountCodeEntity discountCodeEntity = await _discountCodeService.GetDiscountCodeAsync(discountCodeId);
            if (discountCodeEntity.Status == (int)DiscountCodeStatus.Active)
            {
                discountCodeEntity.Status = (int)DiscountCodeStatus.NotActive;
            }
            else
            {
                discountCodeEntity.Status = (int)DiscountCodeStatus.Active;
            }
            try
            {
                await _discountCodeService.UpdateDiscountCodeAsync(discountCodeEntity);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

            return RedirectToAction("DiscountCodeList");
        }
    }
}