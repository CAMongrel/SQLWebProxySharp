using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using SQLWebProxySharpEntities.Types;

namespace SQLWebProxySharpEntities.Entities
{
	public class SQLWebProxyResultNonQuery : SQLWebProxyResult
	{
		public int Value { get; set; }

        public SQLWebProxyResultNonQuery()
		{
			Value = 0;
		}

		public string ToXml()
		{
			XmlDocument doc = CreateBaseDocument("NonQuery");

			XmlElement nonqueryElem = doc.CreateElement("NonQuery");
            nonqueryElem.InnerXml = TypeSerializer.Serialize(Value);
			doc.DocumentElement.AppendChild(nonqueryElem);

			return doc.OuterXml;
		}

		public static SQLWebProxyResult FromXml(XmlDocument xml)
		{
            SQLWebProxyResultNonQuery result = new SQLWebProxyResultNonQuery();

            XmlNode nonqueryElem = xml.DocumentElement.SelectSingleNode("NonQuery");
            result.Value = (int)TypeSerializer.Deserialize(nonqueryElem.FirstChild);

			return result;
		}

        public override string ToString()
        {
            return "Result NonQuery: " + Value;
        }
	}
}
