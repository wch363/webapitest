using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Should;
using System.Collections.Specialized;
using TechTalk.SpecFlow;
using System.Collections.Generic;
using System.Text;

namespace TestWebAPI.Lib
{
    public class MetadataService
    {
        public MetadataService(string entityName)
        {
            this.entityName = entityName;
            HttpResponseMessage response = HttpService.Instance.DoGet("Metadata/GetEntity/" + entityName);
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
            metaData = response.Content.ReadAsAsync<dynamic>().Result;
        }

        /// <summary>
        /// Get dropdown list item value by name
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="attributeName"></param>
        /// <param name="itemName"></param>
        /// <returns></returns>
        private string GetDropdownItemValue(string attributeName, string itemName)
        {
            HttpResponseMessage response;
            string type = (string)Attributes[attributeName]["Type"];
            if (string.Compare(type, "Lookup", true) == 0)
                response = HttpService.Instance.DoGet("Metadata/GetLookupDropdownlist/" + entityName + "?attributeName=" + attributeName);
            else
                response = HttpService.Instance.DoGet("Metadata/GetDropdownlist/" + entityName + "?attributeName=" + attributeName);
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
            JObject piclist = response.Content.ReadAsAsync<dynamic>().Result;
            string value = (from li in piclist["Items"].Children()
                            where string.Compare(li["Text"].Value<string>(), itemName, true) == 0
                            select li["Value"].Value<string>()).First<string>();
            return value;
        }

        /// <summary>
        /// Get classification node value by name
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="attributeName"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        private string GetClassificationNodeValue(string attributeName, string nodeName)
        {
            string value = string.Empty;
            string api = string.Format("Metadata/GetClassificationNodes/{0}?attributeName={1}", entityName, attributeName);
            HttpResponseMessage response = HttpService.Instance.DoGet(api);
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
            JArray nodes = response.Content.ReadAsAsync<JArray>().Result;
            foreach (JToken token in nodes.Children())
            {
                value = GetNodeValue(token, nodeName);
                if (!string.IsNullOrEmpty(value))
                    break;
            }

            return value;
        }

        /// <summary>
        /// Get node value
        /// </summary>
        /// <param name="token"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        private string GetNodeValue(JToken token, string nodeName)
        {
            if (token == null)
                return "";
            if (string.Compare(token["text"].Value<string>().Replace("\"", ""), nodeName, true) == 0)
                return token["id"].Value<string>();

            return GetNodeValue(token["items"].First, nodeName);
        }

        /// <summary>
        /// Get primary key of entity record by primary name
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="attributeName"></param>
        /// <param name="recordPrimaryName"></param>
        /// <returns></returns>
        private string GetPrimaryKeyValue(string attributeName, string recordPrimaryName)
        {
            string refEntity = (string)Attributes[attributeName]["ReferencedEntity"];
            HttpResponseMessage response = HttpService.Instance.DoGet("Metadata/GetEntity/" + refEntity);
            response.StatusCode.ShouldEqual(HttpStatusCode.OK);
            JObject refEntityInfo = response.Content.ReadAsAsync<JObject>().Result;
            JEnumerable<JToken> _attributes = refEntityInfo["Attributes"].Children();
            string primaryName = (from attr in _attributes
                                  where string.Compare((string)attr["IsPrimaryName"], "true", true) == 0
                                  select attr.Value<string>("PhysicalName") ?? "").First<string>();
            string primaryKey = (from attr in _attributes
                                 where string.Compare(attr.Value<string>("IsPKAttribute"), "True", true) == 0
                                 select attr.Value<string>("PhysicalName") ?? "").First<string>();

            HttpResponseMessage response2 = HttpService.Instance.DoPost("DynamicPage/GetViewResult/" + refEntity, "_QueryType=0&_Page=0&_PageSize=0");
            JObject records = response2.Content.ReadAsAsync<dynamic>().Result;
            string id = (from record in records["rows"].Children()
                         where string.Compare(record.Value<string>(primaryName), recordPrimaryName, true) == 0
                         select record.Value<string>(primaryKey)).First<string>();

            return id;
        }

