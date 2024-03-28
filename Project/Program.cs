using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Mapper;
using Project.Models;
using Project.Repositories;
using Project.ViewModels;

namespace Project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<BookStoreContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("BookStore"));
            });

            //Register Identity Service (userManager -roleMnager- SigninManager)
            builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
            {
                options.User.AllowedUserNameCharacters = null;
                options.User.RequireUniqueEmail = true;

            }).AddEntityFrameworkStores<BookStoreContext>();

            builder.Services.AddScoped<IBookRepository, BookRepository>();
            builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICommentRepository, CommentRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderDetailsRepository, OrderDetailsRepository>();
            builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
            builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();

            builder.Services.AddAutoMapper(typeof(MapperProfile));


            var app = builder.Build();

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

            app.UseAuthorization();
            app.MapControllerRoute(
                "Login", "Login", new { Controller = "Account", Action = "Login" }
               );
            app.MapControllerRoute(
              "Register", "Register", new
              {
                  Controller = "Account",
                  Action = "Register"
              }

             );
            app.MapControllerRoute(
              "Home",
              "Home",
              new { Controller = "Home", Action = "Index" }
              );
            app.MapControllerRoute(
              "Home",
              "Cart",
              new { Controller = "Home", Action = "Cart" }
              );
            app.MapControllerRoute(
                "ContactUs",
                "Contact",
                new { Controller = "Contact", Action = "Index" }
                );


            app.MapControllerRoute(
                name: "default",

                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
