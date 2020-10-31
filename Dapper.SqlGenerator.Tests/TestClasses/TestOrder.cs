namespace Dapper.SqlGenerator.Tests.TestClasses
{
    public class TestOrder
    {
        public int OrderId { get; set; }
        
        public TestProduct Product { get; set; } 

        public int ProductId { get; set; }
        
        public int Count { get; set; }

        public bool ReadOnly { get; } = true;
    }

}