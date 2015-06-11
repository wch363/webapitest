using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Should;
using Newtonsoft.Json.Linq;
using System.Net.Http.Formatting;
using System.Collections.Generic;
using System.Web;
using System.Collections.Specialized;

namespace TestWebAPI.Lib
{
    public class HttpService
    {
        private static HttpClient client = new HttpClient();
        private static HttpService instance = new HttpService();
        public static HttpService Instance { get { return instance; } }

        private HttpService() { }

        /// <summary>
        /// Send post request to server, content of request is a json object
        /// </summary>
        /// <param name="api"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public HttpResponseMessage DoPost(string api, JContainer content)
        {
            HttpResponseMessage response;
            using (HttpRequestMessage request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(TestContext.URL + api);
                request.Headers.Add("X-Token", TestContext.X_TOKEN);
                request.Headers.Add("ContentType", "application/json;charset=UTF-8");
                request.Content = new ObjectContent<dynamic>(content, new JsonMediaTypeFormatter());
                response = client.SendAsync(request).Result;
                //response.StatusCode.ShouldEqual(HttpStatusCode.OK);
            }

            return response;
        }

        /// <summary>
        /// Send post request to server, content of request is a query string
        /// </summary>
        /// <param name="api"></param>
        /// <param name="queryString"></param>
        /// <returns></returns>
        public HttpResponseMessage DoPost(string api, string queryString)
        {
            HttpResponseMessage response;
            using (HttpRequestMessage request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(TestContext.URL + api);
                request.Headers.Add("X-Token", TestContext.X_TOKEN);
                request.Headers.Add("ContentType", "application/x-www-form-urlencoded");
                NameValueCollection nv = HttpUtility.ParseQueryString(queryString);
                var values = new List<KeyValuePair<string, string>>();
                foreach (var key in nv.AllKeys)
                    values.Add(new KeyValuePair<string, string>(key, nv[key]));
                    //values.Add(new KeyValuePair<string, string>(key, HttpUtility.UrlEncode(nv[key])));
                request.Content = new FormUrlEncodedContent(values.AsEnumerable());
                response = client.SendAsync(request).Result;
                //response.StatusCode.ShouldEqual(HttpStatusCode.OK);
            }

            return response;
        }

        /// <summary>
        /// Send a get request to server
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        public HttpResponseMessage DoGet(string api)
        {
            HttpResponseMessage response;
            using (HttpRequestMessage request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Get;
                request.RequestUri = new Uri(TestContext.URL + api);
                request.Headers.Add("X-Token", TestContext.X_TOKEN);
                response = client.SendAsync(request).Result;
                //response.StatusCode.ShouldEqual(HttpStatusCode.OK);
            }

            return response;
        }

        /// <summary>
        /// Delete an entity record
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public HttpResponseMessage DoDelete(string entityName, string id)
        {
            HttpResponseMessage response;
            using (HttpRequestMessage request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Delete;
                request.RequestUri = new Uri(TestContext.URL + "Organization/Delete/" + entityName + "/" + id);
                request.Headers.Add("X-Token", TestContext.X_TOKEN);
                response = client.SendAsync(request).Result;
                response.StatusCode.ShouldEqual(HttpStatusCode.OK);
            }

            return response;
        }

        /// <summary>
        /// Send a http request to server
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public HttpResponseMessage Service(HttpRequestMessage request)
        {
            request.Headers.Remove("X-Token");
            request.Headers.Add("X-Token", TestContext.X_TOKEN);
            HttpResponseMessage response = client.SendAsync(request).Result;
            return response;
        }
    }
}
