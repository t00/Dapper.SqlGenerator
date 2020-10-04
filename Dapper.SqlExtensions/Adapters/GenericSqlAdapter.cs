using System.Text;

namespace Dapper.SqlExtensions.Adapters
{
    public class GenericSqlAdapter : ISqlAdapter
    {
        /// <summary>
        /// Adds the name of a column.
        /// </summary>
        /// <param name="sb">The string builder to append to.</param>
        /// <param name="property">The property name.</param>
        public void AppendColumn(StringBuilder sb, PropertyBuilder property)
        {
            sb.AppendFormat("\"{0}\"", property.ColumnName ?? property.Name);
        }
    }
}