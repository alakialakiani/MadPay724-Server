﻿using ImageResizer.AspNetCore.Helpers;
using MadPay724.Data.DatabaseContext;
using MadPay724.Data.Dtos.Api;
using MadPay724.Services.Seed.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MadPay724.Api.Helpers.Configuration
{
    public static class InitConfigurationExtensions
    {
        public static void AddMadDbContext(this IServiceCollection services)
        {
            services.AddDbContext<Main_MadPayDbContext>();
            services.AddDbContext<Financial_MadPayDbContext>();
            services.AddDbContext<Log_MadPayDbContext>();
        }
        public static void AddMadInitialize(this IServiceCollection services, int? httpsPort)
        {
            services.AddMvcCore(config =>
            {
                config.EnableEndpointRouting = false;
                config.ReturnHttpNotAcceptable = true;
                //config.SslPort = httpsPort;
                config.Filters.Add(typeof(RequireHttpsAttribute));
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
                //var jsonFormatter = config.OutputFormatters.OfType<JsonOutputFormatter>().Single();
                //config.OutputFormatters.Remove(jsonFormatter);
                //config.OutputFormatters.Add(new IonOutputFormatter(jsonFormatter));
                //config.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                //config.InputFormatters.Add(new XmlSerializerInputFormatter(config));
            })
             .AddApiExplorer()
             .AddFormatterMappings()
             .AddDataAnnotations()
             .AddCors(opt =>
                                 opt.AddPolicy("CorsPolicy", builder =>
                                builder.WithOrigins("http://localhost:4200")
                                        .AllowAnyMethod()
                                        .AllowAnyHeader()
                                        .AllowCredentials()))
             .AddNewtonsoftJson(opt =>
                {
                    opt.SerializerSettings.ReferenceLoopHandling =
                    Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });
            //Custom ModelState Error
            services.Configure<ApiBehaviorOptions>(opt =>
            {
                opt.InvalidModelStateResponseFactory = context =>
                {
                    var strErrorList = new List<string>();
                    var msErrors = context.ModelState.Where(p => p.Value.Errors.Count > 0);
                    foreach (var msError in msErrors)
                    {
                        foreach (var error in msError.Value.Errors)
                        {
                            strErrorList.Add(error.ErrorMessage);
                        }
                    }
                    var errorModel = new GateApiReturn<string>
                    {
                        Status = false,
                        Messages = strErrorList.ToArray(),
                        Result = null
                    };
                    return new BadRequestObjectResult(errorModel);
                };
            });
            //
            services.AddResponseCaching();
            services.AddHsts(opt =>
            {
                opt.MaxAge = TimeSpan.FromDays(180);
                opt.IncludeSubDomains = true;
                opt.Preload = true;
            });
            //services.AddResponseCompression(opt => opt.Providers.Add<GzipCompressionProvider>());
            //services.AddRouting( opt => opt.LowercaseUrls = true);
            //services.AddApiVersioning(opt =>
            //{
            //    opt.ApiVersionReader = new MediaTypeApiVersionReader();
            //    opt.AssumeDefaultVersionWhenUnspecified = true;
            //    opt.ReportApiVersions = true;
            //    opt.DefaultApiVersion = new ApiVersion(1,0);
            //    opt.ApiVersionSelector = new CurrentImplementationApiVersionSelector(opt);
            //});
            services.AddImageResizer();
        }

        public static void UseMadInitialize(this IApplicationBuilder app)
        {
            //app.UseResponseCompression();
            app.UseRouting();
            app.UseImageResizer();

            app.UseStaticFiles(new StaticFileOptions()
            {
                RequestPath = new PathString("/wwwroot")
            });
        }

        public static void UseMadInitializeInProd(this IApplicationBuilder app)
        {
            app.UseHsts();
            app.UseHttpsRedirection();
            app.UseResponseCaching();
        }
    }
}