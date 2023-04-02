using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdmin.Core
{

    /// <summary>
    /// object拓展
    /// </summary>
    public static class ObjectExtension
    {
        /// <summary>
        /// json字符串序列化
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static object ToObject(this string json)
        {
            return string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject(json);
        }

        /// <summary>
        /// json字符串序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T ToObject<T>(this string json)
        {
            if (json != null)
            {
                json = json.Replace("&nbsp;", "");
                return JsonConvert.DeserializeObject<T>(json);
            }
            else return default;
        }

        /// <summary>
        /// json字符串序列化
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static JObject ToJObject(this string json)
        {
            return json == null ? JObject.Parse("{}") : JObject.Parse(json.Replace("&nbsp;", ""));
        }


    }

}
