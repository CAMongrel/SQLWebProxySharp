using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SQLWebProxySharpEntities.Entities
{
	public abstract class SQLWebProxyResult
	{
        public string Type { get; private set; }

		protected XDocument CreateBaseDocument(string setType)
		{
			XDocument doc = new XDocument(
				new XElement("SQLWebProxyResult",
					new XAttribute("Type", setType)));

            Type = setType;

			return doc;
		}

		public static SQLWebProxyResult FromXml(string xml)
		{
			XDocument doc = XDocument.Parse(xml);

			if (doc.Root.Name != "SQLWebProxyResult")
				throw new Exception("Invalid XML");

			string type = doc.Root.Attribute("Type").Value;

			SQLWebProxyResult result = null;

			switch (type)
			{
                default:
                    result = new SQLWebProxyResultError() { Error = "SERVER_ERROR" };
                    break;
				case "Ok":
                    result = new SQLWebProxyResultOk();
                    break;
				case "Error":
					result = SQLWebProxyResultError.FromXml(doc);
                    break;
				case "NonQuery":
                    result = SQLWebProxyResultNonQuery.FromXml(doc);
                    break;
				case "Scalar":
                    result = SQLWebProxyResultScalar.FromXml(doc);
                    break;
				case "Reader":
                    result = SQLWebProxyResultReader.FromXml(doc);
                    break;
			}

            result.Type = type;

			return result;
		}
	}
}
