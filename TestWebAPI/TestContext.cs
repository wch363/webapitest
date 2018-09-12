using System;
using Should;
using System.Configuration;
using TechTalk.SpecFlow;
using System.Net.Http;
using TestWebAPI.Lib;
using System.Net;
using System.Collections.Generic;

namespace TestWebAPI
{
    [Binding]
    public class TestContext
    {
        public static readonly string NAME, PASSWORD, URL;
        private static string x_token = string.Empty;
        //login authentication
        public static string X_TOKEN { get { return Utility.Base64Encode(x_token); } }
        //entity data records to be deleted after test run.
        private static Dictionary<string, List<string>> cleanUpData = new Dictionary<string, List<string>>();

        static TestContext()
        {
            // Hello World
            // 2018-09-12 13:08到此一游
            NAME = ConfigurationManager.AppSettings["UserName"];
            PASSWORD = ConfigurationManager.AppSettings["PassWord"];
            URL = ConfigurationManager.AppSettings["WebAPIURL"] + "api/";
        }

        [BeforeTestRun]
        public static void SetUpTestContext()
        {
            string requestContent = "userName=" + NAME + "&password=" + PASSWORD;
            HttpResponseMessage response = HttpService.Instance.DoPost("Authentication/Login", requestContent);
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
            x_token = response.Content.ReadAsAsync<dynamic>().Result;
        }

        [AfterTestRun]
        public static void CleanUpTestContext()
        {
            foreach (KeyValuePair<string, List<string>> entry in cleanUpData)
                foreach (string id in entry.Value)
                    HttpService.Instance.DoDelete(entry.Key, id);

            HttpResponseMessage response = HttpService.Instance.DoGet("Authentication/Logout");
            response.StatusCode.ShouldEqual(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Registe data records need to be deleted after test run
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="recordAutoids"></param>
        public static void RegisteCleanUpData(string entityName, params string[] recordAutoids)
        {
            List<string> idList;
            cleanUpData.TryGetValue(entityName, out idList);
            if (idList == null)
                cleanUpData.Add(entityName, new List<string>(recordAutoids));
            else
            {
                for (int i = 0; i < recordAutoids.Length; i++)
                    if (!idList.Contains(recordAutoids[i]))
                        idList.Add(recordAutoids[i]);
            }
        }
    }
}