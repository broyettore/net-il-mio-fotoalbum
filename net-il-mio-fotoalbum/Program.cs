using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using net_il_mio_fotoalbum.Database;
using System.Text.Json.Serialization;

namespace net_il_mio_fotoalbum
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<PhotoContext>();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false).AddRoles<IdentityRole>().AddEntityFrameworkStores<PhotoContext>();


         
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowMyLocalHost",
                      builder =>
                      {
                          builder.WithOrigins("http://localhost:3000")
                                 .AllowAnyMethod()
                                 .AllowAnyHeader();
                      });
            });

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Codice di cofigurazione per il serializzatore JSON, in modo che ignori completamente le dipendenze cicliche di
            // eventuali relazione N:N o 1:N presenti nel JSON risultante.
            builder.Services.AddControllers().AddJsonOptions(x =>
                            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            // Dependency injection
            builder.Services.AddScoped<PhotoContext, PhotoContext>();

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

            app.UseCors("AllowMyLocalHost");

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Photo}/{action=UserIndex}/{id?}");


            app.MapRazorPages();

            app.Run();
        }
    }
}