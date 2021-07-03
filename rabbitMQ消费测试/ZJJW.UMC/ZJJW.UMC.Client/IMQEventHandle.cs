using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZJJWEPlatform.DataTransfer;

namespace ZJJW.UMC.Client
{
    /// <summary>
    /// 消息队列事件处理接口
    /// </summary>
    public interface IMQEventHandle
    {

        /// <summary>
        /// 事件处理
        /// </summary>
        /// <param name="content"></param>
        public ReceiveResult Execute(long seed, string content);
    }
}
