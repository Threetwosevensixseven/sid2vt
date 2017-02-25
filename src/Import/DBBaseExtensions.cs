using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Import
{
    public static class DBBaseExtensions
    {
        public static SqlParameter AddWithValueNullable<T>(this SqlParameterCollection Parameters, string parameterName, T value,
            bool TreatDefaultValueAsNull = true, bool TreatEmptyStringAsNull = true)
        {
            SqlParameter rv;
            if (value == null
                || (TreatDefaultValueAsNull && EqualityComparer<T>.Default.Equals(value, default(T)))
                || (TreatEmptyStringAsNull && value.GetType() == typeof(string) && value.Equals(string.Empty)))
            {
                rv = Parameters.AddWithValue(parameterName, default(T));
                rv.Value = DBNull.Value;
            }
            else if (value.GetType() == typeof(bool))
                rv = Parameters.AddWithValue(parameterName, (bool)(object)value ? "T" : "F");
            else
                rv = Parameters.AddWithValue(parameterName, value);
            rv.IsNullable = true;
            return rv;
        }
    }
}
