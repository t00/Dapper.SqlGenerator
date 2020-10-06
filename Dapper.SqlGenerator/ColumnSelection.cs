using System;

namespace Dapper.SqlGenerator
{
    [Flags]
    public enum ColumnSelection
    {
        None = 0x00,
        Keys = 0x01,
        NonKeys = 0x02,
        Computed = 0x04,
        Write = 0x08,
        Select = Keys | NonKeys | Computed
    }
}