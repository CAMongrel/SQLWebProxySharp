using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using SQLWebProxySharpEntities.Types;
using System.Xml.Linq;

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
			XDocument doc = CreateBaseDocument("Reader");

			int rowCount = 0;
			int fieldCount = 0;

			if (Rows != null)
			{
				rowCount = Rows.Length;

				if (rowCount > 0)
					fieldCount = Rows[0].Length;
			}

			XElement rowsElem = new XElement("Rows",
				new XAttribute("RowCount", rowCount),
				new XAttribute("FieldCount", fieldCount));
			doc.Root.Add(rowsElem);

            for (int i = 0; i < rowCount; i++)
            {
				XElement rowElem = new XElement("Row");

                rowsElem.Add(rowElem);

                for (int j = 0; j < fieldCount; j++)
                {
                    XElement colElem = new XElement("Column");
					rowElem.Add(colElem);

                    colElem.Add(TypeSerializer.Serialize(Rows[i][j]));
                }
            }

			return doc.ToString();
		}

		public static SQLWebProxyResult FromXml(XDocument xml)
		{
            SQLWebProxyResultReader result = new SQLWebProxyResultReader();

			XElement rowsElem = xml.Root.Element("Rows");

			int rowCount = int.Parse(rowsElem.Attribute("RowCount").Value);
            int fieldCount = int.Parse(rowsElem.Attribute("FieldCount").Value);

            result.Rows = new object[rowCount][];

			int cnt = 0;
			foreach (XElement rowNode in rowsElem.Elements())
			{
                result.Rows[cnt] = new object[fieldCount];

				int cnt2 = 0;
				foreach (XElement colNode in rowNode.Elements())
				{
					result.Rows[cnt][cnt2++] = TypeSerializer.Deserialize(colNode.Element("Value"));
				}

				cnt++;
            }

			return result;
		}

        public override string ToString()
        {
            return "Result Reader: " + Rows.Length;
        }
	}
}
