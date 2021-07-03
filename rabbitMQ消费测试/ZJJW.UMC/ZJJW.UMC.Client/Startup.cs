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
using ZJJW.UMC.Client.Middleware;
using ZJJW.UMC.Client.MQEventHandler;
using ZJJWEPlatform.DataTransfer;

namespace ZJJW.UMC.Client
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

            services.AddServicesOptions(Configuration);


            // 添加消息队例配置
            services.AddDataTransfer(Configuration, opt =>
            {
                opt.ExceptionHandler += (ex) =>
                {

                };
                // 接收数据并处理
                opt.ReceiveDataHandler += ReceiveDataHandler;
            });
        }

        /// <summary>
        /// 消息提收事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private ReceiveResult ReceiveDataHandler(object sender, EventArgs e, MessageBase message)
        {

            var logger = this.Logger.CreateLogger("ReceiveDataHandler");
            var service = app.ApplicationServices.GetRequiredService<MsgReceive>();
            ReceiveResult result = service.Execute(message.Seed, message.Content);
            logger.LogInformation(Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                seed = message.Seed,
                result
            }));
            return result;
        }

        IApplicationBuilder app;

        ILoggerFactory Logger;

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory logger)
        {
            this.app = app;
            this.Logger = logger;
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
        }
    }
}
