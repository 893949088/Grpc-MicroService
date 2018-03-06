using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Grpc.MicroService
{
    public static class CommonUtility
    {
        /// <summary>
        /// 获取js的时间戳（从1970/1/1 开始的毫秒数,UTC时间）
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long GetJsTimespan(DateTime dt)
        {
            var ts = dt.ToUniversalTime() - new DateTime(1970, 1, 1);
            return (long)ts.TotalMilliseconds;
        }

        /// <summary>
        /// 获取c#时间
        /// </summary>
        /// <param name="jstime"></param>
        /// <returns></returns>
        public static DateTime GetConvertTime(long jstime)
        {
            var timeTricks = new DateTime(1970, 1, 1).Ticks + jstime * 10000 +
                TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).Hours * 3600 * (long)10000000;
            return new DateTime(timeTricks);
        }

        /// <summary>
        /// 检查密码是否一致
        /// </summary>
        /// <param name="password"></param>
        /// <param name="dbpassword"></param>
        /// <param name="passwordKey"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static bool CheckPassword(string password, string dbpassword,
            string passwordKey, CommonEnum.PasswordFormat format)
        {
            password = EncodePassword(password, passwordKey, format);
            return password.ToUpper() == dbpassword.ToUpper();
        }


        /// <summary>
        /// 获取密码key
        /// </summary>
        /// <returns></returns>
        public static string GetPasswordKey()
        {
            //TODO
            //var cryptoProvider = new RNGCryptoServiceProvider();
            var cryptoProvider = RandomNumberGenerator.Create();
            byte[] key = new byte[16];
            cryptoProvider.GetBytes(key);
            return Convert.ToBase64String(key);
        }

        /// <summary>
        /// 加密密码
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordKey"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string EncodePassword(string password, string passwordKey, CommonEnum.PasswordFormat format)
        {
            if (password == null)
                return null;
            if (format == CommonEnum.PasswordFormat.Clear)
                return password;

            byte[] passwordBytes = Encoding.Unicode.GetBytes(password);
            byte[] keyBytes = Convert.FromBase64String(passwordKey);
            byte[] keyedBytes = new byte[passwordBytes.Length + keyBytes.Length];

            if (format == CommonEnum.PasswordFormat.Hashed)
            {
                Array.Copy(keyBytes, keyedBytes, keyBytes.Length);
                Array.Copy(passwordBytes, 0, keyedBytes, keyBytes.Length, passwordBytes.Length);
                return HashPasswordBytes(keyBytes, keyedBytes);
            }
            else if (format == CommonEnum.PasswordFormat.MD5)
            {
                ///return MD5PasswordBytes(keyBytes, passwordBytes);
                return MD5Password(password);
            }

            return string.Empty;
        }

        /// <summary>
        /// HASH加密
        /// </summary>
        /// <param name="key"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static string HashPasswordBytes(byte[] key, byte[] bytes)
        {
            HMACSHA256 hash = new HMACSHA256();

            if (hash is KeyedHashAlgorithm)
            {
                KeyedHashAlgorithm keyedHash = hash as KeyedHashAlgorithm;

                keyedHash.Key = key;
            }
            return Convert.ToBase64String(hash.ComputeHash(bytes));
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="key"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private static string MD5PasswordBytes(byte[] key, byte[] bytes)
        {
            //HashAlgorithm hash = HashAlgorithm.Create("MD5");
            HMACMD5 hash = new HMACMD5();

            if (hash is KeyedHashAlgorithm)
            {
                KeyedHashAlgorithm keyedHash = hash as KeyedHashAlgorithm;

                keyedHash.Key = key;
            }
            return Convert.ToBase64String(hash.ComputeHash(bytes));
        }

        public static string MD5Password(string str)
        {
            string cl = str;
            string pwd = "";
            MD5 md5 = MD5.Create();//实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符
                pwd = pwd + s[i].ToString("X2");
            }
            Console.WriteLine(pwd);
            return pwd;
        }

        /// <summary>
        /// 比较数据的差异
        /// </summary>
        /// <param name="oldValues"></param>
        /// <param name="newValues"></param>
        /// <param name="adds"></param>
        /// <param name="dels"></param>
        /// <param name="updates"></param>
        public static void CompareList(List<int> oldValues, List<int> newValues, ref List<int> adds, ref List<int> dels,
            ref List<int> updates)
        {
            dels = oldValues.Except(newValues).ToList();
            adds = newValues.Except(oldValues).ToList();
            updates = oldValues.Intersect(newValues).ToList();
        }

        #region 生成签名
        /// <summary>
        ///  根据请求参数和key生成签名
        /// </summary>
        /// <param name="parameters">参数键值对结合</param>
        /// <param name="partnerkey">密钥KEY</param>
        /// <returns></returns>
        public static string CreateSign(SortedDictionary<string, string> parameters, string partnerkey)
        {
            StringBuilder sb = new StringBuilder();

            //拼接成字符串stringA
            foreach (var item in parameters)
            {
                // 如果参数的值为空不参与签名
                if (null != item.Value && "".CompareTo(item.Value) != 0 && "sign".CompareTo(item.Key) != 0 && "key".CompareTo(item.Key) != 0)
                {
                    sb.Append(item.Key + "=" + item.Value + "&");
                }
            }
            //在stringA最后拼接上key得到stringSignTemp字符串
            sb.Append("key=" + partnerkey);
            // MD5运算，再将得到的字符串所有字符转换为大写
            return GetMD5(sb.ToString(), "UTF-8").ToUpper();
        }

        /// <summary>
        /// 获取大写的MD5签名结果
        /// </summary>
        /// <param name="encypStr">待加密字符串</param>
        /// <param name="charset">码格式</param>
        /// <returns>加密后字符串</returns>
        private static string GetMD5(string encypStr, string charset)
        {
            string retStr;
            //MD5CryptoServiceProvider m5 = new MD5CryptoServiceProvider();
            MD5 m5 = MD5.Create();
            //创建md5对象
            byte[] inputBye;
            byte[] outputBye;

            //使用GB2312编码方式把字符串转化为字节数组．
            try
            {
                inputBye = Encoding.GetEncoding(charset).GetBytes(encypStr);
            }
            catch (Exception)
            {
                inputBye = Encoding.GetEncoding("GB2312").GetBytes(encypStr);
            }
            outputBye = m5.ComputeHash(inputBye);

            retStr = BitConverter.ToString(outputBye);
            retStr = retStr.Replace("-", "").ToUpper();
            return retStr;
        }
        #endregion 

        /// <summary>
        /// json和任意类型之间的转换扩展
        /// </summary>
        /// <typeparam name="Tto">转换成何种类型如json对应的是string，可以转换成List类型，字典类型，实体类型等</typeparam>
        /// <typeparam name="Tsource">数据源是什么类型的，如把List<string>转换成json，数据源类型就是List<string></typeparam>
        /// <param name="para">数据源参数</param>
        /// <returns></returns>
        public static TTo JsonConvertExtend<TTo, TSource>(TSource para) where TTo : class
        {
            try
            {
                string json = typeof(TSource) == typeof(string) ? (para as string) : JsonConvert.SerializeObject(para);
                TTo t = typeof(TTo) == typeof(string) ? (json as TTo) : JsonConvert.DeserializeObject<TTo>(json);
                return t;
            }
            catch
            {
                return default(TTo);
            }
        }
    }
}
