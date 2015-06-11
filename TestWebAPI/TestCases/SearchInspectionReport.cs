using System;
using System.Linq;
using Should;
using System.Collections.Specialized;
using System.Collections.Generic;
using TechTalk.SpecFlow;
using TestWebAPI.Lib;
using Newtonsoft.Json.Linq;

namespace TestWebAPI.TestCases
{
    [Binding]
    public class SearchInspectionReport
    {
        private IEnumerable<string> searchResultAutoIds;
        private static List<NameValueCollection> existingInspections;
        private InspectionReport inspect = new InspectionReport();
        private static Dictionary<string, string> rowAutoIDs = new Dictionary<string, string>();

        [Given("Exist following Inspection Reports")]
        public void GivenFollowingInspectionReport(Table tbl)
        {
            if (existingInspections == null)
            {
                int i = 1;
                existingInspections = inspect.MetaService.ConvertComplexTable(tbl);
                foreach (NameValueCollection inspection in existingInspections)
                {
                    string id = inspect.Save(inspection, false);
                    rowAutoIDs.Add("#" + i++, id);
                }
            }
        }

        [When("Send a request to api:(.*)")]
        public void SendRequestToApi(string api, Table tbl)
        {
            string queryString = inspect.MetaService.ConvertSearchTable(tbl);
            JArray searchResult = inspect.Search(queryString);
            searchResultAutoIds = searchResult.Children()["InspectionReportAutoID"].Values<string>();
            TestContext.RegisteCleanUpData("inspectionreport", rowAutoIDs.Values.ToArray<string>());
        }

        [Then("These Inspection Reports should be searched out:(.*)")]
        public void ThenSearchResultShouldInclude(string rowNumbers)
        {
            foreach (string num in rowNumbers.Split(','))
                searchResultAutoIds.ShouldContain<string>(rowAutoIDs[num]);
        }

        [Then("These Inspection Reports should not be searched out:(.*)")]
        public void ThenSearchResultShouldExclude(string rowNumbers)
        {
            foreach (string num in rowNumbers.Split(','))
                searchResultAutoIds.ShouldNotContain<string>(rowAutoIDs[num]);
        }
    }
}