using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using SQLWebProxySharpEntities.Types;

namespace SQLWebProxySharpEntities.Entities
{
	public class SQLWebProxyResultReader : SQLWebProxyResult
	{
		public object[][] Rows { get; set; }

        public SQLWebProxyResultReader()
		{
            Rows = null;
		}

		public string ToXml()
		{
            XmlDocument doc = CreateBaseDocument("Reader");

            XmlElement rowsElem = doc.CreateElement("Rows");

            int rowCount = 0;
            int fieldCount = 0;

            if (Rows != null)
            {
                rowCount = Rows.Length;

                if (rowCount > 0)
                    fieldCount = Rows[0].Length;
            }

            rowsElem.SetAttribute("RowCount", rowCount.ToString());
            rowsElem.SetAttribute("FieldCount", fieldCount.ToString());

            doc.DocumentElement.AppendChild(rowsElem);

            for (int i = 0; i < rowCount; i++)
            {
                XmlElement rowElem = doc.CreateElement("Row");
                rowsElem.AppendChild(rowElem);

                for (int j = 0; j < fieldCount; j++)
                {
                    XmlElement colElem = doc.CreateElement("Column");
                    rowElem.AppendChild(colElem);

                    colElem.InnerXml = TypeSerializer.Serialize(Rows[i][j]);
                }
            }

			return doc.OuterXml;
		}

		public static SQLWebProxyResult FromXml(XmlDocument xml)
		{
            SQLWebProxyResultReader result = new SQLWebProxyResultReader();

            XmlNode rowsElem = xml.DocumentElement.SelectSingleNode("Rows");

            int rowCount = int.Parse(rowsElem.Attributes["RowCount"].InnerText);
            int fieldCount = int.Parse(rowsElem.Attributes["FieldCount"].InnerText);

            result.Rows = new object[rowCount][];

            for (int i = 0; i < rowCount; i++)
            {
                result.Rows[i] = new object[fieldCount];

                XmlNode rowNode = rowsElem.ChildNodes[i];
                for (int j = 0; j < fieldCount; j++)
                {
                    XmlNode colNode = rowNode.ChildNodes[j];
                    result.Rows[i][j] = TypeSerializer.Deserialize(colNode.FirstChild);
                }
            }

			return result;
		}

        public override string ToString()
        {
            return "Result Reader: " + Rows.Length;
        }
	}
}
