using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZJJW.UMC.Client.Options
{
    public class RedisOptions : IOptions<RedisOptions>
    {



        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { set; get; }


        /// <summary>
        /// 实例名称
        /// </summary>
        public string InstanceName { set; get; }



        public RedisOptions Value => this;
    }
}
