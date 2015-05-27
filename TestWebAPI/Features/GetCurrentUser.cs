using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Should;
using TechTalk.SpecFlow;
using TestWebAPI.Utilities;
using System.Collections.Generic;

namespace TestWebAPI.Features
{
    [Binding]
    public class GetCurrentUser
    {
        private JObject result;
        
        [When("Send a request to api:'(.*)'")]
        public void WhenSendRequesttoAPI(string apiName)
        {
            //Dictionary<string,string> dic=new Dictionary<string,string>();
            CipEntity cip = new CipEntity("inspectionreport");
            string jo= cip.GetClassificationNodeValue("WBSCode_10005","c3  - c3");
        }

        [Then("Information of current user should be returned")]
        public void ThenInforofCurrentUserShouldBeReturned()
        {
        }
    }
}
