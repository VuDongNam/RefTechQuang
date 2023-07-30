using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyTemplate.Data;
using MyTemplate.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyTemplate
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

      services.AddDbContext<AppDbContext>(o =>
      {
        o.UseSqlServer(Configuration.GetConnectionString("MyTemplateConnectionString"));
      });

      services.AddIdentity<AppUser, IdentityRole>()
          .AddEntityFrameworkStores<AppDbContext>()
          .AddDefaultTokenProviders();

      // Truy cập IdentityOptions
      services.Configure<IdentityOptions>(options =>
      {
        // Thiết lập về Password
        options.Password.RequireDigit = false; // Không bắt phải có số
        options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
        options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
        options.Password.RequireUppercase = false; // Không bắt buộc chữ in
        options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
        options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

        // Cấu hình Lockout - khóa user
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
        options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lầ thì khóa
        options.Lockout.AllowedForNewUsers = true;

        // Cấu hình về User.
        options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        options.User.RequireUniqueEmail = true; // Email là duy nhất

        // Cấu hình đăng nhập.
        options.SignIn.RequireConfirmedEmail = false; // Cấu hình xác thực địa chỉ email (email phải tồn tại)
        options.SignIn.RequireConfirmedPhoneNumber = false; // Xác thực số điện thoại
      });

      // Cấu hình Cookie
      services.ConfigureApplicationCookie(options =>
      {
        // options.Cookie.HttpOnly = true;  
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.LoginPath = $"/Identity/Account/Login/";                                 // Url đến trang đăng nhập
        options.LogoutPath = $"/Identity/Account/logout/";
        options.AccessDeniedPath = $"/Identity/Account/AccessDenied";   // Trang khi User bị cấm truy cập
      });
      services.Configure<SecurityStampValidatorOptions>(options =>
      {
              // Trên 5 giây truy cập lại sẽ nạp lại thông tin User (Role)
              // SecurityStamp trong bảng User đổi -> nạp lại thông tin Security
              options.ValidationInterval = TimeSpan.FromSeconds(5);
      });

      services.AddAuthorization(o => {
        o.AddPolicy("AdminDropdown", policy => {
          policy.RequireRole("Admin");
        });
      });

      services.AddOptions();

      var mailsettings = Configuration.GetSection("MailSettings");  // đọc config
      services.Configure<MailSettings>(mailsettings);               // đăng ký để Inject

      services.AddTransient<IEmailSender, SendMailService>();       // Đăng ký dịch vụ Mail


      // for session
      services.AddDistributedMemoryCache();      // Đăng ký dịch vụ lưu cache trong bộ nhớ (Session sẽ sử dụng nó)
      services.AddSession(cfg => {               // Đăng ký dịch vụ Session
        cfg.Cookie.Name = "fptaptech";           // Đặt tên Session - tên này sử dụng ở Browser (Cookie)
        cfg.IdleTimeout = new TimeSpan(0, 30, 0);// Thời gian tồn tại của Session
      });

      services.AddRazorPages();
      services.AddControllersWithViews();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
      }
      app.UseStaticFiles();

      // for session
      app.UseSession();

      app.UseRouting();

      app.UseAuthentication();    // đăng nhập

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
          name: "MyArea",
          pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

        endpoints.MapControllerRoute(
          name: "default",
          pattern: "{controller=Home}/{action=Index}/{id?}");
                
        // for razor page
        endpoints.MapRazorPages();
      });
    }
  }
}
