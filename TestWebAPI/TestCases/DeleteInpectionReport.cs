using System;
using Should;
using TechTalk.SpecFlow;
using System.Collections.Specialized;
using TestWebAPI.Lib;

namespace TestWebAPI.TestCases
{
    [Binding]
    public class DeleteInpectionReport
    {
        private string autoid;
        private InspectionReport inspect = new InspectionReport();

        [Given("Create a new Inspection Report")]
        public void GivenInspectionReport(Table table)
        {
            NameValueCollection inputData = inspect.MetaService.ConvertSimpleTable(table);
            autoid = inspect.Save(inputData, false);
        }

        [When("Send a request to api:'(.*)' to delete this Inspection Report")]
        public void WhenSendRequesttoAPI(string api)
        {
            inspect.Delete(autoid);
        }

        [Then("Target Inspection Report should be deleted")]
        public void ThenInspectionReportShouldBeDeleted()
        {
        }
    }
}
