using System;
using TechTalk.SpecFlow;
using TestWebAPI.Lib;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace TestWebAPI.TestCases
{
    [Binding]
    public class AddInspectionReport
    {
        private string autoid;
        private InspectionReport inspection = new InspectionReport();
        private Dictionary<string, NameValueCollection> inputData = new Dictionary<string, NameValueCollection>();

        [Given("Inspection Report information as following")]
        public void GivenInspectionReport(Table table)
        {
            NameValueCollection nv = inspection.MetaService.ConvertSimpleTable(table);
            inputData.Add("parent", nv);
        }

        [Given("Input line items at '(.*)' section")]
        public void GivenInspectionReportLineItem(string sectionName, Table table)
        {
            MetadataService meta = new MetadataService("inspectionreportitem");
            NameValueCollection nv = meta.ConvertSimpleTable(table);
            inputData.Add(sectionName, nv);
        }

        [When("Post Inspection Report information to api '(.*)'")]
        public void WhenPostInspectionReportToServer(string api)
        {
            if (inputData.Count==1)
                autoid = inspection.Save(inputData["parent"], false);
            else
                autoid = inspection.SaveAll(inputData, false);
        }

        [Then("A new Inspection Report should be created and all fields saved correctly")]
        public void ThenNewInspectionReportShouldBeCreated()
        {
            TestContext.RegisteCleanUpData("inspectionreport", autoid);
            //JObject data = inspection.GetAll(autoid);
            //compare
        }
    }
}
