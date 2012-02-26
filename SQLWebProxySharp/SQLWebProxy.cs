// #region License
// Microsoft Public License (Ms-PL)
// MonoGame - Copyright © 2012 Henning Thöle
//
// All rights reserved.
//
// This license governs use of the accompanying software. If you use the software, you accept this license. If you do not
// accept the license, do not use the software.
//
// 1. Definitions
// The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under
// U.S. copyright law.
//
// A "contribution" is the original software, or any additions or changes to the software.
// A "contributor" is any person that distributes its contribution under this license.
// "Licensed patents" are a contributor's patent claims that read directly on its contribution.
//
// 2. Grant of Rights
// (A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3,
// each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
// (B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3,
// each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.
//
// 3. Conditions and Limitations
// (A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
// (B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software,
// your patent license from such contributor to the software ends automatically.
// (C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution
// notices that are present in the software.
// (D) If you distribute any portion of the software in source code form, you may do so only under this license by including
// a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object
// code form, you may only do so under a license that complies with this license.
// (E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees
// or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent
// permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular
// purpose and non-infringement.
// #endregion License
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Web;
using MySql.Data.MySqlClient;

namespace SQLWebProxySharp
{
	public class SQLWebProxy
	{
		private Webserver webserver;

		public event LogOutputDelegate OnLogOutput;

		public SQLWebProxy(string listenServer = "localhost", int listenPort = 8080)
		{
			webserver = new Webserver();
			webserver.OnLogOutput += (line) =>
				{
					if (OnLogOutput != null)
						OnLogOutput(line);
				};
			webserver.OnReceiveRequest += webserver_OnReceiveRequest;
			webserver.AddPrefix("http://" + listenServer + ":" + listenPort + "/");
		}

		private string ConstructError(string errorMessage)
		{
			return "<SQLWebProxyResult><Error>" + errorMessage + "</Error></SQLWebProxyResult>";
		}

		private string ConstructOk()
		{
			return "<SQLWebProxyResult><Ok /></SQLWebProxyResult>";
		}

		private string ConstructReader(object[][] rows)
		{
			int rowCount = rows.Length;
			int fieldCount = 0;
			if (rows.Length > 0)
				fieldCount = rows[0].Length;

			string result = "<SQLWebProxyResult>";
			result += "<Reader RowCount=\"" + rowCount + "\" FieldCount=\"" + fieldCount + "\">";
			for (int i = 0; i < rows.Length; i++)
			{
				result += "<Row>";
				for (int j = 0; j < rows[i].Length; j++)
				{
					object obj = rows[i][j];
					string typeName = "null";
					if (obj != null)
						typeName = obj.GetType().Name;

					result += "<Column Type=\"" + typeName + "\">" + obj + "</Column>";
				}
				result += "</Row>";
			}
			result += "</Reader>";
			result += "<SQLWebProxyResult>";

			return result;
		}

		private string ConstructScalar(object obj)
		{
			string typeName = "null";
			if (obj != null)
				typeName = obj.GetType().Name;

			return "<SQLWebProxyResult><Scalar Type=\"" + typeName + "\">" + obj + "</Scalar></SQLWebProxyResult>";
		}

		private string ConstructNonQuery(int result)
		{
			return "<SQLWebProxyResult><NonQuery>" + result.ToString() + "</NonQuery></SQLWebProxyResult>";
		}

		private string webserver_OnReceiveRequest(System.Net.HttpListenerRequest request)
		{
			Uri uri = request.Url;
			NameValueCollection parameters = HttpUtility.ParseQueryString(uri.Query);
			string mode = parameters["mode"];

			switch (mode)
			{
				case null:
					return ConstructError("NO_MODE");

				case "open":
					{
						if (DBConnect.State != System.Data.ConnectionState.Closed)
							return ConstructError("ALREADY_CONNECTED");

						string connString = parameters["connString"];
						try
						{
							DBConnect.Connect(connString);
						}
						catch (Exception ex)
						{
							return ConstructError("EXCEPTION: " + ex.ToString());
						}

						if (DBConnect.State == System.Data.ConnectionState.Open)
							return ConstructOk();

						return ConstructError("CONNECT_FAILED");
					}

				case "close":
					{
						if (DBConnect.State != System.Data.ConnectionState.Open)
							return ConstructError("NOT_CONNECTED");

						try
						{
							DBConnect.Close();
						}
						catch (Exception ex)
						{
							return ConstructError("EXCEPTION: " + ex.ToString());
						}

						if (DBConnect.State == System.Data.ConnectionState.Closed)
							return ConstructOk();

						return ConstructError("DISCONNECT_FAILED");
					}

				case "reader":
					{
						if (DBConnect.State != System.Data.ConnectionState.Open)
							return ConstructError("NOT_CONNECTED");

						string query = parameters["query"];

						try
						{
							MySqlDataReader reader = DBConnect.ExecuteReader(query);

							List<object[]> resultArray = new List<object[]>();
							while (reader.Read())
							{
								object[] row = new object[reader.VisibleFieldCount];
								for (int i = 0; i < reader.VisibleFieldCount; i++)
									row[i] = reader.GetValue(i);
								resultArray.Add(row);
							}

							return ConstructReader(resultArray.ToArray());
						}
						catch (Exception ex)
						{
							return ConstructError("EXCEPTION: " + ex.ToString());
						}
					}
					break;

				case "scalar":
					{
						if (DBConnect.State != System.Data.ConnectionState.Open)
							return ConstructError("NOT_CONNECTED");

						string query = parameters["query"];

						try
						{
							object result = DBConnect.ExecuteScalar(query);
							return ConstructScalar(result);
						}
						catch (Exception ex)
						{
							return ConstructError("EXCEPTION: " + ex.ToString());
						}
					}

				case "nonquery":
					{
						if (DBConnect.State != System.Data.ConnectionState.Open)
							return ConstructError("NOT_CONNECTED");

						string query = parameters["query"];

						try
						{
							int result = DBConnect.ExecuteNonQuery(query);
							return ConstructNonQuery(result);
						}
						catch (Exception ex)
						{
							return ConstructError("EXCEPTION: " + ex.ToString());
						}
					}
			}

			// Empty response
			string responseString = "<SQLWebProxyResult></SQLWebProxyResult>";
			return responseString;
		}

		public void Start()
		{
			webserver.Start();
		}

		public void Stop()
		{
			webserver.Stop();
		}
	}
}