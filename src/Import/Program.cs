using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Import
{
    class Program

    {
        const string FILE = @"C:\Users\robin\Documents\Visual Studio 2015\Projects\JetPowerJack\music\jpj-dump-frames.txt";
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("SID2VT Import");
                Console.WriteLine("\r\nAre you sure you want to overwrite the existing data?");
                bool chosen = false;
                while (!chosen)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.N) return;
                    if (key.Key == ConsoleKey.Y) chosen = true;
                }
                string constr = ConfigurationManager.ConnectionStrings["DB"].ConnectionString;
                using (var con = new SqlConnection(constr))
                {
                    con.Open();
                    DeleteAll(con);
                    Console.WriteLine("\r\nProcessing SIDDUMP file: " + Path.GetFileName(FILE) + "...\r\n");
                    var lines = File.ReadLines(FILE);
                    int len = lines.Count();
                    int i = -1;
                    int frame;
                    Console.WriteLine("\r\n Frame   NoteA   NoteB   NoteC");
                    Console.WriteLine("==============================");
                    foreach (var line in lines)
                    {
                        i++;
                        if (i < 7)
                            continue;
                        string f = line.Substring(1, 7).Trim();
                        int.TryParse(f, out frame);
                        int offset = f.ToString().Length;
                        offset = offset < 6 ? 0 : 1;
                        string a = line.Substring(16 + offset, 3).Trim().ToUpper();
                        if (a == "...") a = null;
                        string b = line.Substring(44 + offset, 3).Trim().ToUpper();
                        if (b == "...") b = null;
                        string c = line.Substring(72 + offset, 3).Trim().ToUpper();
                        if (c == "...") c = null;
                        if (a == null && b == null && c == null)
                            continue;
                        AddNote(con, frame, a, b, c);
                        string F = frame.ToString().PadLeft(6);
                        string A = (a ?? "---").Trim().PadRight(5);
                        string B = (a ?? "---").Trim().PadRight(5);
                        string C = (c ?? "---").Trim().PadRight(5);
                        Console.WriteLine(string.Format("{0}   {1}   {2}   {3}", F, A, B, C));
                    }
                }
            }
            finally
            {
                Console.WriteLine("\r\nPress any key to exit...");
                Console.ReadKey(true);
            }
        }

        private static void DeleteAll(SqlConnection ConX)
        {
            string sql = "DELETE FROM Sequence;DELETE FROM Pattern;DELETE FROM Frame;";
            var cmd = new SqlCommand(sql, ConX);
            cmd.ExecuteNonQuery();
        }

        private static void AddNote(SqlConnection ConX, int Frame, string NoteA, string NoteB, string NoteC)
        {
            string sql = "INSERT INTO Frame (FrameID,A,B,C) VALUES (@FrameID,@A,@B,@C);";
            var cmd = new SqlCommand(sql, ConX);
            cmd.Parameters.AddWithValue("FrameID", Frame);
            cmd.Parameters.AddWithValueNullable("A", NoteA);
            cmd.Parameters.AddWithValueNullable("B", NoteB);
            cmd.Parameters.AddWithValueNullable("C", NoteC);
            cmd.ExecuteNonQuery();
        }
    }
}

