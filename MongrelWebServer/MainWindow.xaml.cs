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

namespace MongrelWebServer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private SQLWebProxy server;

		public MainWindow()
		{
			InitializeComponent();

			this.Closing += (s, e) =>
				{
					server.Stop();
				};

			server = new SQLWebProxy();
			server.OnLogOutput += (line) =>
				{
					Dispatcher.Invoke(new Action(() =>
					{
						textBox1.Text += line + "\r\n";
					}));
				};
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			server.Start();
		}

		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			server.Stop();
		}
	}
}
