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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SQLWebProxySharpClient;
using SQLWebProxySharp;
using System.Xml;
using SQLWebProxySharpEntities.Entities;
using System.Threading;

namespace SQLWebProxySharpTest
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// 
	/// This test application requires a working MySQL server. The correct
	/// settings should be entered into the .config file
	/// </summary>
	public partial class MainWindow : Window
	{
		private SQLWebProxyClient client;
		private SQLWebProxy server;

		public MainWindow()
		{
			InitializeComponent();

            TestSerialize();

			this.Closing += (s, e) =>
				{
					server.Stop();
				};

			// NOTE: The constructor of SQLWebProxy uses default settings. You might want to
			// override these and should edit the .config file to provide valid settings here
            server = new SQLWebProxy(Properties.Settings.Default.ServerAddress, Properties.Settings.Default.ServerUsername,
                Properties.Settings.Default.ServerPassword, Properties.Settings.Default.ServerDatabase, Properties.Settings.Default.ServerPort);
			server.OnLogOutput += (line) =>
				{
                    Dispatcher.Invoke(new Action(() =>
                        {
                            textBox1.Text += line + "\r\n";
                        }));
				};

            Properties.Settings.Default.RemoteAddress = "localhost";
            Properties.Settings.Default.RemotePort = 8080;

			// NOTE: You should edit the .config file to provide valid settings here
			client = new SQLWebProxyClient();
			client.RemoteAddress = Properties.Settings.Default.RemoteAddress;
			client.RemotePort = Properties.Settings.Default.RemotePort;
		}

        private static void TestSerialize()
        {
            SQLWebProxyResultOk ok = new SQLWebProxyResultOk();
            string xml = ok.ToXml();

            SQLWebProxyResultError error = new SQLWebProxyResultError();
            error.Error = "Alles doof";
            xml = error.ToXml();
            error.Error = "Nu is weg";
            error = SQLWebProxyResultError.FromXml(xml) as SQLWebProxyResultError;

            SQLWebProxyResultNonQuery nonquery = new SQLWebProxyResultNonQuery();
            nonquery.Value = 42;
            xml = nonquery.ToXml();
            nonquery.Value = 0;
            nonquery = SQLWebProxyResultNonQuery.FromXml(xml) as SQLWebProxyResultNonQuery;

            SQLWebProxyResultScalar scalar = new SQLWebProxyResultScalar();
            scalar.ScalarValue = "Hallo Welt!";
            xml = scalar.ToXml();
            scalar.ScalarValue = null;
            scalar = SQLWebProxyResultScalar.FromXml(xml) as SQLWebProxyResultScalar;

            scalar = new SQLWebProxyResultScalar();
            scalar.ScalarValue = (short)42;
            xml = scalar.ToXml();
            scalar.ScalarValue = null;
            scalar = SQLWebProxyResultScalar.FromXml(xml) as SQLWebProxyResultScalar;

            scalar = new SQLWebProxyResultScalar();
            scalar.ScalarValue = 47.11;
            xml = scalar.ToXml();
            scalar.ScalarValue = null;
            scalar = SQLWebProxyResultScalar.FromXml(xml) as SQLWebProxyResultScalar;

            SQLWebProxyResultReader reader = new SQLWebProxyResultReader();
            List<object[]> rowList = new List<object[]>();
            rowList.Add(new object[] { 4711, "Test123", "Super", 0.01 });
			rowList.Add(new object[] { 4711, 999, null, "Willi" });
            reader.Rows = rowList.ToArray();
            xml = reader.ToXml();
            reader.Rows = null;
            reader = SQLWebProxyResultReader.FromXml(xml) as SQLWebProxyResultReader;
        }

		private void Button_Start_Click(object sender, RoutedEventArgs e)
		{
			server.Start();
		}

		private void Button_Stop_Click(object sender, RoutedEventArgs e)
		{
			server.Stop();
		}

		private void Button_Reader_Click(object sender, RoutedEventArgs e)
		{
            SQLWebProxyResult result = client.ExecuteReader(queryBox.Text);
            textBox1.Text += "Received: " + result + "\r\n";
        }

		private void Button_Scalar_Click(object sender, RoutedEventArgs e)
		{
            SQLWebProxyResult result = client.ExecuteScalar(queryBox.Text);
            textBox1.Text += "Received: " + result + "\r\n";
        }

		private void Button_NonQuery_Click(object sender, RoutedEventArgs e)
		{
            SQLWebProxyResult result = client.ExecuteNonQuery(queryBox.Text);
            textBox1.Text += "Received: " + result + "\r\n";
		}
	}
}
