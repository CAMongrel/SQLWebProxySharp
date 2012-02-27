using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using SQLWebProxySharpEntities.Types;

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
            XmlDocument doc = CreateBaseDocument("Scalar");

            XmlElement scalarElem = doc.CreateElement("Scalar");
            scalarElem.InnerXml = TypeSerializer.Serialize(ScalarValue);
            doc.DocumentElement.AppendChild(scalarElem);

			return doc.OuterXml;
		}

		public static SQLWebProxyResult FromXml(XmlDocument xml)
		{
            SQLWebProxyResultScalar result = new SQLWebProxyResultScalar();

            XmlNode scalarElem = xml.DocumentElement.SelectSingleNode("Scalar");
            result.ScalarValue = TypeSerializer.Deserialize(scalarElem.FirstChild);

			return result;
		}

        public override string ToString()
        {
            return "Result Scalar: " + ScalarValue;
        }
	}
}
