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
            var id = await connection.InsertReturnAsync<TestProduct, int>(p3);
            Assert.IsTrue(id > 0);
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

            rows = await connection.DeleteAsync<TestProduct>(new { Id = data.Id1 });
            Assert.AreEqual(1, rows);
            
            products = (await connection.SelectAsync<TestProduct>()).ToList();
            Assert.AreEqual(0, products.Count);
        }
        
        [Test]
        public async Task TestMerge()
        {
            var data = new QueryTestData();
            var connection = await data.SetUp();
            
            var p3Conflicted = new TestProduct
            {
                Kind = 5,
                Content = "Empty box",
                Value = 666,
                Date = DateTime.UtcNow,
                Last = true
            };
            
            connection.Sql().HasColumnSet<TestProduct>("kind+content", x => x.Kind, x => x.Content);
            connection.Sql().HasColumnSet<TestProduct>("kind+content+value+date+last", x => x.Kind, x => x.Content, x => x.Value, x => x.Date, x => x.Last);
            var rows3 = await connection.MergeAsync(p3Conflicted, "kind+content", "kind+content+value+date+last");
            Assert.AreEqual(1, rows3);
           
            var product3 = (await connection.SelectWhereAsync<TestProduct>("WHERE Content = 'Empty box'")).Single();
            Assert.IsTrue(product3.Id == data.Id1);
            Assert.AreEqual(5, product3.Kind);
            Assert.AreEqual("Empty box", product3.Content);
            Assert.AreEqual(666, product3.Value);
            Assert.AreEqual(true, product3.Last);

            var p4New = new TestProduct
            {
                Kind = 6,
                Content = "Empty box",
                Value = 777,
                Date = DateTime.UtcNow,
                Last = true
            };
            
            var rows4 = await connection.MergeAsync(p4New, "kind+content", "kind+content+value+date+last");
            Assert.AreEqual(1, rows4);
           
            var product4 = (await connection.SelectWhereAsync<TestProduct>("WHERE Kind = 6")).Single();
            Assert.IsTrue(product4.Id != data.Id1);
            Assert.AreEqual(6, product4.Kind);
            Assert.AreEqual("Empty box", product4.Content);
            Assert.AreEqual(777, product4.Value);
            Assert.AreEqual(true, product4.Last);
        }         
    }
}