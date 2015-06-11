using System;
using System.Linq;
using Should;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System.Collections.Specialized;
using TechTalk.SpecFlow;

namespace TestWebAPI.Lib
{
    public abstract class EntityRecord
    {
        private string entityName;
        private MetadataService meta;
        public MetadataService MetaService
        {
            get
            {
                if (meta == null) meta = new MetadataService(entityName);
                return meta;
            }
        }

        public EntityRecord(string entityName)
        {
            this.entityName = entityName;
        }
        
        /// <summary>
        /// Save an entity record
        /// </summary>
        /// <param name="recordData"></param>
        /// <param name="isUpdate"></param>
        /// <returns></returns>
        public string Save(NameValueCollection recordData, bool isUpdate)
        {
            HttpResponseMessage response = HttpService.Instance.DoPost("Organization/RetrieveAll/inspectionreport/?id=", new JObject());
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
            JObject recordStructure = response.Content.ReadAsAsync<dynamic>().Result;
            for (int i = 0; i < recordData.Count; i++)
                recordStructure["Attributes"][recordData.GetKey(i)] = recordData.Get(i);
            recordStructure["ActionType"] = isUpdate == true ? "U" : "A";

            HttpResponseMessage saveResponse = HttpService.Instance.DoPost("Organization/Save/InspectionReport", recordStructure);
            saveResponse.StatusCode.ShouldEqual(HttpStatusCode.OK);
            JObject result = saveResponse.Content.ReadAsAsync<JObject>().Result;
            string autoid = result.SelectToken("Results[0].RecordID").Value<string>();

            return autoid;
        }

        /// <summary>
        /// Save an entity record with child
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <param name="isUpdate"></param>
        /// <returns></returns>
        public abstract string SaveAll(Dictionary<string, NameValueCollection> multiSectionData, bool isUpdate);

        /// <summary>
        /// Get entity record with child
        /// </summary>
        /// <param name="autoid"></param>
        /// <returns></returns>
        public abstract JObject GetAll(string autoid);

        /// <summary>
        /// Search entity records
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <returns></returns>
        public JArray Search(string searchCriteria)
        {
            HttpResponseMessage response = HttpService.Instance.DoPost("DynamicPage/GetViewResult/" + entityName, searchCriteria);
            JObject records = response.Content.ReadAsAsync<dynamic>().Result;

            return (JArray)records["rows"];
        }

        /// <summary>
        /// Delete an entity record
        /// </summary>
        /// <param name="autoid"></param>
        public void Delete(string autoid)
        {
            HttpResponseMessage response = HttpService.Instance.DoDelete(entityName, autoid);
            bool result = response.Content.ReadAsAsync<dynamic>().Result;
            result.ShouldBeTrue();
        }
    }
}