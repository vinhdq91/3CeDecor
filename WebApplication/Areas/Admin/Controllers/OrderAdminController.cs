using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DAL.Core.Enums;
using DAL.Dtos;
using DAL.Models.Application;
using DAL.UnitOfWorks.CustomerService;
using DAL.UnitOfWorks.OrderDetailService;
using DAL.UnitOfWorks.OrderService;
using DAL.UnitOfWorks.ProductService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApplication.BuildLink;

namespace WebApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderAdminController : BaseController
    {
        IOrderService _orderService;
        IOrderDetailService _orderDetailService;
        ICustomerService _customerService;
        IProductService _productService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IBuildLinkProduct _buildLinkProduct;
        private IConfiguration _configuration;
        public OrderAdminController(
            IOrderService orderService,
            IOrderDetailService orderDetailService,
            ICustomerService customerService,
            IProductService productService,
            IWebHostEnvironment hostEnvironment,
            IMapper mapper,
            IHttpContextAccessor contextAccessor,
            IBuildLinkProduct buildLinkProduct,
            IConfiguration configuration
        )
        {
            _orderService = orderService;
            _orderDetailService = orderDetailService;
            _customerService = customerService;
            _productService = productService;
            _hostEnvironment = hostEnvironment;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _buildLinkProduct = buildLinkProduct;
            _configuration = configuration;
        }
        public async Task<ActionResult<List<OrderDto>>> OrderList()
        {
            IQueryable<OrderEntity> orderEntities = await _orderService.GetListAllOrdersAsync();
            IQueryable<CustomerEntity> customerEntities = await _customerService.GetListAllCustomersAsync();
            var query = from order in orderEntities
                        join customer in customerEntities on order.CustomerId equals customer.Id
                        select new { Order = order, Customer = customer };

            List<OrderDto> listOrderDtos = new List<OrderDto>();
            List<int> orderIds = query.Select(x => x.Order.Id).Distinct().ToList();
            foreach (var orderItem in orderIds)
            {
                var order = query.ToList().Find(x => x.Order.Id == orderItem);
                OrderDto orderDto = _mapper.Map<OrderDto>(order.Order);
                if (orderDto != null)
                {
                    listOrderDtos.Add(orderDto);
                }
            }
            return View(listOrderDtos);
        }

        public async Task<ActionResult<List<OrderDetailDto>>> OrderDetailList(int orderId, int customerId)
        {
            IQueryable<OrderDetailEntity> orderDetailEntities = await _orderDetailService.GetListAllOrderDetailByOrderIdAsync(orderId);
            List<OrderDetailDto> listOrderDetailDtos = new List<OrderDetailDto>();
            foreach (OrderDetailEntity orderDetailItem in orderDetailEntities.ToList())
            {
                OrderDetailDto orderDetailDto = _mapper.Map<OrderDetailDto>(orderDetailItem);
                if (orderDetailDto != null)
                {
                    listOrderDetailDtos.Add(orderDetailDto);
                }
            }

            // Lấy thông tin khách hàng
            CustomerEntity customerEntity = await _customerService.GetCustomerAsync(customerId);
            CustomerDto customerDto = _mapper.Map<CustomerDto>(customerEntity);
            ViewBag.CustomerInfo = customerDto;
            return View(listOrderDetailDtos);
        }

        public async Task<ActionResult> DeleteOrder(int orderId)
        {
            OrderEntity orderEntity = await _orderService.GetOrderAsync(orderId);
            if (orderEntity != null)
            {
                await _orderService.RemoveAsync(orderEntity);
            }

            return RedirectToAction("OrderList");
        }

        public async Task<ActionResult> ChangeStatus(int orderId)
        {
            OrderEntity orderEntity = await _orderService.GetOrderAsync(orderId);
            if (orderEntity != null)
            {
                if (orderEntity.Status == (int)OrderStatusEnum.Confirm)
                {
                    orderEntity.Status = (int)OrderStatusEnum.NotConfirm;
                }
                else
                {
                    orderEntity.Status = (int)OrderStatusEnum.Confirm;
                }
                await _orderService.UpdateAsync(orderEntity);
            }

            return RedirectToAction("OrderList");
        }
    }
}