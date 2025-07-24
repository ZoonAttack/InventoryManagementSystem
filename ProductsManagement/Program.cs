using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProductsManagement.Context;
using ProductsManagement.Data;

namespace ProductsManagement
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddIdentity<User, IdentityRole>()
                            .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            });
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddControllers();
            builder.Services.AddControllersWithViews();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            //using (var scope = app.Services.CreateScope())
            //{
            //    var services = scope.ServiceProvider;
            //    await SeedRolesAsync(services);
            //}
            app.Run();
        }
        static async Task SeedRolesAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<User>>();
            string[] roles = ["user", "admin"];

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            //Initialize chef
            string adminEmail = "admin1@gmail.com";
            string adminPassword = "admin123";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                User user = new User()
                {
                    UserName = adminEmail,
                    Email = adminEmail
                };
                await userManager.CreateAsync(user, adminPassword);
                userManager.AddToRoleAsync(user, "admin").Wait();
            }
        }
    }
}
