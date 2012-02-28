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
using System.Net;
using System.IO;
using SQLWebProxySharpEntities.Entities;
using System.Threading;

namespace SQLWebProxySharpClient
{
	/// <summary>
	/// Stateless client class
	/// </summary>
	public class SQLWebProxyClient
	{
		class AsyncHelper
		{
			public HttpWebRequest Request;
			public string Query;
			public string Result;
		}

		public string RemoteAddress { get; set; }
		public int RemotePort { get; set; }

		private string RemoteServer
		{
			get
			{
				return "http://" + RemoteAddress + ":" + RemotePort + "/";
			}
		}

		public SQLWebProxyClient()
		{
			RemoteAddress = null;
			RemotePort = -1;
		}

		private void CheckSettings()
		{
			if (string.IsNullOrWhiteSpace(RemoteAddress))
				throw new Exception("RemoteAddress not set (must be a valid server address, e.g. localhost or 192.168.1.1)");
			if (RemotePort == -1 || RemotePort == 0)
				throw new Exception("RemotePort not set (must be a valid server port, e.g. 8080)");
		}

		#region Asynchronous Request forced to be synchronous
		// TODO: Use async/await
		// NOTE: Keep it simple for now ... needs improvement
		private ManualResetEvent allDone = new ManualResetEvent(false);
		private string GetResponse(string uri, string query)
		{
            allDone.Reset();

            HttpWebRequest request = HttpWebRequest.Create(uri) as HttpWebRequest;
            request.Method = "POST";

			AsyncHelper asyncHlp = new AsyncHelper() { Query = query, Request = request };
			request.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), asyncHlp);

			allDone.WaitOne();

			return asyncHlp.Result;
		}

		private void GetRequestStreamCallback(IAsyncResult asynchronousResult)
		{
			AsyncHelper state = (AsyncHelper)asynchronousResult.AsyncState;

			HttpWebRequest request = state.Request;
			string query = state.Query;

			using (StreamWriter writer = new StreamWriter(request.EndGetRequestStream(asynchronousResult)))
			{
				writer.WriteLine(query);
			}

			request.BeginGetResponse(new AsyncCallback(GetResponseCallback), state);
		}

		private void GetResponseCallback(IAsyncResult asynchronousResult)
		{
			AsyncHelper state = (AsyncHelper)asynchronousResult.AsyncState;

			HttpWebRequest request = state.Request;

			HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);

			using (StreamReader reader = new StreamReader(response.GetResponseStream()))
			{
				state.Result = reader.ReadToEnd();
			}

			allDone.Set();
		}
		#endregion

		public SQLWebProxyResult ExecuteReader(string query)
		{
			CheckSettings();

			string response = GetResponse(RemoteServer + "reader", query);
            return SQLWebProxyResult.FromXml(response);
		}

        public SQLWebProxyResult ExecuteScalar(string query)
		{
			CheckSettings();

            string response = GetResponse(RemoteServer + "scalar", query);
            return SQLWebProxyResult.FromXml(response);
        }

        public SQLWebProxyResult ExecuteNonQuery(string query)
		{
			CheckSettings();

            string response = GetResponse(RemoteServer + "nonquery", query);
            return SQLWebProxyResult.FromXml(response);
        }
	}
}
