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
using Util.Webs.Extensions;
using Util.Datas.Dapper;
using Util.Caches;

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

            services.AddMySqlDatabase();
            services.UseCouchbaseCache(config =>
            {
                config.Urls = new List<string> { "http://192.168.1.90:8091/pools" };
                config.BucketAndPassword = new Dictionary<string, string> { { "Test", "123456" } };
            });

            //注册XSRF令牌服务
            services.AddXsrfToken();

            //添加Swagger
            services.AddSwaggerGen(options => {
                options.SwaggerDoc("v1", new Info { Title = "Util Web Api Demo", Version = "v1" });
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Util.xml"));
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Util.Webs.xml"));
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Eagle.WebApi.xml"));
            });

            //添加Util基础设施服务
            return services.AddUtil();
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

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
