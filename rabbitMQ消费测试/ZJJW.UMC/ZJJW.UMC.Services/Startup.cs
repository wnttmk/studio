using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ZJJWEPlatform.DataTransfer;

namespace ZJJW.UMC.Services
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
            services.AddControllers();

            // 添加消息队例配置
            services.AddDataTransfer(Configuration, opt =>
            {
                opt.ExceptionHandler += (ex) =>
                {

                };

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
            app.UseDataTransfer(logger.CreateLogger<IDataTransfer>());

            var dt = app.ApplicationServices.GetRequiredService<ZJJWEPlatform.DataTransfer.IDataTransfer>();
            var i = 0;
            do
            {
                dt.SendDatas("ZJJW.UMC.Client", new MessageBase()
                {
                    Seed = i,
                    SendTime = DateTime.Now,
                    Sender = "ZJJW.UMC.Services"
                });
                i++;
            } while (i < 100);
        }
    }
}
