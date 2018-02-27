using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grpc.MicroService.Redis.Internal
{
    public class RedisRepository : IRedisRepository
    {
        #region 初始化

        private IDatabase _db;
        private readonly ConnectionMultiplexer _redis;

        /// <summary>
        /// 构造函数，在其中注册Redis事件
        /// </summary>
        public RedisRepository(string configuration)
        {
            //const string configuration = "{0},abortConnect=false,defaultDatabase={1},ssl=false,ConnectTimeout={2},allowAdmin=true,connectRetry={3}";
            _redis = ConnectionMultiplexer.Connect(configuration);
            //_redis.PreserveAsyncOrder = false;
            //_redis.ConnectionFailed;
            _db = _redis.GetDatabase();
        }

        /// <summary>
        /// 设置操作的数据库
        /// </summary>
        /// <param name="db"></param>
        /// <param name="asyncState"></param>
        public void SetDatabase(int db, object asyncState = null)
        {
            _db = _redis.GetDatabase(db, asyncState);
        }

        #endregion

        #region Redis String数据类型操作

        /// <summary>
        /// Redis String类型 新增一条记录
        /// </summary>
        /// <typeparam name="T">generic refrence type</typeparam>
        /// <param name="key">unique key of value</param>
        /// <param name="value">value of key of type T</param>
        /// <param name="expiresAt">time span of expiration</param>
        /// <returns>true or false</returns>
        public bool StringSet<T>(string key, T value, TimeSpan? expiresAt = default(TimeSpan?), When when = When.Always, CommandFlags commandFlags = CommandFlags.None) where T : class
        {
            var stringContent = SerializeContent(value);
            return _db.StringSet(key, stringContent, expiresAt, when, commandFlags);
        }

        /// <summary>
        /// Redis String类型 新增一条记录
        /// </summary>
        /// <typeparam name="T">generic refrence type</typeparam>
        /// <param name="key">unique key of value</param>
        /// <param name="value">value of key of type object</param>
        /// <param name="expiresAt">time span of expiration</param>
        /// <returns>true or false</returns>
        public bool StringSet<T>(string key, object value, TimeSpan? expiresAt = default(TimeSpan?), When when = When.Always, CommandFlags commandFlags = CommandFlags.None) where T : class
        {
            var stringContent = SerializeContent(value);

            return _db.StringSet(key, stringContent, expiresAt, when, commandFlags);
        }


        /// <summary>
        /// Redis String类型 新增一条记录
        /// </summary>
        /// <typeparam name="T">generic refrence type</typeparam>
        /// <param name="key">unique key of value</param>
        /// <param name="value">value of key of type object</param>
        /// <param name="expiresAt">time span of expiration</param>
        /// <returns>true or false</returns>
        public bool StringSetByValue(string key, RedisValue value, TimeSpan? expiresAt = default(TimeSpan?), When when = When.Always, CommandFlags commandFlags = CommandFlags.None)
        {
            return _db.StringSet(key, value, expiresAt, when, commandFlags);
        }

        /// <summary>
        /// Redis String数据类型 获取指定key中字符串长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long StringLength(string key, CommandFlags commandFlags = CommandFlags.None)
        {
            return _db.StringLength(key, commandFlags);
        }

        /// <summary>
        ///  Redis String数据类型  返回拼接后总长度
        /// </summary>
        /// <param name="key"></param>
        /// <param name="appendVal"></param>
        /// <returns>总长度</returns>
        public long StringAppend(string key, string appendVal, CommandFlags commandFlags = CommandFlags.None)
        {
            return _db.StringAppend(key, appendVal, commandFlags);
        }

        /// <summary>
        /// 设置新值并且返回旧值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newVal"></param>
        /// <param name="commandFlags"></param>
        /// <returns>OldVal</returns>
        public string StringGetAndSet(string key, string newVal, CommandFlags commandFlags = CommandFlags.None)
        {
            return DeserializeContent<string>(_db.StringGetSet(key, newVal, commandFlags));
        }

        /// <summary>
        /// 更新时应使用此方法，代码更可读。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiresAt"></param>
        /// <param name="when"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        public bool StringUpdate<T>(string key, T value, TimeSpan expiresAt, When when = When.Always, CommandFlags commandFlags = CommandFlags.None) where T : class
        {
            var stringContent = SerializeContent(value);
            return _db.StringSet(key, stringContent, expiresAt, when, commandFlags);
        }

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <param name="commandFlags"></param>
        /// <returns>增长后的值</returns>
        public double StringIncrement(string key, double val, CommandFlags commandFlags = CommandFlags.None)
        {
            return _db.StringIncrement(key, val, commandFlags);
        }

        /// <summary>
        /// Redis String类型  Get
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns>T</returns>
        public T StringGet<T>(string key, CommandFlags commandFlags = CommandFlags.None) where T : class
        {
            try
            {
                RedisValue myString = _db.StringGet(key, commandFlags);

                if (myString.HasValue && !myString.IsNullOrEmpty)
                {
                    return DeserializeContent<T>(myString);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                // Log Exception
                return null;
            }
        }

        /// <summary>
        /// Redis String类型  Get
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns>T</returns>
        public string StringGet(string key, CommandFlags commandFlags = CommandFlags.None)
        {
            return _db.StringGet(key, commandFlags);
        }

        #endregion

        #region Redis Hash散列数据类型操作
        /// <summary>
        /// Redis散列数据类型  批量新增
        /// </summary>
        public void HashSet(string key, List<HashEntry> hashEntrys, CommandFlags flags = CommandFlags.None)
        {
            _db.HashSet(key, hashEntrys.ToArray(), flags);
        }
        /// <summary>
        /// Redis散列数据类型  新增一个
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="val"></param>
        public void HashSet<T>(string key, string field, T val, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            _db.HashSet(key, field, SerializeContent(val), when, flags);
        }
        /// <summary>
        ///  Redis散列数据类型 获取指定key的指定field
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public T HashGet<T>(string key, string field)
        {
            return DeserializeContent<T>(_db.HashGet(key, field));
        }
        /// <summary>
        ///  Redis散列数据类型 获取所有field所有值,以 HashEntry[]形式返回
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public HashEntry[] HashGetAll(string key, CommandFlags flags = CommandFlags.None)
        {
            return _db.HashGetAll(key, flags);
        }
        /// <summary>
        /// Redis散列数据类型 获取key中所有field的值。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public List<T> HashGetAllValues<T>(string key, CommandFlags flags = CommandFlags.None)
        {
            List<T> list = new List<T>();
            var hashVals = _db.HashValues(key, flags).ToArray();
            foreach (var item in hashVals)
            {
                list.Add(DeserializeContent<T>(item));
            }
            return list;
        }

        /// <summary>
        /// Redis散列数据类型 获取所有Key名称
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public string[] HashGetAllKeys(string key, CommandFlags flags = CommandFlags.None)
        {
            return _db.HashKeys(key, flags).ToStringArray();
        }
        /// <summary>
        ///  Redis散列数据类型  单个删除field
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public bool HashDelete(string key, string hashField, CommandFlags flags = CommandFlags.None)
        {
            return _db.HashDelete(key, hashField, flags);
        }
        /// <summary>
        ///  Redis散列数据类型  批量删除field
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashFields"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public long HashDelete(string key, string[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            List<RedisValue> list = new List<RedisValue>();
            for (int i = 0; i < hashFields.Length; i++)
            {
                list.Add(hashFields[i]);
            }
            return _db.HashDelete(key, list.ToArray(), flags);
        }
        /// <summary>
        ///  Redis散列数据类型 判断指定键中是否存在此field
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public bool HashExists(string key, string field, CommandFlags flags = CommandFlags.None)
        {
            return _db.HashExists(key, field, flags);
        }
        /// <summary>
        /// Redis散列数据类型  获取指定key中field数量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public long HashLength(string key, CommandFlags flags = CommandFlags.None)
        {
            return _db.HashLength(key, flags);
        }
        /// <summary>
        /// Redis散列数据类型  为key中指定field增加incrVal值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="incrVal"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public double HashIncrement(string key, string field, double incrVal, CommandFlags flags = CommandFlags.None)
        {
            return _db.HashIncrement(key, field, incrVal, flags);
        }
        #endregion

        #region Redis List列表数据类型操作

        public T ListGetByIndex<T>(string key, long index, CommandFlags flags = CommandFlags.None)
        {
            return DeserializeContent<T>(_db.ListGetByIndex(key, index, flags));
        }

        public long ListInsertAfter(string key, object pivot, object value, CommandFlags flags = CommandFlags.None)
        {
            return _db.ListInsertAfter(key, SerializeContent(pivot), SerializeContent(value), flags);
        }

        public long ListInsertBefore(string key, object pivot, object value, CommandFlags flags = CommandFlags.None)
        {
            return _db.ListInsertBefore(key, SerializeContent(pivot), SerializeContent(value), flags);
        }

        public T ListLeftPop<T>(string key, CommandFlags flags = CommandFlags.None)
        {
            return DeserializeContent<T>(_db.ListLeftPop(key, flags));
        }

        public long ListLeftPush(string key, object[] values, CommandFlags flags = CommandFlags.None)
        {
            return _db.ListLeftPush(key, values.Select(s => (RedisValue)SerializeContent(s)).ToArray(), flags);
        }

        public long ListLeftPush(string key, object value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            return _db.ListLeftPush(key, SerializeContent(value), when, flags);
        }

        public long ListLength(string key, CommandFlags flags = CommandFlags.None)
        {
            return _db.ListLength(key, flags);
        }

        public T[] ListRange<T>(string key, long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None)
        {
            return _db.ListRange(key, start, stop, flags).Select(s => DeserializeContent<T>(s)).ToArray();
        }

        public long ListRemove(string key, object value, long count = 0, CommandFlags flags = CommandFlags.None)
        {
            return _db.ListRemove(key, SerializeContent(value), count, flags);
        }

        public T ListRightPop<T>(string key, CommandFlags flags = CommandFlags.None)
        {
            return DeserializeContent<T>(_db.ListRightPop(key, flags));
        }

        public T ListRightPopLeftPush<T>(string source, string destination, CommandFlags flags = CommandFlags.None)
        {
            return DeserializeContent<T>(_db.ListRightPopLeftPush(source, destination, flags));
        }

        public long ListRightPush(string key, object[] values, CommandFlags flags = CommandFlags.None)
        {
            return _db.ListRightPush(key, values.Select(s => (RedisValue)SerializeContent(s)).ToArray(), flags);
        }

        public long ListRightPush(string key, object value, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            return _db.ListRightPush(key, SerializeContent(value), when, flags);
        }

        public void ListSetByIndex(string key, long index, object value, CommandFlags flags = CommandFlags.None)
        {
            _db.ListSetByIndex(key, index, SerializeContent(value), flags);
        }

        public void ListTrim(string key, long start, long stop, CommandFlags flags = CommandFlags.None)
        {
            _db.ListTrim(key, start, stop, flags);
        }

        #endregion

        #region Redis Set集合数据类型操作

        #endregion

        #region Redis Sort Set有序集合数据类型操作

        #endregion

        #region Redis发布订阅


        #endregion

        #region Redis各数据类型公用

        /// <summary>
        /// 设置key的过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiry"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public bool KeyExpire(RedisKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None)
        {
            return _db.KeyExpire(key, expiry, flags);
        }

        /// <summary>
        /// Redis中是否存在指定Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool KeyExists(string key, CommandFlags commandFlags = CommandFlags.None)
        {
            return _db.KeyExists(key, commandFlags);
        }

        /// <summary>
        /// Dispose DB connection 释放DB相关链接
        /// </summary>
        public void DbConnectionStop()
        {
            _redis.Dispose();
        }

        /// <summary>
        /// 从Redis中移除键
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool KeyRemove(string key, CommandFlags commandFlags = CommandFlags.None)
        {
            return _db.KeyDelete(key, commandFlags);
        }
        /// <summary>
        /// 从Redis中移除多个键
        /// </summary>
        /// <param name="keys"></param>
        public void KeyRemove(RedisKey[] keys, CommandFlags commandFlags = CommandFlags.None)
        {
            _db.KeyDelete(keys, commandFlags);
        }
        #endregion

        #region 私有公用方法   在其中我们序列化操作使用Newtonsoft.Json组件

        private string SerializeContent(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        private T DeserializeContent<T>(RedisValue myString)
        {
            if (string.IsNullOrWhiteSpace(myString)) return default(T);
            return JsonConvert.DeserializeObject<T>(myString);
        }

        #endregion
    }
}
