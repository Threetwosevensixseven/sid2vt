using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Export.Classes
{
    public class Pattern
    {
        public int PatternID { get; set; }
        public int FrameStart { get; set; }
        public int FrameEnd { get; set; }
        public List<Frame> Frames { get; set; }

        public Pattern()
        {
            Frames = new List<Frame>();
        }

        public static Pattern Load(SqlConnection Con, int PatternID)
        {
            var pattern = new Pattern();
            string sql = "SELECT * FROM Pattern WHERE PatternID=" + PatternID;
            using (var cmd = Con.CreateCommand())
            {
                cmd.CommandText = sql;
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        pattern.PatternID = (int)dr["PatternID"];
                        pattern.FrameStart = (int)dr["FrameStart"];
                        pattern.FrameEnd = (int)dr["FrameEnd"];
                        break;
                    }
                }
            }
            Frame.Load(Con, pattern);
            return pattern;
        }
    }
}