using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper.SqlGenerator.Tests.TestClasses;
using NUnit.Framework;

namespace Dapper.SqlGenerator.Async.Tests
{
    [TestFixture]
    public class QueryTests
    {
        [Test]
        public async Task TestInsert()
        {
            var data = new QueryTestData();
            var connection = await data.SetUp();

            var p3 = new TestProduct
            {
                Kind = 3,
                Content = "Reboot",
                Value = 5,
                Date = DateTime.UtcNow,
                MaybeDate = DateTime.UtcNow,
                Last = false
            };

            connection.Sql().HasColumnSet<TestProduct>("kind+content+value+date+last", x => x.Kind, x => x.Content, x => x.Value, x => x.Date, x => x.Last);
            var rows = await connection.InsertAsync(p3, "kind+content+value+date+last");
            Assert.AreEqual(1, rows);

            var product = (await connection.SelectWhereAsync<TestProduct>("WHERE Content = 'Reboot'")).Single();
            Assert.IsTrue(product.Id > 0);
            Assert.AreEqual(p3.Content, product.Content);
            Assert.AreEqual(p3.Value, product.Value);
            Assert.IsNull(product.MaybeDate);
        }
        
        [Test]
        public async Task TestInsertReturn()
        {
            var data = new QueryTestData();
            var _ = await data.SetUp();
        }
        
        [Test]
        public async Task TestUpdate()
        {
            var data = new QueryTestData();
            var connection = await data.SetUp();
            
            connection.Sql().HasColumnSet<TestProduct>("kind+content", x => x.Kind, x => x.Content);
            var p1Changed = new TestProduct
            {
                Id = data.Id1,
                Kind = 999,
                Content = "Modified"
            };
            
            var rows = await connection.UpdateAsync(p1Changed, "kind+content");
            Assert.AreEqual(1, rows);
           
            var product = (await connection.SelectWhereAsync<TestProduct>("WHERE Content = 'Modified'")).Single();
            Assert.IsTrue(product.Id == data.Id1);
            Assert.AreEqual("Modified", product.Content);
            Assert.AreEqual(999, product.Kind);
            Assert.AreEqual(-5, product.Value);
            Assert.AreEqual(TestEnum.All, product.Enum);
        }         
        
        [Test]
        public async Task TestDelete()
        {
            var data = new QueryTestData();
            var connection = await data.SetUp();

            var delete2 = new TestProduct
            {
                Id = data.Id2
            };
            var rows = await connection.DeleteAsync(delete2);
            Assert.AreEqual(1, rows);
           
            var products = (await connection.SelectAsync<TestProduct>()).ToList();
            Assert.AreEqual(1, products.Count);
        }         
    }
}