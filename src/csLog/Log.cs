using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace csLog
{
    public class Log
    {
        public static string LibName = "csLog";
        public static string Version = "1.10";
        public static string Author = "Kris Craig";
        public static string Email = "kriscraig@php.net";
        public static string Repo = "https://github.com/sirkris/csLog";
        
        private Dictionary<string, string> buffer;
        private List<string> removebuf;
        public string timestamp;
        private string logdir;

        public Log(string logsdir, string logsubdir = null, string intimestamp = null, string tsformat = "yyyyMMdd-HHmmss.fffffff")
        {
            buffer = new Dictionary<string, string>();
            removebuf = new List<string>();

            if (intimestamp == null)
            {
                timestamp = DateTime.Now.ToString(tsformat);
            }
            else
            {
                timestamp = intimestamp;
            }
            
            logdir = logsdir + @"\recent\text\" + timestamp + @"\" + logsubdir;

            MakeDir();
        }

        public Log() : this(Environment.CurrentDirectory + @"\logs") { }

        public Dictionary<string, string> GetBuffer()
        {
            return buffer;
        }

        internal void MakeDir()
        {
            if (!(Directory.Exists(logdir)))
            {
                Directory.CreateDirectory(logdir);
            }
        }

        public void Init(string logname, string emutype = "string")
        {
            if (buffer.ContainsKey(logname))
            {
                buffer.Remove(logname);
            }

            switch (emutype.ToLower())
            {
                default:
                case "string":
                    buffer.Add(logname, null);
                    break;
                case "int":
                    buffer.Add(logname, "0");
                    break;
                case "bool":
                    buffer.Add(logname, "false");
                    break;
            }
        }

        public void Append(string logname, string text, bool newline = false)
        {
            if (buffer.ContainsKey(logname) == false)
            {
                Init(logname);
            }

            buffer[logname] += text;

            if (newline == true)
            {
                buffer[logname] += "\r\n";
            }
        }

        public bool Increment(string logname, int count = 1)
        {
            int dummy;

            if (buffer[logname] != null
                && Int32.TryParse(buffer[logname], out dummy) == false)
            {
                return false;
            }

            if (buffer[logname] == null)
            {
                buffer[logname] = "0";
            }

            buffer[logname] = (Int32.Parse(buffer[logname]) + count).ToString();

            return true;
        }

        public bool Decrement(string logname, int count = 1)
        {
            int dummy;

            if (buffer[logname] != null
                && Int32.TryParse(buffer[logname], out dummy) == false)
            {
                return false;
            }

            if (buffer[logname] == null)
            {
                buffer[logname] = "0";
            }

            buffer[logname] = (Int32.Parse(buffer[logname]) - count).ToString();

            return true;
        }

        public bool Toggle(string logname)
        {
            switch (buffer[logname].Trim().ToLower())
            {
                default:
                    return false;
                case "true":
                    SetBool(logname, false);
                    break;
                case "false":
                    SetBool(logname, true);
                    break;
            }

            return true;
        }

        public void SetBool(string logname, bool value = false)
        {
            buffer[logname] = value.ToString().ToLower();
        }

        public void Save(string logname, bool remove = true)
        {
            string path = logdir + logname + ".log";

            if (File.Exists(path) == false)
            {
                System.IO.File.WriteAllText(path, buffer[logname]);
            }
            else
            {
                System.IO.File.AppendAllText(path, buffer[logname]);
            }

            if (remove == true)
            {
                buffer.Remove(logname);
            }
            else
            {
                removebuf.Add(logname);
            }
        }

        public void Save()
        {
            removebuf = new List<string>();
            foreach (KeyValuePair<string, string> log in buffer)
            {
                Save(log.Key, false);
            }

            foreach (string logname in removebuf)
            {
                buffer.Remove(logname);
            }
        }

        public string GetData(string logname)
        {
            return buffer[logname];
        }

        internal string MergeDateTime(string logtext, string timestamp)
        {
            string outbuf;

            outbuf = "\r\n\r\n****************************************\r\n";
            outbuf += timestamp + "\r\n";
            outbuf += "****************************************\r\n\r\n";

            outbuf += logtext;

            return outbuf;
        }

        internal string MergeDuration(string logtext, string duration)
        {
            return Regex.Replace(logtext, @"[\r\n]*$", "") + "\r\n\r\nLoad Time:  " + duration + "\r\n";
        }

        public string Combine(Dictionary<int, Dictionary<string, string>> statements, Dictionary<int, string> times, Dictionary<int, string> timespans, string userclass, int statementkey)
        {
            string outbuf = null;
            string directives;

            foreach (KeyValuePair<int, Dictionary<string, string>> statement in statements)
            {
                if (statement.Key != statementkey)
                {
                    continue;
                }

                directives = null;
                foreach (KeyValuePair<string, string> directive in statement.Value)
                {
                    directives += "\t" + userclass + "." + directive.Key + " = " + directive.Value + "\r\n";
                }
                outbuf += "Statements:\r\n";
                outbuf += directives;
                outbuf = MergeDateTime(outbuf, times[statement.Key]);
                outbuf = MergeDuration(outbuf, timespans[statement.Key]);
            }

            return outbuf;
        }
    }
}
