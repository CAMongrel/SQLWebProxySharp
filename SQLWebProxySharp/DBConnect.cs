using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace SQLWebProxySharp
{
	internal class DBConnect
	{
		static MySqlConnection conn;

		public static System.Data.ConnectionState State
		{
			get
			{
				if (conn == null)
					return System.Data.ConnectionState.Closed;

				return conn.State;
			}
		}

		static DBConnect()
		{
			conn = null;
		}

		internal static void Connect(string ConnectionString)
		{
			if (conn != null && conn.State != System.Data.ConnectionState.Closed)
				return;

			conn = new MySqlConnection(ConnectionString);
			// Block until done
			conn.Open();
		}

		internal static void Close()
		{
			if (State != System.Data.ConnectionState.Open)
				throw new InvalidOperationException("Not connected!");

			conn.Close();
		}

		internal static MySqlDataReader ExecuteReader(string query)
		{
			if (State != System.Data.ConnectionState.Open)
				throw new InvalidOperationException("Not connected!");

			MySqlCommand cmd = conn.CreateCommand();
			cmd.CommandText = query;
			return cmd.ExecuteReader();
		}

		internal static int ExecuteNonQuery(string query)
		{
			if (State != System.Data.ConnectionState.Open)
				throw new InvalidOperationException("Not connected!");

			MySqlCommand cmd = conn.CreateCommand();
			cmd.CommandText = query;
			return cmd.ExecuteNonQuery();
		}

		internal static object ExecuteScalar(string query)
		{
			if (State != System.Data.ConnectionState.Open)
				throw new InvalidOperationException("Not connected!");

			MySqlCommand cmd = conn.CreateCommand();
			cmd.CommandText = query;
			return cmd.ExecuteScalar();
		}

		public static string ConvertDateTimeToSQLDateTime(ref DateTime dt)
		{
			return dt.ToString("yyyy-MM-dd");
		}
	}
}
