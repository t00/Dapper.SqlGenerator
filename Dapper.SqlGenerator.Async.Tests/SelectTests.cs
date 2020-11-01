using System;
using System.Linq;
using System.Threading.Tasks;
using Dapper.SqlGenerator.Tests.TestClasses;
using NUnit.Framework;

namespace Dapper.SqlGenerator.Async.Tests
{
    [TestFixture]
    public class SelectTests
    {
        [Test]
        public async Task TestSelect()
        {
            var data = new QueryTestData();
            using var connection = await data.SetUp();

            connection.Sql().HasColumnSet<TestProduct>("id+kind+content", x => x.Id, x => x.Kind, x => x.Content);
            var all = (await connection.SelectAsync<TestProduct>("id+kind+content")).ToList();
            Assert.AreEqual(2, all.Count());
            CollectionAssert.AreEquivalent(new[] { 5, 7 }, all.Select(x => x.Kind));
        }
        
        [Test]
        public async Task TestSelectWhere()
        {
            var data = new QueryTestData();
            using var connection = await data.SetUp();

            var filtered = (await connection.SelectWhereAsync<TestProduct>("WHERE Kind > 5")).Single();
            Assert.AreEqual(7, filtered.Kind);
        }

        [Test]
        public async Task TestSelectFirst()
        {
            var data = new QueryTestData();
            using var connection = await data.SetUp();

            var loadedP2 = await connection.SelectFirstAsync<TestProduct>(new { Id = data.Id2 });
            Assert.AreEqual(7, loadedP2.Kind);
            Assert.AreEqual(null, loadedP2.Name);
            Assert.AreEqual("Full box", loadedP2.Content);
            Assert.AreEqual(987, loadedP2.Value);
            Assert.AreEqual(TestEnum.None, loadedP2.Enum);
            Assert.IsTrue(loadedP2.Date < DateTime.UtcNow && loadedP2.Date > DateTime.UtcNow.AddSeconds(-5));
            
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                using var connection2 = await data.SetUp();
                var _ = await connection2.SelectFirstAsync<TestProduct>(new { Id = -1 });
            });
            
            Assert.DoesNotThrowAsync(async () =>
            {
                using var connection2 = await data.SetUp();
                var _ = await connection2.SelectFirstOrDefaultAsync<TestProduct>(new { Id = -1 });
            });
        }

        [Test]
        public async Task TestSelectSingle()
        {
            var data = new QueryTestData();
            using var connection = await data.SetUp();

            var loadedP1 = await connection.SelectSingleAsync<TestProduct>(new { Id = data.Id1 });
            Assert.AreEqual(5, loadedP1.Kind);
            Assert.AreEqual(null, loadedP1.Name);
            Assert.AreEqual("Empty box", loadedP1.Content);
            Assert.AreEqual(-5, loadedP1.Value);
            Assert.AreEqual(TestEnum.All, loadedP1.Enum);
            Assert.IsTrue(loadedP1.Date < DateTime.UtcNow && loadedP1.Date > DateTime.UtcNow.AddSeconds(-5));

            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                using var connection2 = await data.SetUp();
                var _ = await connection2.SelectSingleAsync<TestProduct>(new { Id = -1 });
            });
            
            Assert.DoesNotThrowAsync(async () =>
            {
                using var connection2 = await data.SetUp();
                var _ = await connection2.SelectSingleOrDefaultAsync<TestProduct>(new { Id = -1 });
            });
        }
    }
}