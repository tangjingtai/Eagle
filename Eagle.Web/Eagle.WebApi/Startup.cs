﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using Util;
using Util.Datas.Dapper;
using Eagle.WebApi.EventHandlers.Events;
using AspectCore.Configuration;
using Eagle.WebApi.EventHandlers;
using Util.EventBus.RabbitMQ;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Eagle.WebApi.Common;
using Microsoft.AspNetCore.Authorization;
using Util.Logs.Extensions;

namespace Eagle.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                //options.Filters.Add( new AutoValidateAntiforgeryTokenAttribute() );
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddLogging(logginBuilder =>
            {
                logginBuilder.ClearProviders();
                logginBuilder.AddFilter("System", LogLevel.Warning);
                logginBuilder.AddFilter("Microsoft", LogLevel.Warning);
                logginBuilder.SetMinimumLevel(LogLevel.Trace); 
            });
            services.AddLog4net();
            //services.AddNLog();

            // 添加mysql数据库作为数据存储介质
            services.AddMySqlDatabase();
            // 添加couchbase作为缓存组件
            services.AddCouchBaseCache();
            //services.AddCouchBaseCache(config =>
            //{
            //    config.Urls = new List<string> { "http://192.168.1.90:8091/pools" };
            //    config.BucketAndPassword = new Dictionary<string, string> { { "Test", "123456" } };
            //});
            // 添加RabbitMQ作为事件总线组件
            services.AddEventBusOfRabbitMQ();
            // 注册不同的鉴权策略，适用不同的业务场景
            var permissionRequirement = new PermissionRequirement();
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

            services.AddAuthentication(authenOptions =>
            {
                authenOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authenOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
             {
                 // 添加使用JWT作为身份验证，验证未通过返回401
                 o.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidIssuer = JWTHelper.ISSUER,
                     ValidAudience = JWTHelper.AUDIENCE,
                     IssuerSigningKey = JWTHelper.SYMMTRIC_KEY
                 };
             });
            services.AddAuthorization(authorOptions => {
                authorOptions.AddPolicy("Permission", policy => policy.Requirements.Add(permissionRequirement));
            });

            //添加Swagger
            services.AddSwaggerGen(options => {
                //启用auth支持
                options.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", Enumerable.Empty<string>() }
                });

                options.SwaggerDoc("v1", new Info { Title = "Eagle Web Api", Version = "v1" });
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Util.xml"));
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Eagle.WebApi.xml"));
            });

            //添加Util基础设施服务
            var serviceProvider = services.AddUtil(aopConfig =>
            {
                // 配置AOP
                aopConfig.Interceptors.AddDelegate(async (content, next) =>
                {
                    Console.WriteLine("delegate interceptor");
                    await content.Invoke(next);
                });
            });


            // 注册事件处理器
            serviceProvider.RegisterBusHandlers(new RabbitMQConfig { Host = "localhost", UserName = "guest", Password = "guest" },
                configuration =>
                {
                    // TODO: 外置到其他地方进行注册
                    configuration.ConfigureHandler<TestEvent, TestEventHandler>("Test1");
                    configuration.ConfigureHandler<TestEvent2, TestEventHandler2>("Test2");
                });
            return serviceProvider;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseAuthentication();//配置授权
            //处理异常
            app.UseStatusCodePages(new StatusCodePagesOptions()
            {
                HandleAsync = (context) =>
                {
                    if (context.HttpContext.Response.StatusCode == 401)
                    {
                        using (StreamWriter sw = new StreamWriter(context.HttpContext.Response.Body))
                        {
                            sw.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new
                            {
                                status = 401,
                                message = "access denied!",
                            }));
                        }
                    }
                    return Task.Delay(0);
                }
            });

            app.UseHttpsRedirection();
            app.UseMvc(routes => {
                routes.MapRoute(
                   name: "Default",
                   template: "api/{controller}/{action}/{id?}",
                   defaults: new { controller = "Home", action = "Index" }
               );
            });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Eagle Web Api V1");
            });
        }
    }
}
