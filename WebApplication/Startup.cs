using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Core.Utilities;
using DAL.EntityFrameworkCore.Application;
using DAL.Models.Account;
using DAL.Repositories;
using DAL.Repositories.ArticleBlogRepository;
using DAL.Repositories.CartItemRepository;
using DAL.Repositories.CustomerRepository;
using DAL.Repositories.DiscountCodeRepository;
using DAL.Repositories.DistrictRepository;
using DAL.Repositories.GalleryRepository;
using DAL.Repositories.ImageRepository;
using DAL.Repositories.OrderDetailRepository;
using DAL.Repositories.OrderHistoryRepository;
using DAL.Repositories.OrderRepository;
using DAL.Repositories.ProductCategoryRepository;
using DAL.Repositories.ProductProductCategoryRepository;
using DAL.Repositories.ProductRepository;
using DAL.Repositories.ProvinceRepository;
using DAL.Repositories.StrategyProductRepository;
using DAL.Repositories.StrategyRepository;
using DAL.Repositories.WardRepository;
using DAL.UnitOfWorks.ArticleBlogService;
using DAL.UnitOfWorks.CustomerService;
using DAL.UnitOfWorks.DiscountCodeService;
using DAL.UnitOfWorks.DistrictService;
using DAL.UnitOfWorks.GalleryService;
using DAL.UnitOfWorks.ImageService;
using DAL.UnitOfWorks.MailService;
using DAL.UnitOfWorks.OrderDetailService;
using DAL.UnitOfWorks.OrderHistoryService;
using DAL.UnitOfWorks.OrderService;
using DAL.UnitOfWorks.ProductCategoryService;
using DAL.UnitOfWorks.ProductProductCategoryService;
using DAL.UnitOfWorks.ProductService;
using DAL.UnitOfWorks.ProvinceService;
using DAL.UnitOfWorks.StrategyProductService;
using DAL.UnitOfWorks.StrategyService;
using DAL.UnitOfWorks.WardService;
using DAL.Utility.Profile;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using WebApplication.BuildLink;

