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
using SQLWebProxySharp.Database;
using SQLWebProxySharpEntities.Entities;
using SQLWebProxySharpEntities.Types;
using System.IO;

namespace SQLWebProxySharp
{
	public class SQLWebProxy
	{
		private Webserver webserver;
        private IBackend backend;

		public event LogOutputDelegate OnLogOutput;

        private string serverAddress;
        private string userName;
        private string database;
        private string password;
        private int serverPort;

        private string ConnectionString
        {
            get
            {
                return string.Format("server={0};user={1};database={2};port={4};password={3};",
                    serverAddress, userName, database, password, serverPort.ToString());
            }
        }

        #region Constructor
        public SQLWebProxy(string SQLServerAddress, string SQLUserName, string SQLDatabase, string SQLUserPassword, int SQLPort,
            string listenServer = "localhost", int listenPort = 8080)
		{
            serverAddress = SQLServerAddress;
            userName = SQLUserName;
            database = SQLDatabase;
            password = SQLUserPassword;
            serverPort = SQLPort;

            //backend = new DBConnect();
            backend = new DummyConnect();

			webserver = new Webserver();
			webserver.OnLogOutput += (line) =>
				{
					if (OnLogOutput != null)
						OnLogOutput(line);
				};
			webserver.OnReceiveRequest += webserver_OnReceiveRequest;
			webserver.AddPrefix("http://" + listenServer + ":" + listenPort + "/");
		}
        #endregion

        #region Helper methods
        private string ConstructError(string errorMessage)
		{
            SQLWebProxyResultError error = new SQLWebProxyResultError();
            error.Error = errorMessage;
            return error.ToXml();
		}

		private string ConstructOk()
		{
            SQLWebProxyResultOk ok = new SQLWebProxyResultOk();
            return ok.ToXml();
		}

		private string ConstructReader(object[][] rows)
		{
            SQLWebProxyResultReader reader = new SQLWebProxyResultReader();
            reader.Rows = rows;
            return reader.ToXml();
		}

		private string ConstructScalar(object obj)
		{
            SQLWebProxyResultScalar scalar = new SQLWebProxyResultScalar();
            scalar.ScalarValue = obj;
            return scalar.ToXml();
		}

		private string ConstructNonQuery(int result)
		{
            SQLWebProxyResultNonQuery nonquery = new SQLWebProxyResultNonQuery();
            nonquery.Value = result;
            return nonquery.ToXml();
		}
        #endregion

        #region Handle request
        private string webserver_OnReceiveRequest(System.Net.HttpListenerRequest request)
		{
			Uri uri = request.Url;
			string mode = uri.Segments[1];
            
            string query = "";
            using (StreamReader reader = new StreamReader(request.InputStream))
            {
                query = reader.ReadLine();
            }

            //if (OnLogOutput != null)
            //    OnLogOutput(string.Format("Received request for mode '{0}' with query '{1}'", mode, query));

			switch (mode)
			{
				case null:
					return ConstructError("NO_MODE");

				case "reader":
					{
                        if (backend.State != ConnectionState.Open)
							return ConstructError("NOT_CONNECTED");

						try
						{
                            object[][] rows = backend.ExecuteReader(query);
                            return ConstructReader(rows);
						}
						catch (Exception ex)
						{
							return ConstructError("EXCEPTION: " + ex.ToString());
						}
					}

				case "scalar":
					{
                        if (backend.State != ConnectionState.Open)
							return ConstructError("NOT_CONNECTED");

						try
						{
                            object result = backend.ExecuteScalar(query);
							return ConstructScalar(result);
						}
						catch (Exception ex)
						{
							return ConstructError("EXCEPTION: " + ex.ToString());
						}
					}

				case "nonquery":
					{
                        if (backend.State != ConnectionState.Open)
							return ConstructError("NOT_CONNECTED");

						try
						{
                            int result = backend.ExecuteNonQuery(query);
							return ConstructNonQuery(result);
						}
						catch (Exception ex)
						{
							return ConstructError("EXCEPTION: " + ex.ToString());
						}
					}
			}

			// Empty response
			return ConstructError("DONT_KNOW_HOW_TO_HANDLE_REQUEST");
		}
        #endregion

        #region Start/Stop
        public void Start()
		{
			webserver.Start();

            backend.Connect(ConnectionString);

            if (OnLogOutput != null)
                OnLogOutput("Connected to Backend");
		}

		public void Stop()
		{
            backend.Close();

            if (OnLogOutput != null)
                OnLogOutput("Backend closed");

			webserver.Stop();
        }
        #endregion
    }
}