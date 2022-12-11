using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Core.Extension
{
    /// <summary>
    /// Linq扩展
    /// </summary>
    [SuppressSniffer]
    public static class LinqExtension
    {
        /// <summary>
        /// 是否都包含
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first">第一个列表</param>
        /// <param name="secend">第二个列表</param>
        /// <returns></returns>
        public static bool ContainsAll<T>(this List<T> first, List<T> secend)
        {
            return secend.All(s => first.Any(f => f.Equals(s)));
        }
    }
}
