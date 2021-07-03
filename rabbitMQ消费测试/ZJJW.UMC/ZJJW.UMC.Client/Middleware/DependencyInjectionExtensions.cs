using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZJJW.UMC.Client.MQEventHandler;
using ZJJW.UMC.Client.Services;

namespace ZJJW.UMC.Client.Middleware
{
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// 添加服务 选项配置
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="redisAction"></param>
        /// <param name="dbAction"></param>
        public static void AddServicesOptions(this IServiceCollection services, IConfiguration configuration, Action<Options.RedisOptions> redisAction = null

            )
        {



            services.Configure<Options.RedisOptions>(configuration.GetSection("RedisOptions"));
            if (redisAction != null)
            {
                services.Configure(redisAction);
            }

            services.AddSingleton(typeof(RedisCacheService));

            services.AddTransient(typeof(MsgReceive));

        }




    }
}
