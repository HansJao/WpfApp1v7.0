using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WpfApp1.Utility
{
    public class JsonHelper : ISerializer
    {
        private static JsonHelper _jsonSerializer = null;
        private static JsonSerializerSettings jsSet = null;

        /// <summary>
        /// 取得處理Json序列化的執行個體
        /// </summary>
        public static JsonHelper Instance
        {
            get
            {
                if (_jsonSerializer == null)
                {
                    _jsonSerializer = new JsonHelper();
                }

                return _jsonSerializer;
            }
        }        

        /// <summary>
        /// 將物件轉換為Json字串
        /// </summary>
        /// <param name="obj">要轉換的物件</param>
        /// <returns>Json 字串</returns>
        public string Serialize<T>(T obj)
        {
            string rtnString = JsonConvert.SerializeObject(obj, Formatting.Indented);
            return rtnString;
        }

        /// <summary>
        /// 將物件轉換為Json字串(連同 private 變數一同序列化)
        /// </summary>
        /// <param name="obj">要轉換的物件</param>
        /// <returns>Json 字串</returns>
        public string SerializeAllMembers<T>(T obj)
        {
            string rtnString = JsonConvert.SerializeObject(obj, jsonSerialSetting);
            return rtnString;
        }

        /// <summary>
        /// 將Json字串轉換為泛型物件
        /// </summary>
        /// <typeparam name="T">泛型類型</typeparam>
        /// <param name="jsonString">Json 字串</param>
        /// <returns>泛型物件</returns>
        public T Deserialize<T>(string jsonString)
        {
            T rtnValue = JsonConvert.DeserializeObject<T>(jsonString);
            return rtnValue;
        }

        /// <summary>
        /// 將Json字串轉換為泛型物件(連同 private 變數一同反序列化)
        /// </summary>
        /// <typeparam name="T">泛型類型</typeparam>
        /// <param name="jsonString">Json 字串</param>
        /// <returns>泛型物件</returns>
        public T DeserializeAllMember<T>(string jsonString)
        {
            T rtnValue = JsonConvert.DeserializeObject<T>(jsonString, jsonSerialSetting);
            return rtnValue;
        }

        /// <summary>
        /// 序列化設定, 連同 private 變數一同序列化
        /// </summary>
        private static JsonSerializerSettings jsonSerialSetting
        {
            get
            {
                if (jsSet == null)
                {
                    jsSet = new Newtonsoft.Json.JsonSerializerSettings();
                    jsSet.ContractResolver = new IncludeNonPublicMembersContractResolver();
                }
                return jsSet;
            }
        }

        private class IncludeNonPublicMembersContractResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                .Select(p => base.CreateProperty(p, memberSerialization))
                            .Union(type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                       .Select(f => base.CreateProperty(f, memberSerialization)))
                            .ToList();
                props.ForEach(p => { p.Writable = true; p.Readable = true; });
                return props;
            }
        }
    }
}
