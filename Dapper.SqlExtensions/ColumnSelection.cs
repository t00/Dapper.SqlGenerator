using System;

namespace Dapper.SqlExtensions
{
    [Flags]
    public enum ColumnSelection
    {
        Keys,
        NonKeys,
        All = Keys | NonKeys
    }
}