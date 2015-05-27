using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TestWebAPI.Utilities
{
    public static class Util
    {
        public static string Base64Encode(string toEncode)
        {
            byte[] toEncodeAsBytes = System.Text.UTF8Encoding.ASCII.GetBytes(toEncode);
            return System.Convert.ToBase64String(toEncodeAsBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static JObject ToJObject(this object obj)
        {
            string jsonString=JsonConvert.SerializeObject(obj, Formatting.None);
            return JObject.Parse(jsonString);
        }

        public static string ConvertInputFileds(Dictionary<string, string> inputFields, string entityLogicalName)
        {
            //string tempValue;
            //Dictionary<string, string> convertedInputFields = new Dictionary<string, string>();
            //CipEntity entity = new CipEntity(entityLogicalName);
            //Dictionary<string, string> attrTypes = entity.GetAttributeTypes();

            //foreach (KeyValuePair<string, string> entry in inputFields)
            //{
            //    string attrType = attrTypes[entry.Key];
            //    switch (attrType)
            //    {
            //        case "Picklist":
            //        case "Status":
            //            if (!string.IsNullOrEmpty(entry.Value))
            //            {
            //                JObject piclist=entity.GetPickListContent(entry.Key);
            //                tempValue = (from li in piclist["items"].Children()
            //                             where string.Equals(li["Text"].Value<string>(), entry.Value, StringComparison.OrdinalIgnoreCase)
            //                             select li["Value"].Value<string>()).First<string>();
            //                convertedInputFields.Add(entry.Key, tempValue);
            //            }
            //            break;
            //        case "Classification":
            //            if (!string.IsNullOrEmpty(entry.Value))
            //            {
            //                JObject piclist = entity.GetClassificationNodes(entry.Key);
            //                tempValue = (from node in piclist["items"].Children()
            //                             where string.Equals(node.NodeCode + " - " + node.NodeName, entry.Value, StringComparison.OrdinalIgnoreCase)
            //                             select node.NodeID.ToString()).First<string>();
            //                convertedInputFields.Add(entry.Key, tempValue);
            //            }
            //            break;
            //        default:
            //            convertedInputFields.Add(entry.Key, entry.Value);
            //            break;
            //    }
            //}


            //string json = JsonConvert.SerializeObject(convertedInputFields, Formatting.Indented);

            //return json;
            return null;
        }
    }
}