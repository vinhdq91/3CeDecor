using DAL.Common;
using DAL.Core.Utilities;
using DAL.Models.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace WebApplication.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AuthAdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationSettings _appSettings;
        private readonly ILogger<AuthAdminController> _logger;

        public AuthAdminController(
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

        #region Login
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            return View("Index");
        }

        [TempData]
        public string ErrorMessage { get; set; }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            // Đã đăng nhập nên chuyển hướng về Index
            if (_signInManager.IsSignedIn(User))
            {
                ApplicationUser user = await _userManager.GetUserAsync(User);
                if (!user.IsAdminUser)
                {
                    await _signInManager.SignOutAsync();
                    // Xóa Session nếu user không phải admin user
                    SessionHelpers.SetObjectAsJson(HttpContext.Session, Constant.ADMIN_GROUP, null);
                }
                else
                {
                    // Lưu Session để check trong BaseController
                    SessionHelpers.SetObjectAsJson(HttpContext.Session, Constant.ADMIN_GROUP, user);
                    return RedirectToAction("ProductList", "ProductAdmin");
                }
            }

            if (ModelState.IsValid)
            {
                ApplicationUser userLogin = await _userManager.FindByEmailAsync(loginModel.Email);
                Microsoft.AspNetCore.Identity.SignInResult result = new Microsoft.AspNetCore.Identity.SignInResult();
                if (userLogin != null)
                {
                    result = await _signInManager.PasswordSignInAsync(
                        userLogin,
                        loginModel.Password,
                        loginModel.RememberMe,
                        true
                    );
                }

                if (result.Succeeded)
                {
                    _logger.LogInformation("User đã đăng nhập");

                    // Check user có quyền nào không thì mới lưu session để cho đăng nhập admin
                    if (userLogin != null)
                    {
                        foreach (ApplicationRole roleItem in _roleManager.Roles.ToList())
                        {
                            if (await _userManager.IsInRoleAsync(userLogin, roleItem.Name))
                            {
                                userLogin.IsAdminUser = true;
                            }
                        }

                        if (userLogin.IsAdminUser)
                        {
                            // Lưu Session để check trong BaseController
                            SessionHelpers.SetObjectAsJson(HttpContext.Session, Constant.ADMIN_GROUP, userLogin);
                            return RedirectToAction("ProductList", "ProductAdmin");
                        }
                    }

                    // Xoá giá trị trong Session nếu không phải admin user
                    SessionHelpers.SetObjectAsJson(HttpContext.Session, Constant.ADMIN_GROUP, null);
                    return RedirectToAction("ProductList", "ProductAdmin");
                }
                if (result.RequiresTwoFactor)
                {
                    // Nếu cấu hình đăng nhập hai yếu tố thì chuyển hướng đến LoginWith2fa
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = "/", RememberMe = loginModel.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Tài khoản bí tạm khóa.");
                    // Chuyển hướng đến trang Lockout - hiện thị thông báo
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Không đăng nhập được.");
                    return View();
                }
            }
            return RedirectToAction("ProductList", "ProductAdmin");
        }
        #endregion

        #region LogOut
        public async Task<IActionResult> LogOut()
        {
            if (!_signInManager.IsSignedIn(User)) return RedirectToPage("/");

            await _signInManager.SignOutAsync();
            _logger.LogInformation("Người dùng đăng xuất");

            // Xoá giá trị trong Session ADMIN_GROUP
            SessionHelpers.SetObjectAsJson(HttpContext.Session, Constant.ADMIN_GROUP, null);
            return RedirectToAction("Login");
        }
        #endregion

        //public ActionResult logout()
        //{
        //    Session["Admin_id"] = "";
        //    Session["Admin_user"] = "";
        //    Response.Redirect("~/Admin");
        //    return View();
        //}

        //// GET: Admin/User/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Muser muser = db.users.Find(id);
        //    ViewBag.role = db.roles.Where(m => m.parentId == muser.access).First();
        //    if (muser == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View("_information", muser);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(Muser muser)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        muser.img = "ádasd";
        //        muser.access = 0;
        //        muser.created_at = DateTime.Now;
        //        muser.updated_at = DateTime.Now;
        //        muser.created_by = int.Parse(Session["Admin_id"].ToString());
        //        muser.updated_by = int.Parse(Session["Admin_id"].ToString());
        //        db.Entry(muser).State = EntityState.Modified;
        //        db.SaveChanges();
        //        Message.set_flash("Cập nhật thành công", "success");
        //        ViewBag.role = db.roles.Where(m => m.parentId == muser.access).First();
        //        return View("_information", muser);
        //    }
        //    Message.set_flash("Cập nhật Thất Bại", "danger");
        //    ViewBag.role = db.roles.Where(m => m.parentId == muser.access).First();
        //    return View("_information", muser);
        //}

    }
}