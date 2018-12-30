using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Util;
using Util.Datas.Dapper;
using Util.Caches;
using Eagle.WebApi.EventHandlers.Events;
using Util.Helpers;
using AspectCore.Configuration;
using Eagle.WebApi.EventHandlers;
using Util.EventBus.RabbitMQ;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using IdentityModel;
using Eagle.WebApi.Common;

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
                logginBuilder.AddLog4Net("log4net.config");
            });

            // 添加mysql数据库作为数据存储介质
            services.AddMySqlDatabase();
            // 添加couchbase作为缓存组件
            services.AddCouchbaseCache(config =>
            {
                config.Urls = new List<string> { "http://192.168.1.90:8091/pools" };
                config.BucketAndPassword = new Dictionary<string, string> { { "Test", "123456" } };
            });
            // 添加RabbitMQ作为事件总线组件
            services.AddEventBusOfRabbitMQ(config =>
            {
                config.Host = "localhost";
                config.UserName = "guest";
                config.Password = "guest";
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(o =>
                    {
                        o.TokenValidationParameters = new TokenValidationParameters
                        {
                            NameClaimType = JwtClaimTypes.Name,
                            RoleClaimType = JwtClaimTypes.Role,

                            ValidIssuer = ConstantFactory.ValidIssuer,
                            ValidAudience = ConstantFactory.ValidAudience,
                            IssuerSigningKey = ConstantFactory.SymmetricKey
                            /***********************************TokenValidationParameters的参数默认值***********************************/
                            // RequireSignedTokens = true,
                            // SaveSigninToken = false,
                            // ValidateActor = false,
                            // 将下面两个参数设置为false，可以不验证Issuer和Audience，但是不建议这样做。
                            // ValidateAudience = true,
                            // ValidateIssuer = true, 
                            // ValidateIssuerSigningKey = false,
                            // 是否要求Token的Claims中必须包含Expires
                            // RequireExpirationTime = true,
                            // 允许的服务器时间偏移量
                            // ClockSkew = TimeSpan.FromSeconds(300),
                            // 是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
                            // ValidateLifetime = true
                        };
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

                options.SwaggerDoc("v1", new Info { Title = "Eagle Web Api Demo", Version = "v1" });
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Util.xml"));
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Util.Webs.xml"));
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
                        using (System.IO.StreamWriter sw = new System.IO.StreamWriter(context.HttpContext.Response.Body))
                        {
                            sw.Write(Newtonsoft.Json.JsonConvert.SerializeObject(new
                            {
                                status = 401,
                                message = "access denied!",
                            }));
                        }
                    }
                    return System.Threading.Tasks.Task.Delay(0);
                }
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
