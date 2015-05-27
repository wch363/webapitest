using System;
using System.Configuration;
using TechTalk.SpecFlow;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using TestWebAPI.Utilities;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace TestWebAPI
{
    [Binding]
    public class TestContext
    {
        public static readonly string NAME;
        private static readonly string PASSWORD;
        public static readonly string URL;
        private static string x_token = string.Empty;
        public static string X_TOKEN { get { return Util.Base64Encode(x_token); } }

        static TestContext()
        {
            NAME = ConfigurationManager.AppSettings["UserName"];
            PASSWORD = ConfigurationManager.AppSettings["PassWord"];
            URL = ConfigurationManager.AppSettings["WebAPIURL"];
            URL = URL.LastIndexOf("/") == URL.Length - 1 ? URL + "api/" : URL + "/api/";
        }

        [BeforeScenario]
        public void Login()
        {
            dynamic account = new JObject();
            account.userName = NAME;
            account.password = PASSWORD;

            HttpService request = new HttpService();
            x_token = request.DoPost("Authentication/Login", account);
        }

        [AfterScenario]
        public void Logout()
        {
        }
    }
}