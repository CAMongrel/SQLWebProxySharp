using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLWebProxySharpEntities.Types;

namespace SQLWebProxySharp.Database
{
    class DummyConnect : IBackend
    {
        public ConnectionState State
        {
            get { return ConnectionState.Open; }
        }

        public void Connect(string ConnectionString)
        {
            //
        }

        public void Close()
        {
            //
        }

        public object[][] ExecuteReader(string query)
        {
            List<object[]> result = new List<object[]>();

            object[] row = new object[] { 4711, "Test123", query, "Super", 0.01 };

            result.Add(row);

            return result.ToArray();
        }

        public int ExecuteNonQuery(string query)
        {
            return query.Length;
        }

        public object ExecuteScalar(string query)
        {
            return "Hello World: " + query.Length;
        }
    }
}
