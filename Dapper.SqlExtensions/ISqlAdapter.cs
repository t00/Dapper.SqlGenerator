using System.Text;

namespace Dapper.SqlExtensions
{
    public interface ISqlAdapter
    {
        /// <summary>
        /// Adds the name of a column.
        /// </summary>
        /// <param name="sb">The string builder to append to.</param>
        /// <param name="property">The property name.</param>
        void AppendColumn(StringBuilder sb, PropertyBuilder property);
    }
}