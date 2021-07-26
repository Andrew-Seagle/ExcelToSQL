using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelToSQL
{
    public static class Formatter
    {
        public static string JsonPrettyAppend(string oldJson, string toAppend)
        {
            var textString = new StringBuilder(oldJson);

            toAppend = JProperty.Parse(toAppend).ToString();

            textString.Insert(textString.Length - 2, $", {toAppend}");

            return JToken.Parse(textString.ToString()).ToString();
        }
    }
}
