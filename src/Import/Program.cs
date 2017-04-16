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
        const string FILE = @"C:\Users\robin\Documents\Visual Studio 2015\Projects\Tumult\music\sid\equinoxe4-dump-frames.txt";
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
                string constr = ConfigurationManager.ConnectionStrings["Equinoxe4"].ConnectionString;
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

                        int? ifA = null;
                        int? iwfA = null;
                        int? iadsrA = null;
                        int? ipulA = null;
                        string fA = line.Substring(10 + offset, 5).Trim().ToUpper();
                        if (fA != "....") ifA = Convert.ToInt32("0x" + fA, 16);
                        string wfA = line.Substring(23 + offset, 4).Trim().ToUpper();
                        if (wfA != "..") iwfA = Convert.ToInt32("0x" + wfA, 16);
                        string adsrA = line.Substring(26 + offset, 6).Trim().ToUpper();
                        if (adsrA != "....") iadsrA = Convert.ToInt32("0x" + adsrA, 16);
                        string pulA = line.Substring(31 + offset, 5).Trim().ToUpper();
                        if (pulA != "...") ipulA = Convert.ToInt32("0x" + pulA, 16);

                        int? ifB = null;
                        int? iwfB = null;
                        int? iadsrB = null;
                        int? ipulB = null;
                        string fB = line.Substring(37 + offset, 6).Trim().ToUpper();
                        if (fB != "....") ifB = Convert.ToInt32("0x" + fB, 16);
                        string wfB = line.Substring(51 + offset, 4).Trim().ToUpper();
                        if (wfB != "..") iwfB = Convert.ToInt32("0x" + wfB, 16);
                        string adsrB = line.Substring(54 + offset, 6).Trim().ToUpper();
                        if (adsrB != "....") iadsrB = Convert.ToInt32("0x" + adsrB, 16);
                        string pulB = line.Substring(59 + offset, 5).Trim().ToUpper();
                        if (pulB != "...") ipulB = Convert.ToInt32("0x" + pulB, 16);

                        int? ifC = null;
                        int? iwfC = null;
                        int? iadsrC = null;
                        int? ipulC = null;
                        string fC = line.Substring(65 + offset, 6).Trim().ToUpper();
                        if (fC != "....") ifC = Convert.ToInt32("0x" + fC, 16);
                        string wfC = line.Substring(79 + offset, 4).Trim().ToUpper();
                        if (wfC != "..") iwfC = Convert.ToInt32("0x" + wfC, 16);
                        string adsrC = line.Substring(82 + offset, 6).Trim().ToUpper();
                        if (adsrC != "....") iadsrC = Convert.ToInt32("0x" + adsrC, 16);
                        string pulC = line.Substring(87 + offset, 5).Trim().ToUpper();
                        if (pulC != "...") ipulC = Convert.ToInt32("0x" + pulC, 16);

                        int? ifcut = null;
                        int? irc = null;
                        int? iv = null;
                        string fcut = line.Substring(93 + offset, 6).Trim().ToUpper();
                        if (fcut != "....") ifcut = Convert.ToInt32("0x" + fcut, 16);
                        string rc = line.Substring(98 + offset, 4).Trim().ToUpper();
                        if (rc != "..") irc = Convert.ToInt32("0x" + rc, 16);
                        string typ = line.Substring(101 + offset, 5).Trim();
                        if (typ == "...") typ = null;
                        string v = line.Substring(105 + offset, 3).Trim().ToUpper();
                        if (v != ".") iv = Convert.ToInt32("0x" + v, 16);

                        if (ifA == null && iwfA == null && iadsrA == null && ipulA == null
                            && ifB == null && iwfB == null && iadsrB == null && ipulB == null
                            && ifC == null && iwfC == null && iadsrC == null && ipulC == null
                            && ifcut == null && irc == null && typ == null && iv == null)
                            continue;
                        AddNote(con, frame,
                            ifA, iwfA, iadsrA, ipulA,
                            ifB, iwfB, iadsrB, ipulB,
                            ifC, iwfC, iadsrC, ipulC,
                            ifcut, irc, typ, iv);
                        string F = frame.ToString().PadLeft(6);
                        Console.WriteLine(string.Format("{0}   {1}   {2}   {3}", F, fA, fB, fC));
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

        private static void AddNote(SqlConnection ConX, int Frame, 
            int? ifA, int? iwfA, int? iadsrA, int? ipulA,
            int? ifB, int? iwfB, int? iadsrB, int? ipulB,
            int? ifC, int? iwfC, int? iadsrC, int? ipulC,
            int? ifcut, int? irc, string typ, int? iv)
        {
            string sql = @"INSERT INTO Frame (FrameID,
                    freqA,wfA,adsrA,pwA,
                    freqB,wfB,adsrB,pwB,
                    freqC,wfC,adsrC,pwC,
                    fcut,rc,typ,v) 
                VALUES (@FrameID,
                    @ifA,@iwfA,@iadsrA,@ipulA,
                    @ifB,@iwfB,@iadsrB,@ipulB,
                    @ifC,@iwfC,@iadsrC,@ipulC,
                    @ifcut,@irc,@typ,@iv);";
            var cmd = new SqlCommand(sql, ConX);
            cmd.Parameters.AddWithValue("FrameID", Frame);
            cmd.Parameters.AddWithValueNullable("ifA", ifA);
            cmd.Parameters.AddWithValueNullable("iwfA", iwfA);
            cmd.Parameters.AddWithValueNullable("iadsrA", iadsrA);
            cmd.Parameters.AddWithValueNullable("ipulA", ipulA);
            cmd.Parameters.AddWithValueNullable("ifB", ifB);
            cmd.Parameters.AddWithValueNullable("iwfB", iwfB);
            cmd.Parameters.AddWithValueNullable("iadsrB", iadsrB);
            cmd.Parameters.AddWithValueNullable("ipulB", ipulB);
            cmd.Parameters.AddWithValueNullable("ifC", ifC);
            cmd.Parameters.AddWithValueNullable("iwfC", iwfC);
            cmd.Parameters.AddWithValueNullable("iadsrC", iadsrC);
            cmd.Parameters.AddWithValueNullable("ipulC", ipulC);
            cmd.Parameters.AddWithValueNullable("ifcut", ifcut);
            cmd.Parameters.AddWithValueNullable("irc", irc);
            cmd.Parameters.AddWithValueNullable("typ", typ);
            cmd.Parameters.AddWithValueNullable("iv", iv);
            cmd.ExecuteNonQuery();
        }
    }
}

