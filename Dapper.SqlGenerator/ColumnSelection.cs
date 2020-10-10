using System;

namespace Dapper.SqlGenerator
{
    [Flags]
    public enum ColumnSelection
    {
        None = 0x00,
        
        /// <summary>
        /// Only keys
        /// </summary>
        Keys = 0x01,
        
        /// <summary>
        /// Columns not defined as keys
        /// </summary>
        NonKeys = 0x02,
        
        /// <summary>
        /// Computed columns are only for SELECT statements
        /// </summary>
        Computed = 0x04,
        
        /// <summary>
        /// Flags the column selection to include columns or parameters for Insert and Update operations with a CAST if necessary 
        /// </summary>
        Write = 0x08,
        
        /// <summary>
        /// Alias to select all columns
        /// </summary>
        Select = Keys | NonKeys | Computed
    }
}