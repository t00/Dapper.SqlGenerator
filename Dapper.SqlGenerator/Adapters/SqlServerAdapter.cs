using System.Text;

namespace Dapper.SqlGenerator.Adapters
{
    public class SqlServerAdapter : ISqlAdapter
    {
        public void EscapeColumnName(StringBuilder sb, string name)
        {
            // TODO: escape quotes
            sb.AppendFormat("[{0}]", name);
        }
        
        public string EscapeTableName(string name)
        {
            // TODO: escape quotes
            return $"[{name}]";
        }

    }
}