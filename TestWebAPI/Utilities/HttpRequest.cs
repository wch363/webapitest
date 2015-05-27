using System;
using System.Net;
using System.Net.Http;
using Should;
using Newtonsoft.Json.Linq;
using System.Net.Http.Formatting;
using System.Collections.Generic;

namespace TestWebAPI.Utilities
{
    public class HttpService
    {
        private static HttpClient client = new HttpClient();
        private HttpRequestMessage request = new HttpRequestMessage();
        public Dictionary<string, string> Headers
        {
            set
            {
                foreach (KeyValuePair<string, string> entry in value)
                    request.Headers.Add(entry.Key, entry.Value);
            }
        }

        public dynamic DoPost(string api, JObject content)
        {
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri(TestContext.URL + api);
            request.Headers.Add("X-Token", TestContext.X_TOKEN);
            request.Content = new ObjectContent<JObject>(content, new JsonMediaTypeFormatter());
            HttpResponseMessage response = client.SendAsync(request).Result;
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
            dynamic result = response.Content.ReadAsAsync<dynamic>().Result;

            return result;
        }

        public dynamic DoGet(string api)
        {
            request.Method = HttpMethod.Get;
            request.RequestUri = new Uri(TestContext.URL + api);
            request.Headers.Add("X-Token", TestContext.X_TOKEN);
            HttpResponseMessage response = client.SendAsync(request).Result;
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
            dynamic result = response.Content.ReadAsAsync<dynamic>().Result;

            return result;
        }

        public HttpResponseMessage Service(HttpRequestMessage request)
        {
            request.Headers.Remove("X-Token");
            request.Headers.Add("X-Token", TestContext.X_TOKEN);
            HttpResponseMessage response = client.SendAsync(request).Result;
            return response;
        }
    }
}
