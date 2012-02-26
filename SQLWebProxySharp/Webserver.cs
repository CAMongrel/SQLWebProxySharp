using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Web;
using System.Collections.Specialized;

namespace SQLWebProxySharp
{
	public delegate void LogOutputDelegate(string logLine);
	public delegate string ReceiveRequestDelegate(HttpListenerRequest request);

	class Webserver
	{
		private HttpListener listener;
		private Thread listenThread;

		public event LogOutputDelegate OnLogOutput;
		public event ReceiveRequestDelegate OnReceiveRequest;

		public Webserver()
		{
			listener = new HttpListener();
		}

		public void AddPrefix(string prefix)
		{
			listener.Prefixes.Add(prefix);
		}

		public void Start()
		{
			if (listener.IsListening)
				listener.Stop();

			listener.Start();

			listenThread = new Thread(new ThreadStart(ListenThreadFunc));
			listenThread.Start();
		}

		public void Stop()
		{
			if (listener.IsListening)
			{
				listenThread.Abort();
				listenThread = null;

				listener.Stop();
			}
		}

		private void ListenThreadFunc()
		{
			try
			{
				AddLog("Started listening ...");

				while (true)
				{
					HttpListenerContext context = listener.GetContext();

					HttpListenerRequest request = context.Request;

					// Construct a response.
					string responseString = "<HTML><BODY>";
					responseString += request.RawUrl;
					responseString += "</BODY></HTML>";

					if (OnReceiveRequest != null)
						responseString = OnReceiveRequest(request);

					// Obtain a response object.
					HttpListenerResponse response = context.Response;
					byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
					// Get a response stream and write the response to it.
					response.ContentLength64 = buffer.Length;
					System.IO.Stream output = response.OutputStream;
					output.Write(buffer, 0, buffer.Length);
					// You must close the output stream.
					output.Close();
				}
			}
			catch (ThreadAbortException)
			{
				// 'Tis ok
				AddLog("Aborted listening ...");
			}

			AddLog("Finished listening ...");
		}

		private void AddLog(string line)
		{
			if (OnLogOutput != null)
				OnLogOutput(line);
		}
	}
}
