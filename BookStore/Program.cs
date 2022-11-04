using BookStore.Core.Contracts;
using BookStore.Core.Services;
using BookStore.Infrastructure;
using BookStore.Infrastructure.Common.Repositories;
using BookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                //TODO: Implement account confirmation
                options.SignIn.RequireConfirmedAccount = builder.Configuration.GetValue<bool>("Identity:RequireConfirmedAccount");
                options.SignIn.RequireConfirmedEmail = builder.Configuration.GetValue<bool>("Identity:RequireConfirmedEmail");
                options.SignIn.RequireConfirmedPhoneNumber = builder.Configuration.GetValue<bool>("Identity:RequireConfirmedPhoneNumber");
                options.Password.RequireNonAlphanumeric = builder.Configuration.GetValue<bool>("Identity:RequireNonAlphanumeric");
                options.Password.RequiredLength = builder.Configuration.GetValue<int>("Identity:RequiredLength");
            })
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/User/Login";
            });

            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<IAuthorService, AuthorService>();
            builder.Services.AddScoped<IBookService, BookService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IPublisherService, PublisherService>();
            builder.Services.AddScoped<IWarehouseService, WarehouseService>();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(DeletableEntityRepository<>));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
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