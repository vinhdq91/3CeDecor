using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class CustomersController : Controller
    {
        public IActionResult Index()
        {
            Customer customer = new Customer()
            {
                Name = "Vinh Cu Ti"
            };
            return View(customer);
        }
    }
}