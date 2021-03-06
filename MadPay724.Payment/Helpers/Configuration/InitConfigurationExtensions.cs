﻿using MadPay724.Data.DatabaseContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MadPay724.Payment.Helpers.Configuration
{
    public static class InitConfigurationExtensions
    {
        public static void AddMadDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            var con = configuration.GetSection("ConnectionStrings");

            services.AddDbContext<Main_MadPayDbContext>(opt => {
                opt.UseSqlServer(con.GetSection("Main").Value);
            });
            services.AddDbContext<Financial_MadPayDbContext>(opt => {
                opt.UseSqlServer(con.GetSection("Financial").Value);
            });
            services.AddDbContext<Log_MadPayDbContext>();
        }
        public static void AddMadInitialize(this IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddMvcCore(config =>
            {
                config.ReturnHttpNotAcceptable = true;
                config.Filters.Add(typeof(RequireHttpsAttribute));
            });

            services.AddHsts(opt =>
            {
                opt.MaxAge = TimeSpan.FromDays(180);
                opt.IncludeSubDomains = true;
                opt.Preload = true;
            });

            services.AddHttpsRedirection(opt =>
            {
                opt.RedirectStatusCode = StatusCodes.Status302Found;
            });

            services.AddResponseCaching();
           
        }

        public static void UseMadInitialize(this IApplicationBuilder app)
        {
            //app.UseResponseCompression();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseStaticFiles();

            app.UseCsp(opt => opt.DefaultSources(s => s.Self())
            .StyleSources(s => s.Self().UnsafeInline())
            .ScriptSources(s => s.Self().UnsafeInline())
            .ImageSources(s => s.Self().CustomSources("res.cloudinary.com", "cloudinary.com", "data:"))
            .MediaSources(s => s.Self().CustomSources("res.cloudinary.com", "cloudinary.com", "data:"))
            .FontSources(s => s.Self().CustomSources("data:"))
            );
            app.UseXfo(o => o.Deny());
        }

        public static void UseMadInitializeInProd(this IApplicationBuilder app)
        {
            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseResponseCaching();
        }
    }
}
