using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace SQLWebProxySharpEntities.Entities
{
	public class SQLWebProxyResultOk : SQLWebProxyResult
	{
		public string ToXml()
		{
			XmlDocument doc = CreateBaseDocument("Ok");

			return doc.OuterXml;
		}

        public override string ToString()
        {
            return "Result OK";
        }
	}
}
