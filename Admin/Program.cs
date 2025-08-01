using ProductsManagement.Models;

namespace Admin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpClient<ApiCalls>();

            builder.Services.AddSession();
            builder.Services.AddHttpContextAccessor();

            builder.Services.AddAuthentication("MyCookieAuth")
                          .AddCookie("MyCookieAuth", options =>
                          {
                              options.LoginPath = "/Admin/Login";
                              options.LogoutPath = "/Admin/Logout";
                              //options.AccessDeniedPath = "/Account/AccessDenied";
                              options.Cookie.HttpOnly = true;
                              options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                          });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            app.UseSession();
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
