using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace csLogTest
{
    class Program
    {
        public static string basedir = Environment.CurrentDirectory + @"\..\";

        protected static Assembly csLog;
        protected static Type csLogType;
        protected static object csLogInstance;
        
        static void Main(string[] args)
        {
            parseArgs(args);
            runTests();
        }

        static void parseArgs(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                if (arg.IndexOf("--") == 0)
                {
                    switch (arg.Substring(2).ToLower())
                    {
                        default:
                            Help.ShowError("Unrecognized switch '" + arg + "'!");
                            break;
                        case "help":
                            Help.Standard();
                            break;
                        case "basedir":
                            if (i >= args.Length - 1)
                            {
                                Help.ShowError("Syntax error:  Argument required for '" + arg + "'!");
                            }
                            else
                            {
                                i++;
                                basedir = args[i];
                            }
                            break;
                    }
                }
                else
                {
                    switch (arg.Trim().ToLower())
                    {
                        default:
                            Help.ShowError("Unrecognized argument '" + arg + "'!");
                            break;
                        case "":
                            break;
                    }
                }
            }
        }

        /* If you want to make a dependency optional, you can use a try/catch block.  --Kris */
        static void runTests()
        {
            /* The properties we'll be attempting to retrieve and display.  --Kris */
            Dictionary<string, Dictionary<string, string>> fields = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> fStr;

            fields.Add("string", new Dictionary<string, string>());
            fields["string"].Add("LibName", null);
            fields["string"].Add("Version", null);
            fields["string"].Add("Author", null);
            fields["string"].Add("Email", null);
            fields["string"].Add("Repo", null);

            // Repeat the same process for int and other types as-needed.  --Kris

            /* Load the DLL.  --Kris */
            csLog = Assembly.LoadFile(basedir + @"\csLog.dll");

            /* Retrieve the "Log" class definition.  --Kris */
            csLogType = csLog.GetType("csLog.Log");

            /* Instantiate the "Log" class.  --Kris */
            csLogInstance = Activator.CreateInstance(csLogType);
            
            /* Retrieve and display the string fields.  --Kris */
            fStr = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> field in fields["string"])
            {
                fStr.Add(field.Key, field.Value);  // Simply setting fStr to fields["string"] acts as a pointer for some reason and disrupts the foreach.  --Kris
            }

            foreach (KeyValuePair<string, string> field in fStr)
            {
                FieldInfo fieldInfo = csLogType.GetField(field.Key);
                fields["string"][field.Key] = (string)fieldInfo.GetValue(null);

                Console.WriteLine(field.Key + @":  " + fields["string"][field.Key]);
            }

            // Other types (int, etc) go here.
            // TODO - Streamline all types into a single loop?  The requisite casting might make it a bit tricky.

            /* Display all available methods.  --Kris */
            MethodInfo[] methodInfos = csLogType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);

            Console.WriteLine("\r\nAvailable Methods:\r\n");
            for (int i = 0; i < methodInfos.Count(); i++)
            {
                Console.WriteLine(methodInfos[i].ToString());
            }

            /*
             * Tests specific to logging functionality.  --Kris
             */
            Console.WriteLine("\r\nTesting Log Functions:\r\n");

            /* Initialize three new logs.  --Kris */
            csLogType.InvokeMember("Init", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, csLogInstance,
                new object[] { "StringLog", "string" });
            csLogType.InvokeMember("Init", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, csLogInstance,
                new object[] { "IntLog", "int" });
            csLogType.InvokeMember("Init", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, csLogInstance,
                new object[] { "BoolLog", "bool" });

            /* Add some crap to the string log.  --Kris */
            csLogType.InvokeMember("Append", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public | BindingFlags.OptionalParamBinding, null, csLogInstance,
                new object[] { "StringLog", "I am a happy little bunny.  One day, I murdered an entire village.  Everyone died, except a spunky, never-give-up horse named Stevie.  " 
                    + "He gave-up and died the next day." });

            /* Increment the int log by 2 then decrement it by 1.  --Kris */
            csLogType.InvokeMember("Increment", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public | BindingFlags.OptionalParamBinding, null, csLogInstance,
                new object[] { "IntLog", 2 });
            csLogType.InvokeMember("Decrement", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public | BindingFlags.OptionalParamBinding, null, csLogInstance,
                new object[] { "IntLog" });

            /* Toggle the bool log to true.  --Kris */
            csLogType.InvokeMember("Toggle", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, csLogInstance,
                new object[] { "BoolLog" });

            /* Output the data from each log.  --Kris */
            Console.WriteLine("StringLog:  " +
                (string)csLogType.InvokeMember("GetData", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, csLogInstance,
                    new object[] { "StringLog" }));
            Console.WriteLine("IntLog:  " +
                (string)csLogType.InvokeMember("GetData", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, csLogInstance,
                    new object[] { "IntLog" }));
            Console.WriteLine("BoolLog:  " +
                (string)csLogType.InvokeMember("GetData", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, csLogInstance,
                    new object[] { "BoolLog" }));

            /* Save the logs.  --Kris */
            csLogType.InvokeMember("Save", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, csLogInstance,
                null);

            Console.WriteLine("Logs saved.");

            Console.WriteLine("\r\nAll tests completed successfully!");
        }
    }
}
