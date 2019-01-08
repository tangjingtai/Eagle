using System;
using System.Collections.Generic;
using System.Text;

namespace Util
{
    /// <summary>
    /// 系统扩展 - HashSet
    /// </summary>
    public static partial class Extensions
    {
        public static void SafeAdd<T>(this ISet<T> hashSet, T obj)
        {
            if (!hashSet.Contains(obj))
            {
                hashSet.Add(obj);
            }

        }

        public static void SafeAdd<TK, TV>(this IDictionary<TK, TV> dic, TK key, TV value)
        {
            if (!dic.ContainsKey(key))
            {
                dic.Add(key, value);
            }

        }



        public static TV SafeGet<TK, TV>(this IDictionary<TK, TV> dic, TK key, TV defaultValue = default(TV))
        {
            if (dic.ContainsKey(key))
                return dic[key];
            return defaultValue;
        }


        public static void SafeAddRange<T>(this ISet<T> hashSet, IEnumerable<T> objs)
        {
            foreach (var obj in objs)
            {
                hashSet.SafeAdd(obj);
            }
        }

        /// <summary>
        /// MD5加密字符串
        /// </summary>
        /// <param name="sourceStr">要加密的字符串</param>
        /// <returns>MD5加密后的字符串</returns>
        public static string ToMd5Code(this string sourceStr)
        {
            string sRet = string.Empty;
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5CryptoServiceProvider.Create();
            byte[] btRet;
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            System.IO.StreamWriter sw = new System.IO.StreamWriter(ms);
            System.IO.StreamReader sr = new System.IO.StreamReader(ms);

            if (sourceStr == null)
            {
                sourceStr = string.Empty;
            }

            sw.Write(sourceStr);
            sw.Flush();
            ms.Seek(0, System.IO.SeekOrigin.Begin);

            btRet = md5.ComputeHash(ms);
            ms.SetLength(0);
            sw.Flush();

            for (int i = 0; i < btRet.Length; i++)
            {
                sw.Write("{0:X2}", btRet[i]);
            }
            sw.Flush();
            ms.Seek(0, System.IO.SeekOrigin.Begin);
            sRet = sr.ReadToEnd();

            sw.Close();
            sw.Dispose();
            sr.Close();
            sr.Dispose();
            ms.Close();
            ms.Dispose();

            return sRet;
        }
    }
}
