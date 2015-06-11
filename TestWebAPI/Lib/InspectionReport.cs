using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Should;
using System.Collections.Specialized;
using System.Collections.Generic;
using TechTalk.SpecFlow;

namespace TestWebAPI.Lib
{
    public class InspectionReport : EntityRecord
    {
        private JArray requestParameter;

        public InspectionReport()
            : base("inspectionreport") { }

        /// <summary>
        /// Save inspection report with line items
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <param name="isUpdate"></param>
        /// <returns></returns>
        public override string SaveAll(Dictionary<string, NameValueCollection> data, bool isUpdate)
        {
            // data record structure
            string childEntityName = (string)MetaService.MetaData["LineItemEntityName"];
            string schenaName = (string)MetaService.MetaData["LineItemSchemaName"];
            if (requestParameter == null)
                requestParameter = new JArray(new JObject(
                new JProperty("EntityName", childEntityName),
                new JProperty("SchemaName", schenaName),
                new JProperty("Type", "C")));
            HttpResponseMessage response = HttpService.Instance.DoPost("Organization/RetrieveAll/inspectionreport/?id=", requestParameter);
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
            JObject recordStructure = response.Content.ReadAsAsync<dynamic>().Result;

            // user input data
            Dictionary<string, string> sectionAutoid = MetaService.GetLineItemSectionAutoID();
            JArray lineItems = (JArray)recordStructure["Children"][schenaName];
            foreach (KeyValuePair<string, NameValueCollection> entry in data)
            {
                if (string.Compare(entry.Key, "parent", true) == 0)
                    for (int i = 0; i < entry.Value.Count; i++)
                        recordStructure["Attributes"][entry.Value.GetKey(i)] = entry.Value.Get(i);
                else
                {
                    JToken lineitem = recordStructure.SelectToken("Children." + schenaName + "[0]").DeepClone();
                    for (int i = 0; i < entry.Value.Count; i++)
                        lineitem["Attributes"][entry.Value.GetKey(i)] = entry.Value.Get(i);
                    lineitem["ActionType"] = isUpdate == true ? "U" : "A";
                    lineitem["Attributes"]["LineItemSectionAutoID"] = sectionAutoid[entry.Key];
                    lineItems.Add(lineitem);
                }
            }
            recordStructure["ActionType"] = isUpdate == true ? "U" : "A";

            HttpResponseMessage saveResponse = HttpService.Instance.DoPost("Organization/Save/InspectionReport", recordStructure);
            saveResponse.StatusCode.ShouldEqual(HttpStatusCode.OK);
            JObject result = saveResponse.Content.ReadAsAsync<JObject>().Result;
            string autoid = result.SelectToken("Results[0].RecordID").Value<string>();

            return autoid;
        }

        /// <summary>
        /// Get an inspection report with line items
        /// </summary>
        /// <param name="autoid"></param>
        /// <returns></returns>
        public override JObject GetAll(string autoid)
        {
            if (requestParameter == null)
                requestParameter = new JArray(new JObject(
                new JProperty("EntityName", (string)MetaService.MetaData["LineItemEntityName"]),
                new JProperty("SchemaName", (string)MetaService.MetaData["LineItemSchemaName"]),
                new JProperty("Type", "C")));
            HttpResponseMessage response = HttpService.Instance.DoPost("Organization/RetrieveAll/inspectionreport/?id=" + autoid, requestParameter);
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
            JObject content = response.Content.ReadAsAsync<dynamic>().Result;

            return content;
        }
    }
}