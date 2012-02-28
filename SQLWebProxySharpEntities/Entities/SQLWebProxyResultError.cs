using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SQLWebProxySharpEntities.Entities
{
	public class SQLWebProxyResultError : SQLWebProxyResult
	{
		public string Error { get; set; }

		public SQLWebProxyResultError()
		{
			Error = "";
		}

		public string ToXml()
		{
			XDocument doc = CreateBaseDocument("Error");

			XElement error = new XElement("Error");
			error.Value = Error;
			doc.Root.Add(error);

			return doc.ToString();
		}

		public static SQLWebProxyResult FromXml(XDocument xml)
		{
			SQLWebProxyResultError result = new SQLWebProxyResultError();

            XElement errorNode = xml.Root.Element("Error");
            result.Error = errorNode.Value;

			return result;
		}

        public override string ToString()
        {
            return "Result Error: " + Error;
        }
	}
}
