using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ZJJW.UMC.Client.Services;
using ZJJWEPlatform.DataTransfer;

namespace ZJJW.UMC.Client.MQEventHandler
{
    /// <summary>
    /// 消息接收
    /// </summary>
    public class MsgReceive : IMQEventHandle
    {

        RedisCacheService redisCacheService;

        ILogger<MsgReceive> logger;

        public MsgReceive(RedisCacheService redisCacheService, ILogger<MsgReceive> logger)
        {
            this.redisCacheService = redisCacheService;

            this.logger = logger;
        }

        public ReceiveResult Execute(long seed, string content)
        {
            this.redisCacheService.Lock(seed.ToString(), 60);
            try
            {
                var isDob = new Random().Next(0, 10) % 2 == 0;
                Thread.Sleep(1000);
                if (!isDob)
                {
                    throw new Exception("this is no dob in this time. ");
                }
                return new ReceiveResult()
                {
                    State = ReceiveState.ACK,
                    ReplyContent = seed.ToString()

                };
            }
            catch (Exception ex)
            {
                return new ReceiveResult()
                {
                    State = ReceiveState.NACK_REQUEUE,
                    ReplyContent = ex.Message
                };
            }
            finally
            {
                this.redisCacheService.UnLock(seed.ToString());
            }
        }
    }
}
