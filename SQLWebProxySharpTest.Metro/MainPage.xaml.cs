using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SQLWebProxySharpClient;
using SQLWebProxySharpEntities.Entities;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace SQLWebProxySharpTest.Metro
{
    partial class MainPage
    {
        SQLWebProxyClient client;

        public MainPage()
        {
            InitializeComponent();

            client = new SQLWebProxyClient();
            client.RemoteAddress = "localhost";
            client.RemotePort = 8080;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SQLWebProxyResult query = client.ExecuteNonQuery("SELECT * FROM bla;");
            lbTest.Text += query.ToString() + "\r\n";

            query = client.ExecuteScalar("SELECT 1;");
            lbTest.Text += query.ToString() + "\r\n";

            query = client.ExecuteReader("SELECT 0;");
            lbTest.Text += query.ToString() + "\r\n";
        }
    }
}
