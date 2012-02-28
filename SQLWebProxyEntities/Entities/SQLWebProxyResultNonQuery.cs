using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using SQLWebProxySharpEntities.Types;
using System.Xml.Linq;

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
			XDocument doc = CreateBaseDocument("NonQuery");

			XElement nonqueryElem = new XElement("NonQuery");
			nonqueryElem.Add(TypeSerializer.Serialize(Value));
			doc.Root.Add(nonqueryElem);

			return doc.ToString();
		}

		public static SQLWebProxyResult FromXml(XDocument xml)
		{
            SQLWebProxyResultNonQuery result = new SQLWebProxyResultNonQuery();

			XElement nonqueryElem = xml.Root.Element("NonQuery");
			result.Value = (int)TypeSerializer.Deserialize(nonqueryElem.Elements().FirstOrDefault());

			return result;
		}

        public override string ToString()
        {
            return "Result NonQuery: " + Value;
        }
	}
}
