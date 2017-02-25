using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Export.Classes
{
    public class Sequence
    {
        public List<Pattern> Patterns { get; set; }

        public Sequence()
        {
            Patterns = new List<Pattern>();
        }

        public static Sequence Load(SqlConnection Con)
        {
            var seq = new Sequence();
            var pids = new List<int>();
            string sql = "SELECT * FROM [Sequence] ORDER BY SeqID;";
            using (var cmd = Con.CreateCommand())
            {
                cmd.CommandText = sql;
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        pids.Add((int)dr["PatternID"]);
                    }
                }
            }
            foreach (var pid in pids)
            {
                seq.Patterns.Add(Pattern.Load(Con, pid));
            }
            return seq;
        }
    }
}
