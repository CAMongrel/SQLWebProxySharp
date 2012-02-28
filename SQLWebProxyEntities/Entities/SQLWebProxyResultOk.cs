using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace SQLWebProxySharpEntities.Entities
{
	public class SQLWebProxyResultOk : SQLWebProxyResult
	{
		public string ToXml()
		{
			XDocument doc = CreateBaseDocument("Ok");

			return doc.ToString();
		}

        public override string ToString()
        {
            return "Result OK";
        }
	}
}
