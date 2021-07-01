using AutoMapper;
using DAL.Common;
using DAL.Core.Enums;
using DAL.Core.Utilities;
using DAL.Dtos;
using DAL.EntityFrameworkCore.Application;
using DAL.Models.Account;
using DAL.Models.Application;
using DAL.UnitOfWorks.DiscountCodeService;
using DAL.UnitOfWorks.DistrictService;
using DAL.UnitOfWorks.ImageService;
using DAL.UnitOfWorks.MailService;
using DAL.UnitOfWorks.OrderDetailService;
using DAL.UnitOfWorks.OrderService;
using DAL.UnitOfWorks.ProductService;
using DAL.UnitOfWorks.ProvinceService;
using DAL.UnitOfWorks.WardService;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.NganLuongAPI;

namespace WebApplication.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOrderService _orderService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IProductService _productService;
        private readonly IImageService _imgService;
        private readonly IProvinceService _provinceService;
        private readonly IDistrictService _districtService;
        private readonly IWardService _wardService;
        private readonly IDiscountCodeService _discountCodeService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;
        private readonly IMailer _mailer;

        public const string CartSessionKey = "3CECartSessionId";
        public PaymentsController(
            SignInManager<ApplicationUser> signInManager, 
            IOrderService orderService,
            IOrderDetailService orderDetailService,
            IProductService productService,
            IImageService imgService,
            IProvinceService provinceService,
            IDistrictService districtService,
            IWardService wardService,
            IDiscountCodeService discountCodeService,
            IHttpContextAccessor contextAccessor,
            IMapper mapper,
            IMailer mailer)
        {
            _signInManager = signInManager;
            _orderService = orderService;
            _orderDetailService = orderDetailService;
            _productService = productService;
            _imgService = imgService;
            _provinceService = provinceService;
            _districtService = districtService;
            _wardService = wardService;
            _discountCodeService = discountCodeService;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
            _mailer = mailer;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<CartItemDto> cart = SessionHelpers.GetObjectFromJson<List<CartItemDto>>(HttpContext.Session, CartSessionKey);
            if (cart == null)
                cart = new List<CartItemDto>();
            if (!_signInManager.IsSignedIn(User))
            {
                return Redirect("/login");
            }

            #region Meta tags
            ViewBag.MetaTitle = "Thông tin thanh toán của khách hàng.";
            ViewBag.MetaDescription = SEO.AddMeta("description", "Cập nhật thông tin cá nhân cũng như thông tin thanh toán của bạn tại đây.");
            #endregion

            // List Province
            IQueryable<ProvinceEntity> provinceEntities = await _provinceService.GetListAllProvincesAsync();
            List<ProvinceDto> listProvinceDto = new List<ProvinceDto>();
            foreach (ProvinceEntity provinceItem in provinceEntities.ToList())
            {
                ProvinceDto provinceDto = _mapper.Map<ProvinceDto>(provinceItem);
                if (provinceDto != null)
                {
                    listProvinceDto.Add(provinceDto);
                }
            }
            ProvinceDto haNoi = listProvinceDto.SingleOrDefault(x => x.ProvinceCode == "2");
            listProvinceDto.Remove(haNoi);
            listProvinceDto.Insert(0, haNoi);
            ViewBag.ListProvinces = listProvinceDto;
            return View(cart);
        }

        #region Generate District, Ward & Check discount code
        public async Task<ActionResult> GenerateDistrict(string provinceCode)
        {
            IQueryable<DistrictEntity> districtEntities = await _districtService.GetListAllDistrictsByProvinceAsync(provinceCode);
            List<DistrictDto> listDistrictDto = new List<DistrictDto>();
            foreach(DistrictEntity districtItem in districtEntities.ToList())
            {
                DistrictDto districtDto = _mapper.Map<DistrictDto>(districtItem);
                if (districtDto != null)
                {
                    listDistrictDto.Add(districtDto);
                }
            }
            return Json(listDistrictDto, new Newtonsoft.Json.JsonSerializerSettings());
        }

        public async Task<ActionResult> GenerateWard(string districtCode)
        {
            IQueryable<WardEntity> wardEntities = await _wardService.GetListAllWardsByDistrictAsync(districtCode);
            List<WardDto> listWardDto = new List<WardDto>();
            foreach (WardEntity wardItem in wardEntities)
            {
                WardDto wardDto = _mapper.Map<WardDto>(wardItem);
                if (wardDto != null)
                {
                    listWardDto.Add(wardDto);
                }
            }
            return Json(listWardDto, new Newtonsoft.Json.JsonSerializerSettings());
        }

        public async Task<ActionResult> CheckDiscountCode(string discountCode)
        {
            Message message = new Message();
            if (!string.IsNullOrWhiteSpace(discountCode))
            {
                DiscountCodeEntity discountCodeEntity = await _discountCodeService.CheckDiscountCodeAsync(discountCode);
                if (discountCodeEntity != null)
                {
                    message.Obj = discountCodeEntity;
                    message.Error = false;
                }
                else
                {
                    message.Error = true;
                    message.Title = "Mã không hợp lệ hoặc đã hết hạn.";
                }
            }
            return Json(message, new Newtonsoft.Json.JsonSerializerSettings());
        }
        #endregion

        [HttpPost]
        public async Task<IActionResult> GetOrderInfo(OrderEntity order)
        {
            if (order.PaymentMethod.Equals("COD"))
            {
                // cap nhat thong tin sau khi dat hang thanh cong
                await SaveOrder(order);
                var cart = SessionHelpers.GetObjectFromJson<List<CartItemDto>>(HttpContext.Session, CartSessionKey);
                var listCarts = new List<CartItemDto>();
                ViewBag.ListCart = (List<CartItemDto>)cart;
                SessionHelpers.SetObjectAsJson(HttpContext.Session, CartSessionKey, null);
                IQueryable<OrderDetailEntity> listOrderDetails = await _orderDetailService.GetListAllOrderDetailByOrderIdAsync(order.Id);
                listOrderDetails = listOrderDetails.Where(m => m.OrderEntityId == order.Id);
                return View("PaymenSuccess"/*, listOrderDetails.ToList()*/);
            }
            string str_bankcode = order.BankCode != null ? order.BankCode : string.Empty;
            RequestInfo info = new RequestInfo();
            info.Merchant_id = nganluongInfo.Merchant_id;
            info.Merchant_password = nganluongInfo.Merchant_password;
            info.Receiver_email = nganluongInfo.Receiver_email;
            info.cur_code = "VND";
            info.bank_code = str_bankcode;
            info.Order_code = "DH_6437";
            //info.Total_amount = sumOrder;
            info.fee_shipping = "0";
            info.Discount_amount = "0";
            info.order_description = "Thanh toán ngân lượng cho đơn hàng";
            info.return_url = "/confirm_orderPaymentOnline";
            info.cancel_url = "/cancel_order";

            info.Buyer_fullname = order.Customer.Name;
            info.Buyer_email = order.Customer.Email;
            info.Buyer_mobile = order.Customer.PhoneNumber;

            APICheckoutV3 objNLChecout = new APICheckoutV3();
            ResponseInfo result = objNLChecout.GetUrlCheckout(info, order.PaymentMethod);

            if (result.Error_code == "00")
            {

                return Redirect(result.Checkout_url);
            }
            else
            {
                ViewBag.errorPaymentOnline = result.Description;
                return View("PaymenSuccess");
            }
        }

        public ActionResult Cancel_order()
        {
            return View();
        }
        public ActionResult Confirm_orderPaymentOnline()
        {
            //String Token = Request["token"];
            String Token = "";
            RequestCheckOrder info = new RequestCheckOrder();
            info.Merchant_id = nganluongInfo.Merchant_id;
            info.Merchant_password = nganluongInfo.Merchant_password;
            info.Token = Token;
            APICheckoutV3 objNLChecout = new APICheckoutV3();
            ResponseCheckOrder result = objNLChecout.GetTransactionDetail(info);
            if (result.errorCode == "00")
            {

                ViewBag.Status = true;
            }
            else
            {
                ViewBag.Status = false;
            }

            return View();
        }

        public async Task SaveOrder(OrderEntity orderEntity)
        {
            var cart = SessionHelpers.GetObjectFromJson<List<CartItemDto>>(HttpContext.Session, CartSessionKey);
            var listCarts = new List<CartItemDto>();
            if (cart != null)
            {
                listCarts = cart;
            }

            if (ModelState.IsValid)
            {
                orderEntity.CreatedDate = DateTime.Now;
                orderEntity.UpdatedDate = DateTime.Now;
                orderEntity.Status = (int)OrderStatusEnum.NotConfirm;
                orderEntity.DiscountCode = orderEntity.Customer.DiscountCode;
                orderEntity.Customer.ShippingFee = 30000;

                // Generate orderCode with 6 characters
                string orderCodeGuid = Guid.NewGuid().ToString();
                string orderCode = orderCodeGuid.ToString().Substring(orderCodeGuid.Length - 6);
                orderEntity.OrderCode = orderCode;
                await _orderService.AddOrderAsync(orderEntity);

                var province = await _provinceService.GetProvinceAsync(orderEntity.Customer.ProvinceCode);
                string provinceName = province != null ? province.Name : string.Empty;
                var district = await _districtService.GetDistrictAsync(orderEntity.Customer.DistrictCode);
                string districtName = district != null ? district.Name : string.Empty;
                var ward = await _wardService.GetWardAsync(orderEntity.Customer.WardCode);
                string wardName = ward != null ? ward.Name : string.Empty;
                orderEntity.Customer.Address = string.Format("{0}{1}{2}{3}",
                                                                orderEntity.Customer.Address,
                                                                !string.IsNullOrEmpty(wardName) ? ", " + wardName : string.Empty,
                                                                !string.IsNullOrEmpty(districtName) ? ", " + districtName : string.Empty,
                                                                !string.IsNullOrEmpty(provinceName) ? ", " + provinceName : string.Empty
                                                            );
                ViewBag.Name = orderEntity.Customer.Name;
                ViewBag.OrderCode = orderEntity.OrderCode;
                ViewBag.Email = orderEntity.Customer.Email;
                ViewBag.Address = orderEntity.Customer.Address;
                ViewBag.OrderId = orderEntity.Id;
                ViewBag.CustomerId = orderEntity.Customer.Id;
                ViewBag.Phone = orderEntity.Customer.PhoneNumber;
                ViewBag.ShippingFee = orderEntity.Customer.ShippingFee;

                OrderDetailEntity orderDetail = new OrderDetailEntity();
                foreach (var item in listCarts)
                {
                    orderDetail.OrderEntityId = orderEntity.Id;
                    orderDetail.ProductEntityId = item.Product.Id;
                    orderDetail.UnitPrice = (int)item.Product.Price;
                    orderDetail.Quantity = item.Quantity;
                    orderDetail.Discount = item.Product.Discount;
                    orderDetail.ProductName = item.Product.Name;
                    _orderDetailService.AddOrderDetailAsync(orderDetail);

                    ViewBag.Sum = listCarts.Sum((Func<CartItemDto, int>)(m => (int)m.Product.Price * (int)m.Quantity));

                    // Update numberInStock Product
                    //IEnumerable<ImageEntity> lstImages =  _imgService.GetListAllImagesAsync();
                    //updatedProduct.ImageIds = lstImages.ToList().FindAll(x => x.ProductEntityId == updatedProduct.Id);
                    //updatedProduct.UrlPath = "https://" + _contextAccessor.HttpContext.Request.Host.Value;
                    //ProductDto productDto = _mapper.Map<ProductDto>(updatedProduct);
                    //productDto.ProductCategoryId = item.Product.ProductCategoryId;
                    //productDto.Name = item.Product.Name;
                    //productDto.ImageIds = item.Product.ImageIds;
                    //productDto.Description = item.Product.Description;
                    //productDto.Discount = item.Product.Discount;
                    //productDto.Price = item.Product.Price;
                    //productDto.MetaTitle = item.Product.MetaTitle;
                    //productDto.MetaDescription = item.Product.MetaDescription;
                    //productDto.CreatedBy = item.Product.CreatedBy;
                    //productDto.CreatedDate = item.Product.CreatedDate;
                    //productDto.UpdatedBy = item.Product.UpdatedBy;
                    //productDto.UpdatedDate = item.Product.UpdatedDate;
                    //productDto.Status = item.Product.Status;
                    //ProductEntity productEntity = _mapper.Map<ProductEntity>(productDto);
                    //ProductEntity updatedProduct = await _productService.GetProductAsync(item.Product.Id);
                    //updatedProduct.NumberInStock = (int)updatedProduct.NumberInStock - (int)item.Quantity;
                    //await _productService.UpdateProductAsync(updatedProduct);
                }

                // Gửi mail cho khách và cho shop
                string contentForCustomer = GenerateCustomerMailContent(orderEntity, listCarts);
                await _mailer.SendEmailAsync(orderEntity.Customer.Email, "Đặt hàng thành công", contentForCustomer);

                string contentForShop = GenerateShopMailContent(orderEntity, listCarts);
                await _mailer.SendEmailAsync("decor3ce@gmail.com", "Một khách hàng vừa đặt hàng thành công", contentForShop);
            }
            else
            {
                var errors = ModelState.Select(x => x.Value.Errors)
                                       .Where(y => y.Count > 0)
                                       .ToList();
            }
        }

        private string GenerateCustomerMailContent(OrderEntity orderEntity, List<CartItemDto> listCarts)
        {
            string html = "<html><body><h3>Cảm ơn quý khách đã đặt hàng tại 3CE Decor. Dưới đây là thông tin đơn hàng: </h3>";
            html += @"<div class='section pt-4'>
                        <div class='container bg-white'>
                            <div class='row'>
                                <div class='col-md-12'>
                                    <div class='order-summary clearfix'>
                                        <h3 class='title text-cam'>Thông Tin Khách Hàng</h3>
                                        <table class='shopping-cart-table table'>

                                        <tbody>
                                            <tr>
                                                <td class=''><h5>Tên khách hàng: </h5></td>
                                                <td>" + orderEntity.Customer.Name + "</td>" +
                                            @"</tr>
                                            <tr>
                                                <td class=''><h5>Mã đơn hàng</h5></td>" +
                                                "<td>" + orderEntity.OrderCode + "</td>" +
                                            @"</tr>
                                            <tr>
                                                <td class=''><h5>Địa chỉ</h5></td>" +
                                                "<td>" + orderEntity.Customer.Address + "</td>" +
                                            @"</tr>
                                            <tr>
                                                <td class=''><h5>Email</h5></td>" +
                                                "<td>" + orderEntity.Customer.Email + "</td>" +
                                            @"</tr>
                                            <tr>
                                                <td class=''><h5>Số điện thoại</h5></td>" +
                                                "<td>" + orderEntity.Customer.PhoneNumber + "</td>" +
                                            @"</tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                        <div class='order-summary clearfix'>
                            <div class='section-title'>
                                <h3 class='title text-cam'>Thông Tin Đơn Hàng</h3>
                            </div>
                            <table class='shopping-cart-table table'>
                                <thead>
                                    <tr>
                                        <th>Mã sản phẩm</th>
                                        <th>Tên sản phẩm</th>
                                        <th class='text-center'>Giá sản phẩm</th>
                                        <th class='text-center'>Số lượng</th>
                                        <th class='text-center'>Tổng</th>
                                        <th class='text-center'>Hình ảnh</th>
                                    </tr>
                                </thead>
                                <tbody>";
            float sum = 0;
            foreach (var item in listCarts)
            {
                int sale = (int)item.Product.Discount;
                float price = (int)item.Product.Price - (int)item.Product.Price / 100 * (int)sale;
                float price1 = ((int)item.Product.Price - (int)item.Product.Price / 100 * (int)sale) * (item.Quantity);
                if (sale > 0)
                {
                    sum += ((int)item.Product.Price - (int)item.Product.Price / 100 * (int)sale) * ((int)item.Quantity);
                }
                else
                {
                    sum += (int)item.Product.Price * (int)item.Quantity;
                }

                html += @"<tr>
                        <td>" + item.Product.Id + "</td>" +
                        "<td class='details'>" + item.Product.Name + "</td>";

                        if(sale > 0)
                        {
                            html += @"<th><span class='text-danger'><Strike>" + item.Product.Price.ToString("N0") + "VND</Strike></span><br />" +
                                            price.ToString("N0") + "VND" +
                                    "</th>";
                        }
                        else
                        {
                            html += "<th>" + item.Product.Price.ToString("N0") + "VND</th>";
                        }
                html += "<td class='price text-center'>" + item.Quantity + "</td>" +
                            "<td class='price text-center'>" + price1 + "</td>" +
                            "<td class='thumb'><img style='width:50px' src='https://decor3ce.com/Resource/AllImages/" + item.Product.ImageIds.ToList()[0].ImageName + "' alt =''></ td >" +
                        "</tr>";
            }
            float sumAfterFee = sum + Convert.ToInt32(orderEntity.Customer.ShippingFee);
            html += @"</tbody>
                        <tfoot class='border-0'>
                          <tr> 
                            <th class='empty' colspan='3'></th>
                            <th>Tạm tính</th>
                            <th colspan='2' class='sub-total'>" + sum.ToString("N0") + "VND</th>" +
                        "</tr>" +
                        "<tr>" +
                            "<th class='empty' colspan='3'></th>" +
                            "<th>Phí vận chuyển</th>" +
                            "<td colspan='2'>" + orderEntity.Customer.ShippingFee.ToString("N0") + "</td>" +
                        "</tr>" +
                        "<tr>" +
                            "<th class='empty' colspan='3'></th>" +
                            "<th>Tổng tiền</th>" +
                            "<th colspan='2' class='total'>" + sumAfterFee.ToString("N0") + "VND</th>" +
                        "</tr>" +
                    "</tfoot>" +
                "</table>" +
            "</div>" +
        "</div>" +
    "</div>" +
    "<p>Chúng tôi đang kiểm tra đơn hàng và sẽ liên hệ lại Quý khách sớm nhất trong vòng 1 ngày tới.</p>" +
    "<p>Cảm ơn Quý khách và chúc Quý khách một ngày vui vẻ !</p>" +
    "<h3>3CE Decor.</h3>" +
    "</body></html>";
            
            return html;
        }

        private string GenerateShopMailContent(OrderEntity orderEntity, List<CartItemDto> listCarts)
        {
            string html = "<html><body><h3>Một khách hàng vừa đặt đơn hàng thành công. Dưới đây là thông tin đơn hàng: </h3>";
            html += @"<div class='section pt-4'>
                        <div class='container bg-white'>
                            <div class='row'>
                                <div class='col-md-12'>
                                    <div class='order-summary clearfix'>
                                        <h3 class='title text-cam'>Thông Tin Khách Hàng</h3>
                                        <table class='shopping-cart-table table'>

                                        <tbody>
                                            <tr>
                                                <td class=''><h5>Tên khách hàng: </h5></td>
                                                <td>" + orderEntity.Customer.Name + "</td>" +
                                            @"</tr>
                                            <tr>
                                                <td class=''><h5>Mã đơn hàng</h5></td>" +
                                                "<td>" + orderEntity.OrderCode + "</td>" +
                                            @"</tr>
                                            <tr>
                                                <td class=''><h5>Địa chỉ</h5></td>" +
                                                "<td>" + orderEntity.Customer.Address + "</td>" +
                                            @"</tr>
                                            <tr>
                                                <td class=''><h5>Email</h5></td>" +
                                                "<td>" + orderEntity.Customer.Email + "</td>" +
                                            @"</tr>
                                            <tr>
                                                <td class=''><h5>Số điện thoại</h5></td>" +
                                                "<td>" + orderEntity.Customer.PhoneNumber + "</td>" +
                                            @"</tr>
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                        <div class='order-summary clearfix'>
                            <div class='section-title'>
                                <h3 class='title text-cam'>Thông Tin Đơn Hàng</h3>
                            </div>
                            <table class='shopping-cart-table table'>
                                <thead>
                                    <tr>
                                        <th>Mã sản phẩm</th>
                                        <th>Tên sản phẩm</th>
                                        <th class='text-center'>Giá sản phẩm</th>
                                        <th class='text-center'>Số lượng</th>
                                        <th class='text-center'>Tổng</th>
                                        <th class='text-center'>Hình ảnh</th>
                                    </tr>
                                </thead>
                                <tbody>";
            float sum = 0;            
            foreach (var item in listCarts)
            {
                int sale = (int)item.Product.Discount;
                float price = (int)item.Product.Price - (int)item.Product.Price / 100 * (int)sale;
                float price1 = ((int)item.Product.Price - (int)item.Product.Price / 100 * (int)sale) * (item.Quantity);
                if (sale > 0)
                {
                    sum += ((int)item.Product.Price - (int)item.Product.Price / 100 * (int)sale) * ((int)item.Quantity);
                }
                else
                {
                    sum += (int)item.Product.Price * (int)item.Quantity;
                }

                html += @"<tr>
                        <td>" + item.Product.Id + "</td>" +
                        "<td class='details'>" + item.Product.Name + "</td>";

                if (sale > 0)
                {
                    html += @"<th><span class='text-danger'><Strike>" + item.Product.Price.ToString("N0") + "VND</Strike></span><br />" +
                                    price.ToString("N0") + "VND" +
                            "</th>";
                }
                else
                {
                    html += "<th>" + item.Product.Price.ToString("N0") + "VND</th>";
                }
                html += "<td class='price text-center'>" + item.Quantity + "</td>" +
                            "<td class='price text-center'>" + price1 + "</td>" +
                            "<td class='thumb'><img style='width:50px' src='https://decor3ce.com/Resource/AllImages/" + item.Product.ImageIds.ToList()[0].ImageName + "' alt =''></ td >" +
                        "</tr>";
            }
            float sumAfterFee = sum + Convert.ToInt32(orderEntity.Customer.ShippingFee);
            html += @"</tbody>
                        <tfoot class='border-0'>
                          <tr> 
                            <th class='empty' colspan='3'></th>
                            <th>Tạm tính</th>
                            <th colspan='2' class='sub-total'>" + sum.ToString("N0") + "VND</th>" +
                        "</tr>" +
                        "<tr>" +
                            "<th class='empty' colspan='3'></th>" +
                            "<th>Phí vận chuyển</th>" +
                            "<td colspan='2'>" + orderEntity.Customer.ShippingFee.ToString("N0") + "</td>" +
                        "</tr>" +
                        "<tr>" +
                            "<th class='empty' colspan='3'></th>" +
                            "<th>Tổng tiền</th>" +
                            "<th colspan='2' class='total'>" + sumAfterFee.ToString("N0") + "VND</th>" +
                        "</tr>" +
                    "</tfoot>" +
                "</table>" +
            "</div>" +
        "</div>" +
    "</div>" +
    "<h3>3CE Decor.</h3>" +
    "</body></html>";

            return html;
        }
    }
}
