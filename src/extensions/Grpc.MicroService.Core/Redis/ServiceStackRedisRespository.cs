using Grpc.MicroService.Redis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using ServiceStack.Redis;
using System.Linq;

namespace Grpc.MicroService.Redis
{
    public class ServiceStackRedisRespository : IRedisRepository
    {
        #region 初始化

        private readonly IRedisClientsManager _redisClient;

        /// <summary>
        /// 构造函数，在其中注册Redis事件
        /// </summary>
        public ServiceStackRedisRespository(string[] masters, string[] slaves, int maxWritePoolSize, int maxReadPoolSize, bool autoStart, int defaultDb)
        {
            RedisConfig.VerifyMasterConnections = false;

            _redisClient = new PooledRedisClientManager(masters, slaves, new RedisClientManagerConfig
            {
                MaxWritePoolSize = maxWritePoolSize,
                MaxReadPoolSize = maxReadPoolSize,
                AutoStart = autoStart,
                DefaultDb = defaultDb
            });
        }

        /// <summary>
        /// 设置操作的数据库
        /// </summary>
        /// <param name="db"></param>
        /// <param name="asyncState"></param>
        public void SetDatabase(int db, object asyncState = null)
        {
            
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
        public bool StringSet<T>(string key, T value, TimeSpan? expiresAt = default(TimeSpan?)) where T : class
        {
            using (var redis = _redisClient.GetClient())
            {
                return expiresAt == null ? redis.Set<T>(key, value) : redis.Set<T>(key, value, expiresAt.Value);
            }
        }

        /// <summary>
        /// Redis String类型 新增一条记录
        /// </summary>
        /// <typeparam name="T">generic refrence type</typeparam>
        /// <param name="key">unique key of value</param>
        /// <param name="value">value of key of type T</param>
        /// <param name="expiresAt">time span of expiration</param>
        /// <returns>true or false</returns>
        public bool StringSet(string key, int value, TimeSpan? expiresAt = default(TimeSpan?))
        {
            using (var redis = _redisClient.GetClient())
            {
                return expiresAt == null ? redis.Set(key, value) : redis.Set(key, value, expiresAt.Value);
            }
        }

        /// <summary>
        /// Redis String类型 新增一条记录
        /// </summary>
        /// <typeparam name="T">generic refrence type</typeparam>
        /// <param name="key">unique key of value</param>
        /// <param name="value">value of key of type object</param>
        /// <param name="expiresAt">time span of expiration</param>
        /// <returns>true or false</returns>
        public bool StringSet<T>(string key, object value, TimeSpan? expiresAt = default(TimeSpan?)) where T : class
        {
            var stringContent = SerializeContent(value);
            using (var redis = _redisClient.GetClient())
            {
                return expiresAt == null ? redis.Set(key, stringContent) : redis.Set(key, stringContent, expiresAt.Value);
            }
        }


        ///// <summary>
        ///// Redis String类型 新增一条记录
        ///// </summary>
        ///// <typeparam name="T">generic refrence type</typeparam>
        ///// <param name="key">unique key of value</param>
        ///// <param name="value">value of key of type object</param>
        ///// <param name="expiresAt">time span of expiration</param>
        ///// <returns>true or false</returns>
        //public bool StringSetByValue(string key, RedisValue value, TimeSpan? expiresAt = default(TimeSpan?))
        //{
        //    return _db.StringSet(key, value, expiresAt);
        //}

        /// <summary>
        /// Redis String数据类型 获取指定key中字符串长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long StringLength(string key)
        {
            using (var redis = _redisClient.GetClient())
            {
                return redis.GetStringCount(key);
            }
        }

        /// <summary>
        ///  Redis String数据类型  返回拼接后总长度
        /// </summary>
        /// <param name="key"></param>
        /// <param name="appendVal"></param>
        /// <returns>总长度</returns>
        public long StringAppend(string key, string appendVal)
        {
            using (var redis = _redisClient.GetClient())
            {
                return redis.AppendToValue(key, appendVal);
            }
        }

        /// <summary>
        /// 设置新值并且返回旧值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newVal"></param>
        /// <param name="commandFlags"></param>
        /// <returns>OldVal</returns>
        public string StringGetAndSet(string key, string newVal)
        {
            using (var redis = _redisClient.GetClient())
            {
                return redis.GetAndSetValue(key, newVal);
            }
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
        public bool StringUpdate<T>(string key, T value, TimeSpan expiresAt) where T : class
        {
            var stringContent = SerializeContent(value);
            using (var redis = _redisClient.GetClient())
            {
                return expiresAt == null ? redis.Set(key, stringContent) : redis.Set(key, stringContent, expiresAt);
            }
        }

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val">可以为负</param>
        /// <param name="commandFlags"></param>
        /// <returns>增长后的值</returns>
        public double StringIncrement(string key, double val)
        {
            using (var redis = _redisClient.GetClient())
            {
                return redis.IncrementValueBy(key, val);
            }
        }
        
        /// <summary>
        /// Redis String类型  Get
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns>T</returns>
        public T StringGet<T>(string key) where T : class
        {
            using (var redis = _redisClient.GetClient())
            {
                return redis.Get<T>(key);
            }
        }

        /// <summary>
        /// Redis String类型  Get
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns>T</returns>
        public string StringGet(string key)
        {
            using (var redis = _redisClient.GetClient())
            {
                return redis.Get<string>(key);
            }
        }

        #endregion

        #region Redis Hash散列数据类型操作
        /// <summary>
        /// Redis散列数据类型  批量新增
        /// </summary>
        public void HashSet<T>(string key, IEnumerable<KeyValuePair<string, T>> keyvalues)
        {
            using (var redis = _redisClient.GetClient())
            {
                redis.SetRangeInHash(key, keyvalues.Select(p => new KeyValuePair<string, string>(p.Key, SerializeContent(p.Value))));
            }
        }
        /// <summary>
        /// Redis散列数据类型  新增一个
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="val"></param>
        public void HashSet<T>(string key, string field, T val)
        {
            using (var redis = _redisClient.GetClient())
            {
                redis.SetEntryInHash(key, field, SerializeContent(val));
            }
        }
        /// <summary>
        ///  Redis散列数据类型 获取指定key的指定field
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public T HashGet<T>(string key, string field)
        {
            using (var redis = _redisClient.GetClient())
            {
                return DeserializeContent<T>(redis.GetValueFromHash(key, field));
            }
        }

        /// <summary>
        ///  Redis散列数据类型 获取所有field所有值,以 HashEntry[]形式返回
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, T>> HashGetAll<T>(string key)
        {
            using (var redis = _redisClient.GetClient())
            {
                var allHashEntrys = redis.GetAllEntriesFromHash(key);
                return allHashEntrys.Select(p => new KeyValuePair<string, T>(p.Key, DeserializeContent<T>(p.Value)));
            }
        }
        /// <summary>
        /// Redis散列数据类型 获取key中所有field的值。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public IEnumerable<T> HashGetAllValues<T>(string key)
        {
            using (var redis = _redisClient.GetClient())
            {
                return redis.GetHashValues(key).Select(p => DeserializeContent<T>(p));
            }
        }

        /// <summary>
        /// Redis散列数据类型 获取所有Key名称
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public string[] HashGetAllKeys(string key)
        {
            using (var redis = _redisClient.GetClient())
            {
                return redis.GetHashKeys(key).ToArray();
            }
        }
        /// <summary>
        ///  Redis散列数据类型  单个删除field
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public bool HashDelete(string key, string hashField)
        {
            using (var redis = _redisClient.GetClient())
            {
                return redis.RemoveEntryFromHash(hashField, key);
            }
        }
        /// <summary>
        ///  Redis散列数据类型  批量删除field
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashFields"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public long HashDelete(string key, string[] hashFields)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        ///  Redis散列数据类型 判断指定键中是否存在此field
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public bool HashExists(string key, string field)
        {
            using (var redis = _redisClient.GetClient())
            {
                return redis.HashContainsEntry(field, key);
            }
        }
        /// <summary>
        /// Redis散列数据类型  获取指定key中field数量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public long HashLength(string key)
        {
            using (var redis = _redisClient.GetClient())
            {
                return redis.GetHashCount(key);
            }
        }
        /// <summary>
        /// Redis散列数据类型  为key中指定field增加incrVal值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="incrVal"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public double HashIncrement(string key, string field, double incrVal)
        {
            using (var redis = _redisClient.GetClient())
            {
                return redis.IncrementValueInHash(field, key, incrVal);
            }
        }
        #endregion

        #region Redis List列表数据类型操作

        public T ListGetByIndex<T>(string key, long index)
        {
            using (var redis = _redisClient.GetClient())
            {
                return DeserializeContent<T>(redis.GetItemFromList(key, (int)index));
            }
        }

        public long ListInsertAfter(string key, object pivot, object value)
        {
            throw new NotImplementedException();
        }

        public long ListInsertBefore(string key, object pivot, object value)
        {
            throw new NotImplementedException();
        }

        public T ListLeftPop<T>(string key)
        {
            using (var redis = _redisClient.GetClient())
            {
                return DeserializeContent<T>(redis.RemoveStartFromList(key));
            }
            //return DeserializeContent<T>(_db.ListLeftPop(key));
        }

        public long ListLeftPush(string key, object[] values)
        {
            using (var redis = _redisClient.GetClient())
            {
                redis.PrependRangeToList(key, values.Select(s => SerializeContent(s)).ToList());
                return redis.GetListCount(key);
            }
        }

        public long ListLeftPush(string key, object value)
        {
            using (var redis = _redisClient.GetClient())
            {
                redis.PrependItemToList(key, SerializeContent(value));
                return redis.GetListCount(key);
            }
            //return _db.ListLeftPush(key, SerializeContent(value));
        }

        public long ListLength(string key)
        {
            using (var redis = _redisClient.GetClient())
            {
                return redis.GetListCount(key);
            }
        }

        public T[] ListRange<T>(string key, long start = 0, long stop = -1)
        {
            throw new NotImplementedException();
            //return _db.ListRange(key, start, stop).Select(s => DeserializeContent<T>(s)).ToArray();
        }

        public long ListRemove(string key, object value, long count = 0)
        {
            throw new NotImplementedException();
            //return _db.ListRemove(key, SerializeContent(value), count);
        }

        public T ListRightPop<T>(string key)
        {
            throw new NotImplementedException();
            //return DeserializeContent<T>(_db.ListRightPop(key));
        }

        public T ListRightPopLeftPush<T>(string source, string destination)
        {
            throw new NotImplementedException();
            //return DeserializeContent<T>(_db.ListRightPopLeftPush(source, destination));
        }

        public long ListRightPush(string key, object[] values)
        {
            throw new NotImplementedException();
            //return _db.ListRightPush(key, values.Select(s => (RedisValue)SerializeContent(s)).ToArray());
        }

        public long ListRightPush(string key, object value)
        {
            throw new NotImplementedException();
            //return _db.ListRightPush(key, SerializeContent(value));
        }

        public void ListSetByIndex(string key, long index, object value)
        {
            throw new NotImplementedException();
            //_db.ListSetByIndex(key, index, SerializeContent(value));
        }

        public void ListTrim(string key, long start, long stop)
        {
            throw new NotImplementedException();
            //_db.ListTrim(key, start, stop);
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
        public bool KeyExpire(string key, TimeSpan? expiry)
        {
            using (var redis = _redisClient.GetClient())
            {
                return expiry == null ? redis.ExpireEntryIn(key, expiry.Value) : false;
            }
        }

        /// <summary>
        /// Redis中是否存在指定Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool KeyExists(string key)
        {
            using (var redis = _redisClient.GetClient())
            {
                return redis.ContainsKey(key);
            }
        }

        /// <summary>
        /// Dispose DB connection 释放DB相关链接
        /// </summary>
        public void DbConnectionStop()
        {
            _redisClient.Dispose();
        }

        /// <summary>
        /// 从Redis中移除键
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool KeyRemove(string key)
        {
            using (var redis = _redisClient.GetClient())
            {
                return redis.Remove(key);
            }
        }
        /// <summary>
        /// 从Redis中移除多个键
        /// </summary>
        /// <param name="keys"></param>
        public void KeyRemove(string[] keys)
        {
            using (var redis = _redisClient.GetClient())
            {
                redis.RemoveAll(keys);
            }
        }
        #endregion

        #region 私有公用方法   在其中我们序列化操作使用Newtonsoft.Json组件

        private string SerializeContent(object value)
        {
            return JsonConvert.SerializeObject(value);
        }

        private T DeserializeContent<T>(string myString)
        {
            if (string.IsNullOrWhiteSpace(myString)) return default(T);
            return JsonConvert.DeserializeObject<T>(myString);
        }

        public void Dispose()
        {
            _redisClient.Dispose();
        }

        #endregion
    }
}
