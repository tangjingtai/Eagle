using System;
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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Eagle.WebApi.Common;
using Microsoft.AspNetCore.Authorization;
using Util.Logs.Extensions;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Hosting;
using Util.ServiceDiscovery;
using IdentityModel;

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
        public System.IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //services.AddMvc(options =>
            //{
            //    //options.Filters.Add( new AutoValidateAntiforgeryTokenAttribute() );
            //})
            //.SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddControllers();
            services.AddLogging(logginBuilder =>
            {
                logginBuilder.ClearProviders();
                logginBuilder.AddFilter("System", LogLevel.Warning);
                logginBuilder.AddFilter("Microsoft", LogLevel.Warning);
                logginBuilder.SetMinimumLevel(LogLevel.Information); 
            });
            services.AddLog4net();
            //services.AddNLog();

            services.AddConsul();

            // 添加mysql数据库作为数据存储介质
            services.AddMySqlDatabase();
            // 添加couchbase作为缓存组件
            services.AddCouchBaseCache();            
            // 添加RabbitMQ作为事件总线组件
            services.AddEventBusOfRabbitMQ();
            // 注册认证配置
            services.AddAuthentication(authenOptions =>
            {
                authenOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authenOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // 使用JWT Bearer做认证，验证 Issuer、Audience、token是否过期、JWT的签名信息
            // 签名使用IssuerSigningKey指定的key
            .AddJwtBearer(o =>
             {
                 o.Events.OnMessageReceived += context => { return null; };
                 // 添加使用JWT作为身份验证，验证未通过返回401
                 o.TokenValidationParameters = new TokenValidationParameters
                 {
                     NameClaimType = JwtClaimTypes.Name,
                     RoleClaimType = JwtClaimTypes.Role,
                     ValidIssuer = JWTHelper.ISSUER,
                     ValidAudience = JWTHelper.AUDIENCE,
                     IssuerSigningKey = JWTHelper.SYMMTRIC_KEY
                 };
             });

            // 注册鉴权配置，认证通过之后才会进行鉴权
            // 注册不同的鉴权策略，适用不同的业务场景
            var permissionRequirement = new PermissionRequirement();
            // 注册鉴权的实现类
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();      
            services.AddAuthorization(authorOptions => {
                // 指定一个鉴权的策略
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
            services.AddModuleService();

            //添加Util基础设施服务
            var serviceProvider = services.AddUtil(aopConfig =>
            {
                // 配置AOP
                aopConfig.Interceptors.AddDelegate(async (context, next) =>
                {
                    Console.WriteLine($"delegate interceptor:{context.ProxyMethod.Name}");
                    await context.Invoke(next);
                });
            });
            return serviceProvider;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,  IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            //app.UseCouchBaseCache(config =>
            //{
            //    // TODO: 可从配置文件、数据库中读取配置
            //    config.Urls = new List<string> { "http://192.168.1.90:8091/pools" };
            //    config.BucketAndPassword = new Dictionary<string, string> { { "Test", "123456" } };
            //});

            //app.UseEventBusOfRabbitMQ(new RabbitMQConfig { Host = Configuration.GetValue<string>("RabbitMQ:Host"), UserName = "guest", Password = "guest" },
            //    configuration =>
            //    {
            //        // TODO: 外置到其他地方进行注册
            //        // 可以查询数据库、读取配置文件
            //        configuration.ConfigureHandler<TestEvent, TestEventHandler>("Test1");
            //        configuration.ConfigureHandler<TestEvent2, TestEventHandler2>("Test2");
            //    });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();  //配置认证
            // 处理http异常
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

            //app.UseMvc(routes => {
            //    routes.MapRoute(
            //       name: "Default",
            //       template: "api/{controller}/{action}/{id?}",
            //       defaults: new { controller = "Home", action = "Index" }
            //   );
            //});
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapAreaControllerRoute(
                    name: "areas", "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Eagle Web Api V1");
            });
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
        }
    }
}
