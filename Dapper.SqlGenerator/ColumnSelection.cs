using System;

namespace Dapper.SqlGenerator
{
    [Flags]
    public enum ColumnSelection
    {
        Keys,
        NonKeys,
        All = Keys | NonKeys
    }
}