namespace WebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });

            services.AddResponseCaching();
            services.AddResponseCompression();

            services.AddRazorPages();
            // Vinhdq - Inject AppSettings
            services.Configure<ApplicationSettings>(Configuration.GetSection("ApplicationSettings"));

            // Vinhdq - Add NewtonsoftJson to serialize password when calling API if get error: The JSON value could not be converted to System.Int32
            // Install Package: Microsoft.AspNetCore.Mvc.NewtonsoftJson
            services.AddControllers().AddNewtonsoftJson();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddMvc(option => option.EnableEndpointRouting = false);

            // DBContext
            services.AddDbContext<ApplicationDbContext>(option =>
                option.UseSqlServer(Configuration.GetConnectionString("Default"))
            );

            services.Configure<SmtpSettings>(Configuration.GetSection("SmtpSettings"));
            services.AddSingleton<IMailer, Mailer>();

            //services.AddTransient<MySqlConnection>(_ => new MySqlConnection(Configuration["ConnectionStrings:Default"]));

            // Service registration
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient(typeof(IProductRepository), typeof(ProductRepository));
            services.AddTransient(typeof(IProductCategoryRepository), typeof(ProductCategoryRepository));
            services.AddTransient(typeof(IProductProductCategoryRepository), typeof(ProductProductCategoryRepository));
            services.AddTransient(typeof(IArticleBlogRepository), typeof(ArticleBlogRepository));
            services.AddTransient(typeof(ICustomerRepository), typeof(CustomerRepository));
            services.AddTransient(typeof(IImageRepository), typeof(ImageRepository));
            services.AddTransient(typeof(IOrderRepository), typeof(OrderRepository));
            services.AddTransient(typeof(IOrderDetailRepository), typeof(OrderDetailRepository));
            services.AddTransient(typeof(IOrderHistoryRepository), typeof(OrderHistoryRepository));
            services.AddTransient(typeof(ICartItemRepository), typeof(CartItemRepository));
            services.AddTransient(typeof(IDiscountCodeRepository), typeof(DiscountCodeRepository));
            services.AddTransient(typeof(IProvinceRepository), typeof(ProvinceRepository));
            services.AddTransient(typeof(IDistrictRepository), typeof(DistrictRepository));
            services.AddTransient(typeof(IWardRepository), typeof(WardRepository));
            services.AddTransient(typeof(IStrategyRepository), typeof(StrategyRepository));
            services.AddTransient(typeof(IStrategyProductRepository), typeof(StrategyProductRepository));
            services.AddTransient(typeof(IGalleryRepository), typeof(GalleryRepository));

            services.AddTransient<IProductService, ProductService>();
            services.AddTransient<IProductCategoryService, ProductCategoryService>();
            services.AddTransient<IProductProductCategoryService, ProductProductCategoryService>();
            services.AddTransient<IArticleBlogService, ArticleBlogService>();
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IOrderDetailService, OrderDetailService>();
            services.AddTransient<IOrderHistoryService, OrderHistoryService>();
            services.AddTransient<IBuildLinkProduct, BuildLinkProduct>();
            services.AddTransient<IBuildLinkArticleBlog, BuildLinkArticleBlog>();
            services.AddTransient<IBuildLinkStrategy, BuildLinkStrategy>();
            services.AddTransient<IBuildLinkSeo, BuildLinkSeo>();
            services.AddTransient<IDiscountCodeService, DiscountCodeService>();
            services.AddTransient<IProvinceService, ProvinceService>();
            services.AddTransient<IDistrictService, DistrictService>();
            services.AddTransient<IWardService, WardService>();
            services.AddTransient<IStrategyService, StrategyService>();
            services.AddTransient<IStrategyProductService, StrategyProductService>();
            services.AddTransient<IGalleryService, GalleryService>();

            // Vinhdq - Add Application Identity token to cookie 
            services.AddDefaultIdentity<ApplicationUser>()
                    .AddRoles<ApplicationRole>() 
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddSignInManager()
                    .AddDefaultTokenProviders();
            services.Configure<DataProtectionTokenProviderOptions>(opt =>
                    opt.TokenLifespan = TimeSpan.FromHours(2)
            );

            // Vinhdq - Tạo policy yêu cầu đăng nhập, nếu không sẽ có lỗi
            //services.AddMvc(option => {
            //    var policy = new AuthorizationPolicyBuilder()
            //                            .RequireAuthenticatedUser()
            //                            .Build();
            //    option.Filters.Add(new AuthorizeFilter(policy));
            //}).AddXmlSerializerFormatters();

            // Configure rules for Password
            services.Configure<IdentityOptions>(option =>
            {
                // Password settings
                option.Password.RequireDigit = false;
                option.Password.RequireNonAlphanumeric = false;
                option.Password.RequireLowercase = false;
                option.Password.RequireUppercase = false;
                option.Password.RequiredLength = 6;

                // Lockout settings.
                option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                option.Lockout.MaxFailedAccessAttempts = 5;
                option.Lockout.AllowedForNewUsers = true;

                // User settings.
                option.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                option.User.RequireUniqueEmail = true;

                // Sign in settings.
                option.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
                option.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/ApplicationUsers/Login";
                options.AccessDeniedPath = "/ApplicationsUsers/AccessDenied";
                options.SlidingExpiration = true;
            });

            services.AddOptions();                                        // Kích hoạt Options
            var mailsettings = Configuration.GetSection("MailSettings");  // đọc config
            services.Configure<MailSettings>(mailsettings);               // đăng ký để Inject

            //services.AddTransient<IEmailSender, SendMailService>();        // Đăng ký dịch vụ Mail

            services.AddCors();

            // Register AutoMapper
            var configure = new AutoMapper.MapperConfiguration(c => {
                c.AddProfile(new ApplicationProfile());
            });

            var mapper = configure.CreateMapper();
            services.AddSingleton(mapper);

            // JWT Authentication
            //Random Key
            var key = Encoding.UTF8.GetBytes(Configuration["ApplicationSettings:JWT_Secret"].ToString());

            //services.AddAuthentication(x =>
            //{
            //    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            //}).AddJwtBearer(x => {
            //    x.RequireHttpsMetadata = false;
            //    x.SaveToken = false;
            //    x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            //    {
            //        ValidateIssuerSigningKey = true,
            //        IssuerSigningKey = new SymmetricSecurityKey(key),
            //        ValidateIssuer = false,
            //        ValidateAudience = false,
            //        ClockSkew = TimeSpan.Zero
            //    };
            //});

            // Session for Cart
            services.AddDistributedMemoryCache();
            services.AddSession(cfg => {
                cfg.Cookie.Name = "3CECartSessionId";
                cfg.IdleTimeout = new TimeSpan(0, 30, 0);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext context)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                context.Database.EnsureCreated();
                app.UseForwardedHeaders();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseForwardedHeaders();
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseResponseCompression();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();

            //app.UseCors();
            //app.UseCors(builder =>
            //    builder.WithOrigins(Configuration["ApplicationSettings:Client_URL"].ToString())
            //           .AllowAnyHeader()
            //           .AllowAnyMethod()
            //);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "AdminArea",
                    pattern: "{area:exists}/{controller=ProductAdmin}/{action=ProductList}/{id?}"
                );

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Homes}/{action=Index}/{id?}");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "Login",
                    template: "login",
                    defaults: new { controller = "ApplicationUsers", action = "Login" }
                );
                routes.MapRoute(
                    name: "Logout",
                    template: "logout",
                    defaults: new { controller = "ApplicationUsers", action = "Logout" }
                );
                routes.MapRoute(
                    name: "Register",
                    template: "register",
                    defaults: new { controller = "ApplicationUsers", action = "Register" }
                );
                routes.MapRoute(
                    name: "ForgetPassword",
                    template: "forget-pass",
                    defaults: new { controller = "ApplicationUsers", action = "ForgetPassword" }
                );
                routes.MapRoute(
                    name: "ResetPassword",
                    template: "reset-pass",
                    defaults: new { controller = "ApplicationUsers", action = "ResetPassword" }
                );
                routes.MapRoute(
                    name: "ConfirmEmail",
                    template: "confirm-email",
                    defaults: new { controller = "ApplicationUsers", action = "ConfirmEmail" }
                );
                routes.MapRoute(
                    name: "ProductList",
                    template: "product-for-sale",
                    defaults: new { controller = "Products", action = "ProductList" }
                );
                routes.MapRoute(
                    name: "ProductListPaging",
                    template: "product-for-sale/p{pageIndex}",
                    defaults: new { controller = "Products", action = "ProductList" }
                );

                #region Product Search
                // Lưu ý: Đặt theo thứ tự các chỉ tiêu seach từ nhiều nhất đến ít nhất để tránh bị đá link sai
                routes.MapRoute(
                    name: "ProductSerchAllFilter",
                    template: "product-search/{categoryName}/text-{textSearch}/price-{priceMin}-{priceMax}",
                    defaults: new { controller = "Products", action = "ProductSearch" }
                );
                routes.MapRoute(
                    name: "ProductSerchAllFilterPaging",
                    template: "product-search/{categoryName}/text-{textSearch}/price-{priceMin}-{priceMax}/p{pageIndex}",
                    defaults: new { controller = "Products", action = "ProductSearch" }
                );

                routes.MapRoute(
                    name: "ProductSerchByPrice",
                    template: "product-search/{categoryName}/price-{priceMin}-{priceMax}",
                    defaults: new { controller = "Products", action = "ProductSearch" }
                );
                routes.MapRoute(
                    name: "ProductSerchByPricePaging",
                    template: "product-search/{categoryName}/price-{priceMin}-{priceMax}/p{pageIndex}",
                    defaults: new { controller = "Products", action = "ProductSearch" }
                );

                routes.MapRoute(
                    name: "ProductTextSearch",
                    template: "product-search/{categoryName}/text-{textSearch}",
                    defaults: new { controller = "Products", action = "ProductSearch" }
                );
                routes.MapRoute(
                    name: "ProductTextSearchPaging",
                    template: "product-search/{categoryName}/text-{textSearch}/p{pageIndex}",
                    defaults: new { controller = "Products", action = "ProductSearch" }
                );

                routes.MapRoute(
                    name: "ProductSearchCategory",
                    template: "product-search/{categoryName}",
                    defaults: new { controller = "Products", action = "ProductSearch" }
                );
                routes.MapRoute(
                    name: "ProductSearchCategoryPaging",
                    template: "product-search/{categoryName}/p{pageIndex}",
                    defaults: new { controller = "Products", action = "ProductSearch" }
                );
                #endregion

                routes.MapRoute(
                    name: "ProductDetail",
                    template: "product/{title}/pid-{id}",
                    defaults: new { controller = "Products", action = "ProductDetail" }
                );
                routes.MapRoute(
                    name: "ArticleList",
                    template: "article-blog-list",
                    defaults: new { controller = "ArticleBlogs", action = "ArticleBlogList" }
                );
                routes.MapRoute(
                    name: "ArticleListPaging",
                    template: "article-blog-list/p{pageIndex}",
                    defaults: new { controller = "ArticleBlogs", action = "ArticleBlogList" }
                );
                routes.MapRoute(
                    name: "ArticleSearchNoTextSearch",
                    template: "article-search",
                    defaults: new { controller = "ArticleBlogs", action = "ArticleBlogSearch" }
                );
                routes.MapRoute(
                    name: "ArticleSearch",
                    template: "article-search/{textSearch}",
                    defaults: new { controller = "ArticleBlogs", action = "ArticleBlogSearch" }
                );
                routes.MapRoute(
                    name: "ArticleSearchPaging",
                    template: "article-search/{textSearch}/p{pageIndex}",
                    defaults: new { controller = "ArticleBlogs", action = "ArticleBlogSearch" }
                );
                routes.MapRoute(
                    name: "ArticleDetail",
                    template: "article-blog/{title}/aid-{id}",
                    defaults: new { controller = "ArticleBlogs", action = "ArticleBlogDetail" }
                );
                routes.MapRoute(
                    name: "ShoppingCart",
                    template: "shopping-cart",
                    defaults: new { controller = "CartItems", action = "Index" }
                );
                routes.MapRoute(
                    name: "AddToCart",
                    template: "add-to-cart/c{id}",
                    defaults: new { controller = "CartItems", action = "AddToCart" }
                );
                routes.MapRoute(
                    name: "RemoveCartItem",
                    template: "remove/c{id}",
                    defaults: new { controller = "CartItems", action = "Remove" }
                );
                routes.MapRoute(
                    name: "Payments",
                    template: "check-out",
                    defaults: new { controller = "Payments", action = "Index" }
                );
                routes.MapRoute(
                    name: "PaymentGenerateDistrict",
                    template: "check-out/gen-district",
                    defaults: new { controller = "Payments", action = "GenerateDistrict" }
                );
                routes.MapRoute(
                    name: "PaymentGenerateWard",
                    template: "check-out/gen-ward",
                    defaults: new { controller = "Payments", action = "GenerateWard" }
                );
                routes.MapRoute(
                    name: "PaymentCheckDiscountCode",
                    template: "check-out/checkDiscountCode",
                    defaults: new { controller = "Payments", action = "CheckDiscountCode" }
                );
                routes.MapRoute(
                    name: "PaymentsOrder",
                    template: "order-info",
                    defaults: new { controller = "Payments", action = "GetOrderInfo" }
                );
                routes.MapRoute(
                    name: "ConfirmOrderPaymentOnline",
                    template: "confirm_orderPaymentOnline",
                    defaults: new { controller = "Payments", action = "Confirm_orderPaymentOnline" }
                );
                routes.MapRoute(
                    name: "CancelOrder",
                    template: "cancel_order",
                    defaults: new { controller = "Payments", action = "Cancel_order" }
                );
                #region Strategy
                routes.MapRoute(
                    name: "StrategyDetail",
                    template: "strategy-{strategyName}/sid-{strategyId}",
                    defaults: new { controller = "Strategies", action = "StrategyDetail" }
                );
                routes.MapRoute(
                    name: "StrategyDetailPaging",
                    template: "strategy-{strategyName}/sid-{strategyId}/p{pageIndex}",
                    defaults: new { controller = "Strategies", action = "StrategyDetail" }
                );
                #endregion
                routes.MapRoute(
                    name: "HomeIndex",
                    template: "home-page",
                    defaults: new { controller = "Homes", action = "Index" }
                );
            });
        }
    }
}
