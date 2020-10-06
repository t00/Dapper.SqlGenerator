using System.Text;

namespace Dapper.SqlGenerator.Adapters
{
    public class GenericSqlAdapter : ISqlAdapter
    {
        public void EscapeColumnName(StringBuilder sb, string name)
        {
            // TODO: escape quotes
            sb.Append($"\"{name}\"");
        }

        public string EscapeTableName(string name)
        {
            // TODO: escape quotes
            return $"\"{name}\"";
        }
    }
}