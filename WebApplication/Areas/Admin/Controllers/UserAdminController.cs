using DAL.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserAdminController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationSettings _appSettings;
        private readonly ILogger<AuthAdminController> _logger;

        public UserAdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager,
            IOptions<ApplicationSettings> appSettings,
            ILogger<AuthAdminController> logger
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
            _logger = logger;
        }

        public ActionResult<List<ApplicationUser>> AdminList()
        {
            List<ApplicationUser> listAdmin = _userManager.Users.ToList().FindAll(x => x.IsAdminUser == true);

            string errorMessage = (string)TempData["CreateUserErrorMessage"];
            ViewBag.ErrorMessage = errorMessage;
            return View(listAdmin);
        }

        public ActionResult<List<ApplicationUser>> CustomerList()
        {
            List<ApplicationUser> listCustomer = _userManager.Users.ToList().FindAll(x => x.IsAdminUser == false);

            string errorMessage = (string)TempData["CreateUserErrorMessage"];
            ViewBag.ErrorMessage = errorMessage;
            return View(listCustomer);
        }

        #region Create User
        [HttpGet]
        public IActionResult CreateUser()
        {
            string errorMessage = (string)TempData["CreateUserErrorMessage"];
            ViewBag.ErrorMessage = errorMessage;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateUser(ApplicationUserModel userInput)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser userEntity = new ApplicationUser()
                {
                    UserName = userInput.UserName,
                    Email = userInput.Email,
                    IsAdminUser = true
                };
                IdentityResult result = await _userManager.CreateAsync(userEntity, userInput.Password);
                if (result.Succeeded)
                {
                    // Mặc định user được tạo trong admin sẽ có quyền nhân viên cấp thấp nhất
                    ApplicationUser user = await _userManager.FindByNameAsync(userInput.UserName);
                    if (user != null)
                    {
                        result = await _userManager.AddToRoleAsync(user, "Nhân viên");
                        if (result.Succeeded)
                        {
                            return RedirectToAction("AdminList");
                        }
                    }
                    TempData["CreateUserErrorMessage"] = "Add user to role failed !";
                    return RedirectToAction("AdminList");
                }
                else
                {
                    TempData["CreateUserErrorMessage"] = result.Errors.ToList()[0].Code;
                    return RedirectToAction("CreateUser");
                }
            }
            else
            {
                return RedirectToAction("CreateUser");
            }
        }
        #endregion

        #region Edit User
        [HttpGet]
        public async Task<IActionResult> EditUser(string userId)
        {
            ApplicationUser userEntity = await _userManager.FindByIdAsync(userId);
            ApplicationUserModel userModel = new ApplicationUserModel();
            if (userEntity != null)
            {
                userModel.Id = userEntity.Id;
                userModel.UserName = userEntity.UserName;
                userModel.Email = userEntity.Email;
            }

            string errorMessage = (string)TempData["EditUserErrorMessage"];
            ViewBag.ErrorMessage = errorMessage;
            return View(userModel);
        }

        [HttpPost]
        public async Task<ActionResult> EditUser(ApplicationUserModel userInput)
        {
            ApplicationUser userEntity = await _userManager.FindByIdAsync(userInput.Id);
            if (userEntity != null)
            {
                userEntity.UserName = userInput.UserName;
                userEntity.Email = userInput.Email;
                IdentityResult result = await _userManager.UpdateAsync(userEntity);
                if (result.Succeeded)
                {
                    return RedirectToAction("AdminList");
                }
                else
                {
                    TempData["EditUserErrorMessage"] = result.Errors.ToList()[0].Code;
                    return RedirectToAction("EditUser");
                }
            }
            TempData["EditUserErrorMessage"] = "User is not exsist";
            return RedirectToAction("EditUser");
        }
        #endregion

        public async Task<IActionResult> DeleteUser(string userId)
        {
            ApplicationUser userEntity = await _userManager.FindByIdAsync(userId);
            if (userEntity != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(userEntity);
                if (result.Succeeded)
                {
                    return RedirectToAction("CustomerList");
                }
                else
                {
                    TempData["ErrorMessage"] = result.Errors.ToList()[0].Code;
                    return RedirectToAction("CustomerList");
                }
            }
            TempData["ErrorMessage"] = "User không tồn tại";
            return RedirectToAction("CustomerList");
        }
    }
}
