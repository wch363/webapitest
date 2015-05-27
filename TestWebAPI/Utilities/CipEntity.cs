using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace TestWebAPI.Utilities
{
    public class CipEntity
    {
        private string logicalName;
        private JObject entityInfo;

        public CipEntity(string logicalName)
        {
            this.logicalName = logicalName;
            HttpService request = new HttpService();
            entityInfo = request.DoGet("Metadata/GetEntity/" + logicalName);
        }

        public Dictionary<string, string> GetAttributeTypes()
        {
            var types = (from token in entityInfo["Attributes"].Children()
                         select (new { Name = token.Value<string>("PhysicalName"), Type = token.Value<string>("Type") ?? "" }));
            Dictionary<string, string> attibuteTypes = types.ToDictionary(x => x.Name, x => x.Type);

            return attibuteTypes;
        }

        public string GetPickListItemValue(string attrPhysicalName, string itemName)
        {
            HttpService request = new HttpService();
            JObject piclist = request.DoGet("Metadata/GetDropdownlist/" + logicalName + "?attributeName=" + attrPhysicalName);
            string value = (from li in piclist["items"].Children()
                            where string.Compare(li["Text"].Value<string>(), itemName, 0) == 0
                            select li["Value"].Value<string>()).First<string>();
            return value;
        }

        public string GetClassificationNodeValue(string attrPhysicalName, string nodeName)
        {
            string value = string.Empty;
            HttpService request = new HttpService();
            JArray nodes = request.DoGet("Metadata/GetClassificationNodes/" + logicalName + "?attributeName=" + attrPhysicalName);
            foreach (JToken token in nodes.Children())
            {
                value = GetNodeValue(token, nodeName);
                if (!string.IsNullOrEmpty(value))
                    break;
            }

            return value;
        }

        private string GetNodeValue(JToken token, string nodeName)
        {
            if (token == null)
                return "";
            if (string.Compare(token["text"].Value<string>().Replace("\"",""), nodeName, 0) == 0)
                return token["id"].Value<string>();

            return GetNodeValue(token["items"].First, nodeName);
        }

        private void GetRequiredAttributes() { }

        private void GetChildEntities() { }
    }
}