        /// <summary>
        /// Convert UI display value in input table
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="inputTable"></param>
        /// <returns></returns>
        public NameValueCollection ConvertSimpleTable(Table inputTable)
        {
            string key, value, attrType, convertedValue;
            NameValueCollection result = new NameValueCollection();

            for (int i = 0; i < inputTable.Rows.Count; i++)
            {
                attrType = (string)Attributes[inputTable.Rows[i][0]]["Type"];
                key = inputTable.Rows[i][0];
                value = inputTable.Rows[i][1];
                switch (attrType.ToLower())
                {
                    case "picklist":
                    case "status":
                        convertedValue = GetDropdownItemValue(key, value);
                        break;
                    case "classification":
                        convertedValue = GetClassificationNodeValue(key, value);
                        break;
                    case "lookup":
                        convertedValue = GetPrimaryKeyValue(key, value);
                        break;
                    default:
                        convertedValue = value;
                        break;
                }
                result.Add(key, convertedValue);
            }

            return result;
        }

        /// <summary>
        /// Convert UI display value in input table(with multiple data rows)
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="inputTable"></param>
        /// <returns></returns>
        public List<NameValueCollection> ConvertComplexTable(Table inputTable)
        {
            string key, value, attrType, convertedValue;
            List<NameValueCollection> result2 = new List<NameValueCollection>();
            List<string> heads = inputTable.Header.ToList<string>();

            for (int i = 0; i < inputTable.Rows.Count; i++)
            {
                NameValueCollection result = new NameValueCollection();
                for (int j = 0; j < heads.Count; j++)
                {
                    key = heads[j];
                    if (key.Equals("#"))
                        continue;
                    value = inputTable.Rows[i][j];
                    attrType = (string)Attributes[heads[j]]["Type"];
                    switch (attrType.ToLower())
                    {
                        case "picklist":
                        case "status":
                            convertedValue = GetDropdownItemValue(key, value);
                            break;
                        case "classification":
                            convertedValue = GetClassificationNodeValue(key, value);
                            break;
                        case "lookup":
                            convertedValue = GetPrimaryKeyValue(key, value);
                            break;
                        default:
                            convertedValue = value;
                            break;
                    }
                    result.Add(key, convertedValue);
                }
                result2.Add(result);
            }

            return result2;
        }

        /// <summary>
        /// Convert serch conditions table to a query string
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="inputTable"></param>
        /// <returns></returns>
        public string ConvertSearchTable(Table inputTable)
        {
            StringBuilder sb = new StringBuilder();
            string key, value, attrType, convertedValue, tempKey;
            NameValueCollection result = new NameValueCollection();

            for (int i = 0; i < inputTable.Rows.Count; i++)
            {
                key = inputTable.Rows[i][0];
                value = inputTable.Rows[i][1];
                if (key.IndexOf("A0_") != 0)
                {
                    sb.Append(key);
                    sb.Append("=");
                    sb.Append(value);
                    sb.Append("&");
                    continue;
                }
                tempKey = key.Substring(3);
                attrType = (string)Attributes[tempKey]["Type"];
                switch (attrType.ToLower())
                {
                    case "picklist":
                    case "status":
                        convertedValue = GetDropdownItemValue(tempKey, value);
                        break;
                    case "classification":
                        convertedValue = GetClassificationNodeValue(tempKey, value);
                        break;
                    case "lookup":
                        convertedValue = GetPrimaryKeyValue(tempKey, value);
                        break;
                    default:
                        convertedValue = value;
                        break;
                }
                sb.Append(key);
                sb.Append("=");
                sb.Append(convertedValue);
                sb.Append("&");
            }

            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        /// <summary>
        /// Get line item section auto id
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetLineItemSectionAutoID()
        {
            HttpResponseMessage response = HttpService.Instance.DoGet("DynamicPage/GetPage/" + entityName);
            JObject items = response.Content.ReadAsAsync<dynamic>().Result;
            var value = (from item in items["PageItems"].Children()
                         where string.Compare(item["PageItemType"].Value<string>(), "LineItem", true) == 0
                         select new { Label = item.Value<string>("Label"), AutoID = (string)item["Item"]["LineItemSectionAutoID"] });

            return value.ToDictionary(x => x.Label, x => x.AutoID);
        }

        private string entityName;
        private JObject metaData;
        private Dictionary<string, JToken> attributes;
        public JObject MetaData { get { return metaData; } }
        public Dictionary<string, JToken> Attributes
        {
            get
            {
                if (attributes == null)
                    attributes = metaData["Attributes"].ToDictionary(attr => attr.Value<string>("PhysicalName"), attr => attr);
                return attributes;
            }
        }
    }
}