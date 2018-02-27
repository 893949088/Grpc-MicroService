using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.MicroService.Redis
{
    public interface IRedisRepository
    {
        #region Redis 客户端操作

        /// <summary>
        /// 设置操作的数据库
        /// </summary>
        /// <param name="db"></param>
        /// <param name="asyncState"></param>
        void SetDatabase(int db, object asyncState = null);

        #endregion

        #region Redis String类型操作

        /// <summary>
        /// Redis String类型 新增一条记录
        /// </summary>
        /// <typeparam name="T">generic refrence type</typeparam>
        /// <param name="key">unique key of value</param>
        /// <param name="value">value of key of type object</param>
        /// <param name="expiresAt">time span of expiration</param>
        /// <param name= "when">枚举类型</param>
        /// <param name="commandFlags"></param>
        /// <returns>true or false</returns>
        bool StringSet<T>(string key, object value, TimeSpan? expiry = default(TimeSpan?), When when = When.Always, CommandFlags commandFlags = CommandFlags.None) where T : class;

        /// <summary>
        /// Redis String类型 新增一条记录
        /// </summary>
        /// <typeparam name="T">generic refrence type</typeparam>
        /// <param name="key">unique key of value</param>
        /// <param name="value">value of key of type object</param>
        /// <param name="expiresAt">time span of expiration</param>
        /// <param name= "when">枚举类型</param>
        /// <param name="commandFlags"></param>
        /// <returns>true or false</returns>
        bool StringSet<T>(string key, T value, TimeSpan? expiry = default(TimeSpan?), When when = When.Always, CommandFlags commandFlags = CommandFlags.None) where T : class;

        /// <summary>
        /// Redis String类型 新增一条记录
        /// </summary>
        /// <typeparam name="T">generic refrence type</typeparam>
        /// <param name="key">unique key of value</param>
        /// <param name="value">value of key of type object</param>
        /// <param name="expiresAt">time span of expiration</param>
        /// <returns>true or false</returns>
        bool StringSetByValue(string key, RedisValue value, TimeSpan? expiresAt = default(TimeSpan?), When when = When.Always, CommandFlags commandFlags = CommandFlags.None);

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
        bool StringUpdate<T>(string key, T value, TimeSpan expiresAt, When when = When.Always, CommandFlags commandFlags = CommandFlags.None) where T : class;

        /// <summary>
        /// Redis String类型  Get
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="commandFlags"></param>
        /// <returns>T</returns>
        T StringGet<T>(string key, CommandFlags commandFlags = CommandFlags.None) where T : class;

        /// <summary>
        /// Redis String类型  Get
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="commandFlags"></param>
        /// <returns>T</returns>
        string StringGet(string key, CommandFlags commandFlags = CommandFlags.None);
        /// <summary>
        /// Redis String数据类型 获取指定key中字符串长度
        /// </summary>
        /// <param name="key"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        long StringLength(string key, CommandFlags commandFlags = CommandFlags.None);

        /// <summary>
        ///  Redis String数据类型  返回拼接后总长度
        /// </summary>
        /// <param name="key"></param>
        /// <param name="appendVal"></param>
        /// <param name="commandFlags"></param>
        /// <returns>总长度</returns>
        long StringAppend(string key, string appendVal, CommandFlags commandFlags = CommandFlags.None);

        /// <summary>
        /// 设置新值并且返回旧值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newVal"></param>
        /// <param name="commandFlags"></param>
        /// <returns>OldVal</returns>
        string StringGetAndSet(string key, string newVal, CommandFlags commandFlags = CommandFlags.None);

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="commandFlags"></param>
        /// <returns>增长后的值</returns>
        double StringIncrement(string key, double val, CommandFlags commandFlags = CommandFlags.None);

        /// <summary>
        /// Redis String数据类型
        /// 类似于模糊查询  key* 查出所有key开头的键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="pageSize"></param>
        /// <param name="commandFlags"></param>
        /// <returns>返回List<T></returns>
        //List<T> StringGetList<T>(string key, int pageSize = 1000, CommandFlags commandFlags = CommandFlags.None) where T : class;
        #endregion

        #region Redis Hash散列数据类型操作

        /// <summary>
        /// Redis散列数据类型  批量新增
        /// </summary>
        void HashSet(string key, List<HashEntry> hashEntrys, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Redis散列数据类型  新增一个
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="val"></param>
        void HashSet<T>(string key, string field, T val, When when = When.Always, CommandFlags flags = CommandFlags.None);

        /// <summary>
        ///  Redis散列数据类型 获取指定key的指定field
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        T HashGet<T>(string key, string field);

        /// <summary>
        ///  Redis散列数据类型 获取所有field所有值,以 HashEntry[]形式返回
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        HashEntry[] HashGetAll(string key, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Redis散列数据类型 获取key中所有field的值。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        List<T> HashGetAllValues<T>(string key, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Redis散列数据类型 获取所有Key名称
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        string[] HashGetAllKeys(string key, CommandFlags flags = CommandFlags.None);

        /// <summary>
        ///  Redis散列数据类型  单个删除field
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashField"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        bool HashDelete(string key, string hashField, CommandFlags flags = CommandFlags.None);

        /// <summary>
        ///  Redis散列数据类型  批量删除field
        /// </summary>
        /// <param name="key"></param>
        /// <param name="hashFields"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        long HashDelete(string key, string[] hashFields, CommandFlags flags = CommandFlags.None);

        /// <summary>
        ///  Redis散列数据类型 判断指定键中是否存在此field
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        bool HashExists(string key, string field, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Redis散列数据类型  获取指定key中field数量
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        long HashLength(string key, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Redis散列数据类型  为key中指定field增加incrVal值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="incrVal"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        double HashIncrement(string key, string field, double incrVal, CommandFlags flags = CommandFlags.None);


        #endregion

        #region Redis List列表数据类型操作

        /// <summary>
        /// Returns the element at index index in the list stored at key. The index is zero-based, so 0 means the first element, 1 the second element and so on. Negative indices can be used to designate elements starting at the tail of the list. Here, -1 means the last element, -2 means the penultimate and so forth.
        /// </summary>
        /// <returns>the requested element, or nil when index is out of range.</returns>
        /// <remarks>http://redis.io/commands/lindex</remarks>
        T ListGetByIndex<T>(string key, long index, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Inserts value in the list stored at key either before or after the reference value pivot.
        /// When key does not exist, it is considered an empty list and no operation is performed.
        /// </summary>
        /// <returns>the length of the list after the insert operation, or -1 when the value pivot was not found.</returns>
        /// <remarks>http://redis.io/commands/linsert</remarks>
        long ListInsertAfter(string key, object pivot, object value, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Inserts value in the list stored at key either before or after the reference value pivot.
        /// When key does not exist, it is considered an empty list and no operation is performed.
        /// </summary>
        /// <returns>the length of the list after the insert operation, or -1 when the value pivot was not found.</returns>
        /// <remarks>http://redis.io/commands/linsert</remarks>
        long ListInsertBefore(string key, object pivot, object value, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Removes and returns the first element of the list stored at key.
        /// </summary>
        /// <returns>the value of the first element, or nil when key does not exist.</returns>
        /// <remarks>http://redis.io/commands/lpop</remarks>
        T ListLeftPop<T>(string key, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Insert the specified value at the head of the list stored at key. If key does not exist, it is created as empty list before performing the push operations.
        /// </summary>
        /// <returns>the length of the list after the push operations.</returns>
        /// <remarks>http://redis.io/commands/lpush</remarks>
        /// <remarks>http://redis.io/commands/lpushx</remarks>
        long ListLeftPush(string key, object value, When when = When.Always, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Insert all the specified values at the head of the list stored at key. If key does not exist, it is created as empty list before performing the push operations.
        /// Elements are inserted one after the other to the head of the list, from the leftmost element to the rightmost element. So for instance the command LPUSH mylist a b c will result into a list containing c as first element, b as second element and a as third element.
        /// </summary>
        /// <returns>the length of the list after the push operations.</returns>
        /// <remarks>http://redis.io/commands/lpush</remarks>
        long ListLeftPush(string key, object[] values, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns the length of the list stored at key. If key does not exist, it is interpreted as an empty list and 0 is returned. 
        /// </summary>
        /// <returns>the length of the list at key.</returns>
        /// <remarks>http://redis.io/commands/llen</remarks>
        long ListLength(string key, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Returns the specified elements of the list stored at key. The offsets start and stop are zero-based indexes, with 0 being the first element of the list (the head of the list), 1 being the next element and so on.
        /// These offsets can also be negative numbers indicating offsets starting at the end of the list.For example, -1 is the last element of the list, -2 the penultimate, and so on.
        /// Note that if you have a list of numbers from 0 to 100, LRANGE list 0 10 will return 11 elements, that is, the rightmost item is included. 
        /// </summary>
        /// <returns>list of elements in the specified range.</returns>
        /// <remarks>http://redis.io/commands/lrange</remarks>
        T[] ListRange<T>(string key, long start = 0, long stop = -1, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Removes the first count occurrences of elements equal to value from the list stored at key. The count argument influences the operation in the following ways:
        /// count &gt; 0: Remove elements equal to value moving from head to tail.
        /// count &lt; 0: Remove elements equal to value moving from tail to head.
        /// count = 0: Remove all elements equal to value.
        /// </summary>
        /// <returns>the number of removed elements.</returns>
        /// <remarks>http://redis.io/commands/lrem</remarks>
        long ListRemove(string key, object value, long count = 0, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Removes and returns the last element of the list stored at key.
        /// </summary>
        /// <remarks>http://redis.io/commands/rpop</remarks>
        T ListRightPop<T>(string key, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Atomically returns and removes the last element (tail) of the list stored at source, and pushes the element at the first element (head) of the list stored at destination.
        /// </summary>
        /// <returns>the element being popped and pushed.</returns>
        /// <remarks>http://redis.io/commands/rpoplpush</remarks>
        T ListRightPopLeftPush<T>(string source, string destination, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Insert the specified value at the tail of the list stored at key. If key does not exist, it is created as empty list before performing the push operation.
        /// </summary>
        /// <returns>the length of the list after the push operation.</returns>
        /// <remarks>http://redis.io/commands/rpush</remarks>
        /// <remarks>http://redis.io/commands/rpushx</remarks>
        long ListRightPush(string key, object value, When when = When.Always, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Insert all the specified values at the tail of the list stored at key. If key does not exist, it is created as empty list before performing the push operation. 
        /// Elements are inserted one after the other to the tail of the list, from the leftmost element to the rightmost element. So for instance the command RPUSH mylist a b c will result into a list containing a as first element, b as second element and c as third element.
        /// </summary>
        /// <returns>the length of the list after the push operation.</returns>
        /// <remarks>http://redis.io/commands/rpush</remarks>
        long ListRightPush(string key, object[] values, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Sets the list element at index to value. For more information on the index argument, see ListGetByIndex. An error is returned for out of range indexes.
        /// </summary>
        /// <remarks>http://redis.io/commands/lset</remarks>
        void ListSetByIndex(string key, long index, object value, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Trim an existing list so that it will contain only the specified range of elements specified. Both start and stop are zero-based indexes, where 0 is the first element of the list (the head), 1 the next element and so on.
        /// For example: LTRIM foobar 0 2 will modify the list stored at foobar so that only the first three elements of the list will remain.
        /// start and end can also be negative numbers indicating offsets from the end of the list, where -1 is the last element of the list, -2 the penultimate element and so on.
        /// </summary>
        /// <remarks>http://redis.io/commands/ltrim</remarks>
        void ListTrim(string key, long start, long stop, CommandFlags flags = CommandFlags.None);

        #endregion

        #region Redis Set集合数据类型操作

        #endregion

        #region Redis发布订阅


        #endregion

        #region Redis各数据类型公用

        /// <summary>
        /// Set a timeout on key. After the timeout has expired, the key will automatically be deleted. A key with an associated timeout is said to be volatile in Redis terminology.
        /// </summary>
        /// <remarks>If key is updated before the timeout has expired, then the timeout is removed as if the PERSIST command was invoked on key.
        /// For Redis versions &lt; 2.1.3, existing timeouts cannot be overwritten. So, if key already has an associated timeout, it will do nothing and return 0. Since Redis 2.1.3, you can update the timeout of a key. It is also possible to remove the timeout using the PERSIST command. See the page on key expiry for more information.</remarks>
        /// <returns>1 if the timeout was set. 0 if key does not exist or the timeout could not be set.</returns>
        /// <remarks>http://redis.io/commands/expire</remarks>
        /// <remarks>http://redis.io/commands/pexpire</remarks>
        /// <remarks>http://redis.io/commands/persist</remarks>
        bool KeyExpire(RedisKey key, TimeSpan? expiry, CommandFlags flags = CommandFlags.None);

        /// <summary>
        /// Redis中是否存在指定Key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        bool KeyExists(string key, CommandFlags commandFlags = CommandFlags.None);

        /// <summary>
        /// 从Redis中移除键
        /// </summary>
        /// <param name="key"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        bool KeyRemove(string key, CommandFlags commandFlags = CommandFlags.None);

        /// <summary>
        /// 从Redis中移除多个键
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="commandFlags"></param>
        /// <returns></returns>
        void KeyRemove(RedisKey[] keys, CommandFlags commandFlags = CommandFlags.None);

        /// <summary>
        /// Dispose DB connection 释放DB相关链接
        /// </summary>
        void DbConnectionStop();
        #endregion

    }
}
