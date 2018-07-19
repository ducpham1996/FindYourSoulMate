using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FindYourSoulMate.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace FindYourSoulMate
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
            services.AddAuthentication("User")
        .AddCookie("User", options =>
        {
            options.ExpireTimeSpan = TimeSpan.FromDays(30);
            options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
            options.Cookie.Name = "User";
            options.LoginPath = "/Users/Login/";
            options.LogoutPath = "/Users/Logout";
            options.SlidingExpiration = true;
        });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("User", policy => policy.RequireClaim("User"));

            });
            services.AddMvc();
            services.AddDbContext<FindYourSoulMateContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("FindYourSoulMateContext")));

            //services.ConfigureApplicationCookie(options =>
            //{
            //    //options.Cookie.Expiration = TimeSpan.FromDays(150);
            //    //options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
            //    options.Cookie.Name = "MyCookieName";
            //    options.LoginPath = "/Users/Login";
            //    options.SlidingExpiration = true;
            //    //options.ExpireTimeSpan = TimeSpan.FromDays(14);
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Posts}/{action=Index}/{id?}");
            });
        }
    }
}
