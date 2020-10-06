using System;

namespace Dapper.SqlGenerator
{
    [Flags]
    public enum ColumnSelection
    {
        Keys = 0x01,
        NonKeys = 0x02,
        Computed = 0x04,
        Select = Keys | NonKeys | Computed,
        Insert = Keys | NonKeys,
        Update = Keys | NonKeys
    }
}