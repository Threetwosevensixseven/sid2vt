using Export.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Export
{
    class Program
    {
        const string DIR = @"C:\Users\robin\Documents\Visual Studio 2015\Projects\JetPowerJack\music\AY";

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("SID2VT Export");
                Console.WriteLine("\r\nCreating pattern files in:\r\n" + DIR);
                Console.WriteLine("\r\nAre you sure you want to overwrite the existing data?");
                bool chosen = false;
                while (!chosen)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.N) return;
                    if (key.Key == ConsoleKey.Y) chosen = true;
                }
                Export();
            }
            finally
            {
                Console.WriteLine("\r\nPress any key to exit...");
                Console.ReadKey(true);
            }
        }

        private static void Export()
        {
            string constr = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;
            using (var con = new SqlConnection(constr))
            {
                con.Open();
                var sequence = Sequence.Load(con);
                int len = 0;
                foreach (var p in sequence.Patterns)
                {
                    StringBuilder sb = new StringBuilder("[Pattern]\r\n");
                    for (int i = p.FrameStart; i <= p.FrameEnd; i++)
                    {
                        len++;
                        var frame = p.Frames.FirstOrDefault(f => f.FrameID == i);
                        if (frame == null) frame = new Frame();
                        sb.Append("....|..|");
                        sb.Append((frame.NoteA ?? "---").Trim().Substring(0, 3).PadLeft(3));
                        sb.Append(" .... ....|");
                        sb.Append((frame.NoteB ?? "---").Trim().Substring(0, 3).PadLeft(3));
                        sb.Append(" .... ....|");
                        sb.Append((frame.NoteB ?? "---").Trim().Substring(0, 3).PadLeft(3));
                        sb.Append(" .... ....\r\n");
                    }
                    if (!Directory.Exists(DIR))
                        Directory.CreateDirectory(DIR);
                    string fn = Path.Combine(DIR, "Pattern" + p.PatternID.ToString().PadLeft(3, '0')
                        + "-(" + len.ToString().PadLeft(3, '0') + ").txt");
                    File.WriteAllText(fn, sb.ToString());
                }
            }
        }
    }
}
