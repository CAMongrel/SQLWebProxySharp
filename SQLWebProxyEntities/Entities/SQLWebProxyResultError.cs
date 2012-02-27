using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

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
			XmlDocument doc = CreateBaseDocument("Error");

			XmlElement errorElem = doc.CreateElement("Error");
			errorElem.InnerText = Error;
			doc.DocumentElement.AppendChild(errorElem);

			return doc.OuterXml;
		}

		public static SQLWebProxyResult FromXml(XmlDocument xml)
		{
			SQLWebProxyResultError result = new SQLWebProxyResultError();

            XmlNode errorNode = xml.DocumentElement.SelectSingleNode("Error");
            result.Error = errorNode.InnerText;

			return result;
		}

        public override string ToString()
        {
            return "Result Error: " + Error;
        }
	}
}
