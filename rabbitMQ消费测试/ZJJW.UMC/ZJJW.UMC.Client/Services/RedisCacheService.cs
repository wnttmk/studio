using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZJJW.UMC.Client.Services
{
    /// <summary>
    /// Redis缓存服务
    /// </summary>
    public class RedisCacheService
    {

        Options.RedisOptions options;

        ILogger<RedisCacheService> logger;

        private ConnectionMultiplexer redisMultiplexer;

        IDatabase _database;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public RedisCacheService(IOptions<Options.RedisOptions> options, ILogger<RedisCacheService> logger)
        {
            this.options = options.Value;
            this.logger = logger;
            this.Build();
        }

        /// <summary>
        /// 创建
        /// </summary>
        public void Build()
        {
            try
            {
                var RedisConnection = options.ConnectionString;
                ConfigurationOptions opt = ConfigurationOptions.Parse(RedisConnection);
                //Dictionary<string, string> pairs = new Dictionary<string, string>();
                //pairs.Add("AUTH", "hngslgj@2021.com");
                //CommandMap commandMap = CommandMap.Create(pairs);
                //opt.CommandMap = commandMap;

                redisMultiplexer = ConnectionMultiplexer.Connect(opt);


                _database = redisMultiplexer.GetDatabase();

                this.logger.LogInformation("Redis连接成功 ->" + options.ConnectionString);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "创建redis连接时出现异常");
                redisMultiplexer = null;
                _database = null;
            }

        }


        #region 同步方法...

        /// <summary>
        /// 添加一个字符串对象。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        /// <param name="expiry">过期时间（时间间隔）。</param>
        /// <returns>返回是否执行成功。</returns>
        public bool Set(string key, string value, TimeSpan? expiry = null)
        {
            return _database.StringSet(key, value, expiry);
        }

        /// <summary>
        /// 添加一个字符串对象。
        /// </summary>
        /// <param name="key">键。</param>
        /// <typeparam name="T">对象的类型。</typeparam>
        /// <param name="value">值。</param>
        /// <param name="seconds">过期时间（秒）。</param>
        /// <returns>返回是否执行成功。</returns>
        public bool Set(string key, string value, int seconds)
        {
            TimeSpan expiry = TimeSpan.FromSeconds(seconds);
            return _database.StringSet(key, value, expiry);
        }

        /// <summary>
        /// 添加一个对象。
        /// </summary>
        /// <param name="key">键。</param>
        /// <typeparam name="T">对象的类型。</typeparam>
        /// <param name="value">值。</param>
        /// <param name="expiry">过期时间（时间间隔）。</param>
        /// <returns>返回是否执行成功。</returns>
        public bool Set<T>(string key, T value, TimeSpan? expiry = null)
        {
            var data = JsonConvert.SerializeObject(value);
            return _database.StringSet(key, data, expiry);
        }

        /// <summary>
        /// 添加一个对象。
        /// </summary>
        /// <param name="key">键。</param>
        /// <typeparam name="T">对象的类型。</typeparam>
        /// <param name="value">值。</param>
        /// <param name="seconds">过期时间（秒）。</param>
        /// <returns>返回是否执行成功。</returns>
        public bool Set<T>(string key, T value, int seconds)
        {
            TimeSpan expiry = TimeSpan.FromSeconds(seconds);
            var data = JsonConvert.SerializeObject(value);
            return _database.StringSet(key, data, expiry);
        }

        /// <summary>
        /// 获取一个对象。
        /// </summary>
        /// <param name="key">值。</param>
        /// <returns>返回对象的值。</returns>
        public T Get<T>(string key)
        {
            string json = _database.StringGet(key);
            if (string.IsNullOrWhiteSpace(json))
            {
                return default(T);
            }
            T entity = JsonConvert.DeserializeObject<T>(json);
            return entity;
        }

        /// <summary>
        /// 获取一个字符串对象。
        /// </summary>
        /// <param name="key">值。</param>
        /// <returns>返回对象的值。</returns>
        public string Get(string key)
        {
            return _database.StringGet(key);
        }

        /// <summary>
        /// 删除一个对象。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>返回是否执行成功。</returns>
        public bool Delete(string key)
        {
            return _database.KeyDelete(key);
        }

        /// <summary>
        /// 返回键是否存在。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>返回键是否存在。</returns>
        public bool Exists(string key)
        {
            return _database.KeyExists(key);
        }

        /// <summary>
        /// 设置一个键的过期时间。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="expiry">过期时间（时间间隔）。</param>
        /// <returns>返回是否执行成功。</returns>
        public bool SetExpire(string key, TimeSpan? expiry)
        {
            return _database.KeyExpire(key, expiry);
        }

        /// <summary>
        /// 设置一个键的过期时间。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="seconds">过期时间（秒）。</param>
        /// <returns>返回是否执行成功。</returns>
        public bool SetExpire(string key, int seconds)
        {
            TimeSpan expiry = TimeSpan.FromSeconds(seconds);
            return _database.KeyExpire(key, expiry);
        }

        #endregion

        #region 异步方法...

        /// <summary>
        /// 异步添加一个字符串对象。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        /// <param name="expiry">过期时间（时间间隔）。</param>
        /// <returns>返回是否执行成功。</returns>
        public async Task<bool> SetAsync(string key, string value, TimeSpan? expiry = null)
        {
            return await _database.StringSetAsync(key, value, expiry);
        }

        /// <summary>
        /// 异步添加一个字符串对象。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        /// <param name="seconds">过期时间（秒）。</param>
        /// <returns>返回是否执行成功。</returns>
        public async Task<bool> SetAsync(string key, string value, int seconds)
        {
            TimeSpan expiry = TimeSpan.FromSeconds(seconds);
            return await _database.StringSetAsync(key, value, expiry);
        }

        /// <summary>
        /// 异步添加一个对象。
        /// </summary>
        /// <param name="key">键。</param>
        /// <typeparam name="T">对象的类型。</typeparam>
        /// <param name="value">值。</param>
        /// <returns>返回是否执行成功。</returns>
        public async Task<bool> SetAsync<T>(string key, T value)
        {
            var data = JsonConvert.SerializeObject(value);
            return await _database.StringSetAsync(key, data);
        }

        /// <summary>
        /// 异步获取一个对象。
        /// </summary>
        /// <typeparam name="T">对象的类型。</typeparam>
        /// <param name="key">值。</param>
        /// <returns>返回对象的值。</returns>
        public async Task<T> GetAsync<T>(string key)
        {
            string json = await _database.StringGetAsync(key);
            if (string.IsNullOrWhiteSpace(json))
            {
                return default(T);
            }
            T entity = JsonConvert.DeserializeObject<T>(json);
            return entity;
        }

        /// <summary>
        /// 异步获取一个字符串对象。
        /// </summary>
        /// <param name="key">值。</param>
        /// <returns>返回对象的值。</returns>
        public async Task<string> GetAsync(string key)
        {
            return await _database.StringGetAsync(key);
        }

        /// <summary>
        /// 异步删除一个对象。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>返回是否执行成功。</returns>
        public async Task<bool> DeleteAsync(string key)
        {
            return await _database.KeyDeleteAsync(key);
        }

        /// <summary>
        /// 异步设置一个键的过期时间。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="seconds">过期时间（秒）。</param>
        /// <returns>返回是否执行成功。</returns>
        public async Task<bool> SetExpireAsync(string key, int seconds)
        {
            TimeSpan expiry = TimeSpan.FromSeconds(seconds);
            return await _database.KeyExpireAsync(key, expiry);
        }

        /// <summary>
        /// 异步设置一个键的过期时间。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="expiry">过期时间（时间间隔）。</param>
        /// <returns>返回是否执行成功。</returns>
        public async Task<bool> SetExpireAsync(string key, TimeSpan? expiry)
        {
            return await _database.KeyExpireAsync(key, expiry);
        }

        #endregion


        #region 分布式锁...

        /// <summary>
        /// 分布式锁 Token。
        /// </summary>
        private static readonly RedisValue LockToken = Environment.MachineName;

        /// <summary>
        /// 获取锁。
        /// </summary>
        /// <param name="key">锁名称。</param>
        /// <param name="seconds">过期时间（秒）。</param>
        /// <returns>是否已锁。</returns>
        public bool Lock(string key, int seconds)
        {
            return _database.LockTake(key, LockToken, TimeSpan.FromSeconds(seconds));
        }

        /// <summary>
        /// 释放锁。
        /// </summary>
        /// <param name="key">锁名称。</param>
        /// <returns>是否成功。</returns>
        public bool UnLock(string key)
        {
            return _database.LockRelease(key, LockToken);
        }

        /// <summary>
        /// 异步获取锁。
        /// </summary>
        /// <param name="key">锁名称。</param>
        /// <param name="seconds">过期时间（秒）。</param>
        /// <returns>是否成功。</returns>
        public async Task<bool> LockAsync(string key, int seconds)
        {
            return await _database.LockTakeAsync(key, LockToken, TimeSpan.FromSeconds(seconds));
        }

        /// <summary>
        /// 异步释放锁。
        /// </summary>
        /// <param name="key">锁名称。</param>
        /// <returns>是否成功。</returns>
        public async Task<bool> UnLockAsync(string key)
        {
            return await _database.LockReleaseAsync(key, LockToken);
        }

        #endregion

    }
}
