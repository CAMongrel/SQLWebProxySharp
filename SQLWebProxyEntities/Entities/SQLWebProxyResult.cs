using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SQLWebProxySharpEntities.Entities
{
	public abstract class SQLWebProxyResult
	{
        public string Type { get; private set; }

		protected XmlDocument CreateBaseDocument(string setType)
		{
			XmlDocument doc = new XmlDocument();
			doc.AppendChild(doc.CreateElement("SQLWebProxyResult"));
			doc.DocumentElement.SetAttribute("Type", setType);

            Type = setType;

			return doc;
		}

		public static SQLWebProxyResult FromXml(string xml)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(xml);

			if (doc.DocumentElement.Name != "SQLWebProxyResult")
				throw new Exception("Invalid XML");

			string type = doc.DocumentElement.GetAttribute("Type");

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
