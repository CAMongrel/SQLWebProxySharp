using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLWebProxySharp;
using System.Threading;

namespace SQLWebProxySharpCommandLineServer
{
    class Program
    {
        private static bool isRunning;
        private static SQLWebProxy server;

        static void Main(string[] args)
        {
            Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);

            // NOTE: The constructor of SQLWebProxy uses default settings. You might want to
            // override these and should edit the .config file to provide valid settings here
            server = new SQLWebProxy(Properties.Settings.Default.SQLServerAddress, Properties.Settings.Default.SQLUserName,
                Properties.Settings.Default.SQLPassword, Properties.Settings.Default.SQLDatabase, Properties.Settings.Default.SQLServerPort,
                Properties.Settings.Default.ListenAddress, Properties.Settings.Default.ListenPort);
            server.OnLogOutput += (line) =>
                {
                    Console.WriteLine(line);
                };

            server.Start();

            Console.WriteLine("Server has been started ...");

            isRunning = true;
            while (isRunning)
            {
                Thread.Sleep(0);
            }

            Console.WriteLine("Server has been shut down");
        }

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            server.Stop();

            isRunning = false;
        }
    }
}
