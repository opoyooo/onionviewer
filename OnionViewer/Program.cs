using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace OnionViewer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Thread.Sleep(300000);
            Application.Run(new FormViewer());
        }
        static void Application_ThreadException(object sender, ThreadExceptionEventArgs e) {

            // Log the exception, display it, etc

            FileStream fs = File.Open("ThreadException.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(e.Exception.Message);
            sw.Flush();
            sw.Close();
            sw.Dispose();
            fs.Close();
            fs.Dispose();

        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {

            // Log the exception, display it, etc

            FileStream fs = File.Open("UnhandledException.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(e.ExceptionObject.ToString());
            sw.Flush();
            sw.Close();
            sw.Dispose();
            fs.Close();
            fs.Dispose();
        }
    }
}