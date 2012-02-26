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

			this.Closing += (s, e) =>
				{
					server.Stop();
				};

			// NOTE: The constructor of SQLWebProxy uses default settings. You might want to
			// override these and should edit the .config file to provide valid settings here
			server = new SQLWebProxy();
			server.OnLogOutput += (line) =>
				{
					Dispatcher.Invoke(new Action(() =>
						{
							textBox1.Text += line + "\r\n";
						}));
				};

			// NOTE: You should edit the .config file to provide valid settings here
			client = new SQLWebProxyClient();
			client.RemoteAddress = Properties.Settings.Default.RemoteAddress;
			client.RemotePort = Properties.Settings.Default.RemotePort;
		}

		private void Button_Start_Click(object sender, RoutedEventArgs e)
		{
			server.Start();
		}

		private void Button_Stop_Click(object sender, RoutedEventArgs e)
		{
			server.Stop();
		}

		private void Button_Open_Click(object sender, RoutedEventArgs e)
		{
			// NOTE: You should edit the .config file to provide valid settings here
			client.Open(Properties.Settings.Default.ServerAddress, Properties.Settings.Default.ServerUsername,
				Properties.Settings.Default.ServerPassword, Properties.Settings.Default.ServerDatabase, Properties.Settings.Default.ServerPort);
		}

		private void Button_Close_Click(object sender, RoutedEventArgs e)
		{
			client.Close();
		}

		private void Button_Reader_Click(object sender, RoutedEventArgs e)
		{
			client.ExecuteReader(queryBox.Text);
		}

		private void Button_Scalar_Click(object sender, RoutedEventArgs e)
		{
			client.ExecuteScalar(queryBox.Text);
		}

		private void Button_NonQuery_Click(object sender, RoutedEventArgs e)
		{
			client.ExecuteNonQuery(queryBox.Text);
		}
	}
}
