using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using SQLWebProxySharpEntities.Types;
using System.Xml.Linq;

namespace SQLWebProxySharpEntities.Entities
{
	public class SQLWebProxyResultScalar : SQLWebProxyResult
	{
		public object ScalarValue { get; set; }

        public SQLWebProxyResultScalar()
		{
            ScalarValue = null;
		}

		public string ToXml()
		{
			XDocument doc = CreateBaseDocument("Scalar");

			XElement scalarElem = new XElement("Scalar");
			scalarElem.Add(TypeSerializer.Serialize(ScalarValue));
			doc.Root.Add(scalarElem);

			return doc.ToString();
		}

		public static SQLWebProxyResult FromXml(XDocument xml)
		{
            SQLWebProxyResultScalar result = new SQLWebProxyResultScalar();

			XElement nonqueryElem = xml.Root.Element("Scalar");
			result.ScalarValue = TypeSerializer.Deserialize(nonqueryElem.Elements().FirstOrDefault());

			return result;
		}

        public override string ToString()
        {
            return "Result Scalar: " + ScalarValue;
        }
	}
}
