using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Export.Classes
{
    public class Frame
    {
        public int FrameID { get; set; }
        public string NoteA { get; set; }
        public string NoteB { get; set; }
        public string NoteC { get; set; }

        public static void Load(SqlConnection Con, Pattern Pattern)
        {
            string sql = "SELECT * FROM Frame "
                + "WHERE FrameID>=" + Pattern.FrameStart 
                + " AND FrameID<=" + Pattern.FrameEnd
                + " ORDER BY FrameID; ";
            using (var cmd = Con.CreateCommand())
            {
                cmd.CommandText = sql;
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var f = new Frame();
                        f.FrameID = (int)dr["FrameID"];
                        f.NoteA = DRString(dr, "A");
                        f.NoteB = DRString(dr, "B");
                        f.NoteC = DRString(dr, "C");
                        Pattern.Frames.Add(f);
                    }
                }
            }
        }

        private static string DRString(SqlDataReader dr, string ColumnName)
        {
            var obj = dr[ColumnName];
            if (obj == null || obj == DBNull.Value) return null;
            else return obj.ToString();
        }
    }
}
