using System.Text;

namespace Dapper.SqlGenerator.Adapters
{
    public class GenericSqlAdapter : ISqlAdapter
    {
        /// <summary>
        /// Adds the name of a column.
        /// </summary>
        /// <param name="sb">The string builder to append to.</param>
        /// <param name="name">The property name.</param>
        public void EscapeColumnName(StringBuilder sb, string name)
        {
            sb.AppendFormat("\"{0}\"", name);
        }
    }
}