using System;
using System.Collections.Generic;
using System.Text;

namespace Grpc.MicroService
{
    /// <summary>
    /// 全局共通枚举定义
    /// </summary>
    public class CommonEnum
    {
        /// <summary>
        /// 描述用户密码的加密格式。
        /// </summary>
        public enum PasswordFormat
        {

            /// <summary>
            /// 密码未加密。
            /// </summary>
            Clear = 0,

            /// <summary>
            /// 密码使用 SHA1 哈希算法进行单向加密。
            /// </summary>
            Hashed = 1,

            /// <summary>
            /// MD5
            /// </summary>
            MD5 = 2,
        }
    }
}
