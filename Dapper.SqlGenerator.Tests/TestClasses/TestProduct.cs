using System;
using System.Collections.Generic;

namespace Dapper.SqlGenerator.Tests.TestClasses
{
    internal class TestProduct
    {
        public int Id { get; set; }
            
        public int Kind { get; set; }
            
        public string Name { get; set; }
            
        public string Content { get; set; }
            
        public int Value { get; set; }
        
        public KeyValuePair<int, string> Struct { get; set; }
        
        public Exception Class { get; set; }

        public TestEnum Enum { get; set; }
        
        public DateTime? MaybeDate { get; set; }
        
        public DateTime Date { get; set; }
        
        public Guid? MaybeGuid { get; set; }
        
        public Guid Guid { get; set; }
        
        public TimeSpan Duration { get; set; }

        public bool Last { get; set; }
    }

    internal enum TestEnum
    {
        None,
        Some = 1,
        All = 2
    }
}