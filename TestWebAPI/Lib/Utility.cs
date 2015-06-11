using System;
using System.Linq;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace TestWebAPI.Lib
{
    public static class Utility
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

        public static string Join(this NameValueCollection collection, Func<string, string> selector, string separator)
        {
            return String.Join(separator, collection.Cast<string>().Select(e => selector(e)));
        }
    }
}