using DAL.Common;
using DAL.Core.Model;
using DAL.Core.Utilities;
using DAL.Models.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebApplication.ViewComponents;

namespace WebApplication.Controllers
{
    public class ApplicationUsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationSettings _appSettings;
        private readonly ILogger<ApplicationUsersController> _logger;
        private readonly IEmailSender _emailSender;
        public ApplicationUsersController(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            IOptions<ApplicationSettings> appSettings,
            ILogger<ApplicationUsersController> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
            _logger = logger;
            _emailSender = emailSender;
        }

        #region Init Model

        //[BindProperty]
        //public ApplicationUserModel AppUserModel { get; set; }

        //public string ReturnUrl { get; set; }

        // Xác thực từ dịch vụ ngoài (Googe, Facebook ... bài này chứa thiết lập)
        public IList<AuthenticationScheme> ExternalLogins { get; set; } 

        #endregion

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
            ViewBag.ErrorMessage = ErrorMessage;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(ApplicationUserModel model)
        {
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                ApplicationUser applicationUser = new ApplicationUser()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                };
                var result = await _userManager.CreateAsync(applicationUser, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("Vừa tạo mới tài khoản thành công.");

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
                    token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                    var callbackUrl = Url.Action(
                        "ConfirmEmail", "ApplicationUsers",
                        new { userId = applicationUser.Id, token = token, isResetPass = false },
                        Request.Scheme
                    );

                    await _emailSender.SendEmailAsync(model.Email, "Xác nhận địa chỉ email",
                    $"Cảm ơn quý khách đã đến với 3CE Decor. Hãy xác nhận địa chỉ email bằng cách <a href='{callbackUrl}'>Bấm vào đây</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedEmail)
                    {
                        // Nếu cấu hình phải xác thực email mới được đăng nhập thì chuyển hướng đến trang
                        // RegisterConfirmation - chỉ để hiện thông báo cho biết người dùng cần mở email xác nhận
                        return RedirectToAction("RegisterConfirmation", new { email = applicationUser.Email });
                    }
                    else
                    {
                        // Không cần xác thực - đăng nhập luôn
                        await _signInManager.SignInAsync(applicationUser, isPersistent: false);
                        return RedirectToAction("Index", "Homes");
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View();
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

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return View();
        }

        [TempData]
        public string ErrorMessage { get; set; }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel loginModel, string returnUrl)
        {
            //var user1 = _userManager.GetUserAsync(User);
            //bool i = _signInManager.IsSignedIn(User);
            //var user = await _userManager.FindByNameAsync(loginModel.UserName);
            //if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password))
            //{
            //    var tokenDescriptor = new SecurityTokenDescriptor
            //    {
            //        Subject = new ClaimsIdentity(new Claim[]
            //        {
            //            new Claim("UserID", user.Id.ToString())
            //        }),
            //        Expires = DateTime.UtcNow.AddMinutes(60),
            //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
            //    };
            //    var tokenHandler = new JwtSecurityTokenHandler();
            //    var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            //    var token = tokenHandler.WriteToken(securityToken);
            //    return RedirectToAction("Index", "Homes");
            //}
            //else
            //{
            //    return BadRequest(new { message = "UserName or password is incorrect." });
            //}

            // Đã đăng nhập nên chuyển hướng về Index
            if (_signInManager.IsSignedIn(User)) return Redirect("/");

            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(loginModel.Email);
                Microsoft.AspNetCore.Identity.SignInResult result = new Microsoft.AspNetCore.Identity.SignInResult();
                if (user != null)
                {
                    result = await _signInManager.PasswordSignInAsync(
                        user,
                        loginModel.Password,
                        loginModel.RememberMe,
                        true
                    );
                }

                if (result.Succeeded)
                {
                    // Xóa giá trị của session ADMIN_GROUP
                    if (SessionHelpers.GetObjectFromJson<ApplicationUser>(HttpContext.Session, Constant.ADMIN_GROUP) != null)
                    {
                        SessionHelpers.SetObjectAsJson(HttpContext.Session, Constant.ADMIN_GROUP, null);
                    }

                    string urlRedirect = !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : "/";
                    _logger.LogInformation("User đã đăng nhập");
                    return ViewComponent(MessagePage.COMPONENTNAME, new MessagePage.Message()
                    {
                        title = "Đã đăng nhập",
                        htmlcontent = "Đăng nhập thành công",
                        urlredirect = urlRedirect
                    });
                }
                if (result.RequiresTwoFactor)
                {
                    // Nếu cấu hình đăng nhập hai yếu tố thì chuyển hướng đến LoginWith2fa
                    return RedirectToAction("LoginWith2fa", new { ReturnUrl = "/", RememberMe = loginModel.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("Tài khoản bí tạm khóa.");
                    // Chuyển hướng đến trang Lockout - hiện thị thông báo
                    return RedirectToAction("Logout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Không đăng nhập được.");
                    return View();
                }
            }
            return RedirectToAction("Index", "Homes");
        }
        #endregion

        #region Logout
        public async Task<IActionResult> Logout()
        {
            if (!_signInManager.IsSignedIn(User)) return RedirectToPage("/");

            await _signInManager.SignOutAsync();
            _logger.LogInformation("Người dùng đăng xuất");

            // Xoá giá trị trong Session ADMIN_GROUP
            SessionHelpers.SetObjectAsJson(HttpContext.Session, Constant.ADMIN_GROUP, null);

            return ViewComponent(MessagePage.COMPONENTNAME,
                new MessagePage.Message()
                {
                    title = "Đã đăng xuất",
                    htmlcontent = "Đăng xuất thành công",
                    urlredirect = "/"
                }
            );
        }
        #endregion

        [TempData]
        public string StatusMessage { get; set; }
        public async Task<IActionResult> ConfirmEmail(string userId, string token, bool isResetPass = false)
        {

            if (userId == null || token == null)
            {
                return Redirect("/");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Không tồn tại User - '{userId}'.");
            }

            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

            // Trường hợp reset password 
            if (isResetPass)
            {
                return ViewComponent(MessagePage.COMPONENTNAME,
                            new MessagePage.Message()
                            {
                                title = "Xác thực email",
                                htmlcontent = "Đang chuyển hướng đến trang thay đổi mật khẩu",
                                urlredirect = Url.Action("ResetPassword", "ApplicationUsers", new { userId = userId, token = token })
                            }
                        );
            }

            // Trường hợp đăng kí mới
            // Xác thực email
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                // Đăng nhập luôn nếu xác thực email thành công
                await _signInManager.SignInAsync(user, false);

                return ViewComponent(MessagePage.COMPONENTNAME,
                    new MessagePage.Message()
                    {
                        title = "Xác thực email",
                        htmlcontent = "Đã xác thực thành công, đang chuyển hướng",
                        urlredirect = Url.Action("Index", "Homes")
                    }
                );
            }
            else
            {
                StatusMessage = "Lỗi xác nhận email";
            }
            return View();
        }


        #region RegiterConfirmation
        public string Email { get; set; }

        public string UrlContinue { set; get; }

        public async Task<IActionResult> RegisterConfirmation(string email, string returnUrl = null)
        {
            if (email == null)
            {
                return RedirectToAction("/");
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Không có user với email: '{email}'.");
            }

            if (user.EmailConfirmed)
            {

                // Tài khoản đã xác thực email
                return ViewComponent(MessagePage.COMPONENTNAME,
                        new MessagePage.Message()
                        {
                            title = "Thông báo",
                            htmlcontent = "Tài khoản đã xác thực, chờ chuyển hướng",
                            urlredirect = (returnUrl != null) ? returnUrl : Url.Action("Index", "Homes")
                        }

                );
            }

            Email = email;

            if (returnUrl != null)
                UrlContinue = Url.Action("RegisterConfirmation", "ApplicationUsers", new { email = Email, returnUrl = returnUrl });
            else
                UrlContinue = Url.Action("RegisterConfirmation", "ApplicationUsers", new { email = Email });

            ViewBag.UrlContinue = UrlContinue;
            return View();
        }
        #endregion

        #region ResetPassword
        public IActionResult ForgetPassword()
        {
            ForgetPasswordInputModel model = new ForgetPasswordInputModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordInputModel inputModel)
        {
            var user = await _userManager.FindByEmailAsync(inputModel.Email);
            if (user != null)
            {
                // Send email confirm
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                var callbackUrl = Url.Action(
                    "ConfirmEmail", "ApplicationUsers",
                    values: new { userId = user.Id, token = token, isResetPass = true },
                    protocol: Request.Scheme
                );

                await _emailSender.SendEmailAsync(inputModel.Email, "Xác nhận địa chỉ email",
                $"Cảm ơn quý khách đã đến với 3CE Decor. Vui lòng xác nhận địa chỉ email bằng cách <a href='{callbackUrl}'>Bấm vào đây</a>.");

                if (_userManager.Options.SignIn.RequireConfirmedEmail)
                {
                    // Nếu cấu hình phải xác thực email mới được đăng nhập thì chuyển hướng đến trang
                    // ForgetPasswordConfirmation - chỉ để hiện thông báo cho biết người dùng cần mở email xác nhận
                    return RedirectToAction("ForgetPasswordConfirmation", new { email = user.Email });
                }
                else
                {
                    StatusMessage = "Lỗi xác nhận email";
                }
            }
            StatusMessage = "User không tồn tại";
            return Redirect("ForgetPassword");
        }
        public IActionResult ForgetPasswordConfirmation()
        {
            return View();
        }
        public IActionResult ResetPassword(string userId, string token)
        {
            ResetPasswordInputModel model = new ResetPasswordInputModel();
            model.UserId = userId;
            model.Token = token;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordInputModel inputModel)
        {
            if (!ModelState.IsValid)
            {
                return View(inputModel);
            }

            ApplicationUser user = await _userManager.FindByIdAsync(inputModel.UserId);
            if (user != null)
            {
                var resetPassResult = await _userManager.ResetPasswordAsync(user, inputModel.Token, inputModel.NewPassword);
                if (!resetPassResult.Succeeded)
                {
                    foreach (var error in resetPassResult.Errors)
                    {
                        ModelState.TryAddModelError(error.Code, error.Description);
                    }
                    return View();
                }
            }
            return Redirect("ResetPasswordConfirmation");
        }

        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        #endregion
    }
}
