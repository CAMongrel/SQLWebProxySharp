using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLWebProxySharpEntities.Types;
using MySql.Data.MySqlClient;

namespace SQLWebProxySharp.Database
{
    internal interface IBackend
    {
        ConnectionState State { get; }

        void Connect(string ConnectionString);
        void Close();
        object[][] ExecuteReader(string query);
		int ExecuteNonQuery(string query);
        object ExecuteScalar(string query);
    }
}
