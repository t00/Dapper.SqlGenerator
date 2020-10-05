using System.Text;

namespace Dapper.SqlGenerator
{
    public interface ISqlAdapter
    {
        /// <summary>
        /// Adds the name of a column.
        /// </summary>
        /// <param name="sb">The string builder to append to.</param>
        /// <param name="name">The property name.</param>
        void EscapeColumnName(StringBuilder sb, string name);
    }
}