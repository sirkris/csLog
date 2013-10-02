using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace csLogTest
{
    class Help
    {
        public static void ShowError(string errorString)
        {
            Console.WriteLine("\r\n" + errorString);
            Standard();
        }

        public static void Standard()
        {
            Console.WriteLine("\r\ncsLog Test Utility");
            Console.WriteLine("Created by Kris Craig <kriscraig@php.net>");
            Console.WriteLine("------------------------------------------\r\n");
            Console.WriteLine("Usage:  csLogTest.exe [OPTIONS]\r\n");
            Console.WriteLine("Options:");
            Console.WriteLine("         --help");
            Console.WriteLine("             Display this message.");
            Console.WriteLine("         --basedir <basedir>");
            Console.WriteLine("             Where to look for csLog.dll.");
            Console.WriteLine("             Default:  " + Program.basedir);

            Environment.Exit(0);
        }
    }
}
