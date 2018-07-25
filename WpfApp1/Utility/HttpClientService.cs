using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace WpfApp1.Utility
{
    public class HttpClientService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClientService" /> class.
        /// </summary>
        /// <param name="format">序列化格式</param>
        public HttpClientService(SerializerFormat format)
        {
            this._serializer = this.GetSerialzar(format);

            this.MaxResponseContentBufferSize = int.MaxValue;

            this.Timeout = TimeSpan.FromSeconds(100);

            this.InitMediaTypeHeaderValue(format);
        }

        /// <summary>
        /// 序列化格式列舉
        /// </summary>
        public enum SerializerFormat
        {
            Json,
            Xml
        }

        /// <summary>
        /// The Http ResponseBody
        /// </summary>
        public string ResponseBody { get; private set; }

        /// <summary>
        /// The Http Status Code
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; private set; }

        /// <summary>
        /// Gets or sets the maximum number of bytes to buffer when reading the response content.
        /// </summary>
        public long MaxResponseContentBufferSize { get; set; }

        /// <summary>
        /// Gets or sets the number of milliseconds to wait before the request times out.
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// The Media Type Header Value
        /// </summary>
        public MediaTypeWithQualityHeaderValue MediaTypeHeaderValue { get; set; }

        /// <summary>
        /// Gets or sets the http header
        /// </summary>
        public Dictionary<string, string> HttpHeader { get; set; }

        /// <summary>
        /// 實作序列化的物件
        /// </summary>
        protected ISerializer _serializer;

        /// <summary>
        /// 將 GET 要求傳送至指定的 URI
        /// </summary>
        /// <typeparam name="TResponse">The Response Entity</typeparam>
        /// <param name="uri">Request Uri</param>
        /// <returns>The Response Entity</returns>
        public virtual TResponse Get<TResponse>(Uri uri) where TResponse : class
        {
            if (uri == null) throw new ArgumentNullException("uri");

            using (var client = new System.Net.Http.HttpClient())
            {
                client.MaxResponseContentBufferSize = this.MaxResponseContentBufferSize;

                client.Timeout = this.Timeout;

                this.SetHttpHeader(client);

                this.SetMediaTypeHeaderValue(client);

                var response = client.GetAsync(uri).Result;

                this.ResponseBody = response.Content.ReadAsStringAsync().Result;

                this.HttpStatusCode = response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    return this._serializer.Deserialize<TResponse>(this.ResponseBody);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 將 POST 要求傳送至指定的 URI (不含內容)
        /// </summary>
        /// <typeparam name="TResponse">The Response Entity</typeparam>
        /// <param name="uri">Request Uri</param>
        /// <returns>The Response Entity</returns>
        public TResponse Post<TResponse>(Uri uri) where TResponse : class
        {
            return this.Post<TResponse>(uri, new System.Net.Http.StringContent(string.Empty));
        }

        /// <summary>
        /// 將 POST 要求傳送至指定的 URI
        /// </summary>
        /// <typeparam name="TResponse">The Response Entity</typeparam>
        /// <param name="uri">Request Uri</param>
        /// <param name="content">POST的內容</param>
        /// <returns>The Response Entity</returns>
        public TResponse Post<TResponse>(Uri uri, string content) where TResponse : class
        {
            return this.Post<TResponse>(uri, new System.Net.Http.StringContent(content, Encoding.UTF8, this.MediaTypeHeaderValue.MediaType));
        }

        /// <summary>
        /// 將 POST 要求傳送至指定的 URI
        /// </summary>
        /// <typeparam name="TResponse">The Response Entity</typeparam>
        /// <param name="uri">Request Ur</param>
        /// <param name="httpContent">The HttpContent</param>
        /// <returns>The Response Entity</returns>
        public virtual TResponse Post<TResponse>(Uri uri, System.Net.Http.HttpContent httpContent) where TResponse : class
        {
            if (uri == null) throw new ArgumentNullException("uri");

            if (httpContent == null) throw new ArgumentNullException("httpContent");

            using (var client = new System.Net.Http.HttpClient())
            {
                client.MaxResponseContentBufferSize = this.MaxResponseContentBufferSize;

                client.Timeout = this.Timeout;

                this.SetHttpHeader(client);

                this.SetMediaTypeHeaderValue(client);

                var response = client.PostAsync(uri, httpContent).Result;

                this.ResponseBody = response.Content.ReadAsStringAsync().Result;

                this.HttpStatusCode = response.StatusCode;

                if (response.IsSuccessStatusCode)
                {
                    return this._serializer.Deserialize<TResponse>(this.ResponseBody);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 將 DELETE 要求傳送至指定的 URI
        /// </summary>
        /// <param name="uri">The RequestUri</param>
        public void Delete(Uri uri)
        {
            if (uri == null) throw new ArgumentNullException("uri");

            using (var client = new System.Net.Http.HttpClient())
            {
                client.MaxResponseContentBufferSize = this.MaxResponseContentBufferSize;

                client.Timeout = this.Timeout;

                this.SetHttpHeader(client);

                this.SetMediaTypeHeaderValue(client);

                var response = client.DeleteAsync(uri).Result;

                this.ResponseBody = response.Content.ReadAsStringAsync().Result;

                this.HttpStatusCode = response.StatusCode;
            }
        }

        /// <summary>
        /// 將 PUT 要求傳送至指定的 URI
        /// </summary>
        /// <param name="uri">The RequestUri</param>
        /// <param name="httpContent">The HttpContent</param>
        public void Put(Uri uri, System.Net.Http.HttpContent httpContent)
        {
            if (uri == null) throw new ArgumentNullException("uri");

            if (httpContent == null) throw new ArgumentNullException("httpContent");

            using (var client = new System.Net.Http.HttpClient())
            {
                client.MaxResponseContentBufferSize = this.MaxResponseContentBufferSize;

                client.Timeout = this.Timeout;

                this.SetHttpHeader(client);

                this.SetMediaTypeHeaderValue(client);

                var response = client.PutAsync(uri, httpContent).Result;

                this.ResponseBody = response.Content.ReadAsStringAsync().Result;

                this.HttpStatusCode = response.StatusCode;
            }
        }

        /// <summary>
        /// 設定 MediaTypeHeaderValue
        /// </summary>
        /// <param name="client">The HttpClient</param>
        protected void SetMediaTypeHeaderValue(System.Net.Http.HttpClient client)
        {
            bool hasHeaderValue = this.MediaTypeHeaderValue != null;

            if (hasHeaderValue)
            {
                client.DefaultRequestHeaders.Accept.Add(MediaTypeHeaderValue);
            }
        }

        /// <summary>
        /// 設定 Http header
        /// </summary>
        /// <param name="client">The HttpClient</param>
        protected void SetHttpHeader(System.Net.Http.HttpClient client)
        {
            bool hasHttpHeader = this.HttpHeader != null && this.HttpHeader.Any();

            if (hasHttpHeader)
            {
                foreach (var item in this.HttpHeader)
                {
                    client.DefaultRequestHeaders.Add(item.Key, item.Value);
                }
            }
        }

        /// <summary>
        /// 取得實作序列化的物件
        /// </summary>
        /// <param name="format">序列化的格式</param>
        /// <returns>實作序列化物件</returns>
        private ISerializer GetSerialzar(SerializerFormat format)
        {
            switch (format)
            {
                case SerializerFormat.Json:
                    return JsonHelper.Instance;
                //case SerializerFormat.Xml:
                //    return XmlHelper.Instance;
                default:
                    throw new NotImplementedException("Serializer Format Not Implemented.");
            }
        }

        /// <summary>
        /// 初始化MediaTypeWithQualityHeaderValue
        /// </summary>
        /// <param name="format">序列化的格式</param>
        private void InitMediaTypeHeaderValue(SerializerFormat format)
        {
            switch (format)
            {
                case SerializerFormat.Json:
                    this.MediaTypeHeaderValue = new MediaTypeWithQualityHeaderValue("application/json");
                    break;
                case SerializerFormat.Xml:
                    this.MediaTypeHeaderValue = new MediaTypeWithQualityHeaderValue("application/xml");
                    break;
            }
        }
    }
}
