using System;

namespace Dapper.SqlGenerator
{
    [Flags]
    public enum ColumnSelection
    {
        Keys = 0x01,
        NonKeys = 0x02,
        All = Keys | NonKeys
    }
}