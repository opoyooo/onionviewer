using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace CAOVi {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Writing registry ...");
            RegistryKey rk = Registry.LocalMachine;
            RegistryKey rkStartup = rk.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            try {
                rkStartup.SetValue("OnionViewer", "\"" + args[0] + "\"");
                Console.WriteLine("Write completed!");
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
            rkStartup.Close();
            rkStartup.Dispose();
        }
    }
}