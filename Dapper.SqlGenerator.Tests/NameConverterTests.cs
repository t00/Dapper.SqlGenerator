using Dapper.SqlGenerator.NameConverters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dapper.SqlGenerator.Tests
{
    [TestClass]
    public class NameConverterTests
    {
        [TestMethod]
        public void TestLowerCase()
        {
            var n = new LowerCaseNameConverter().Convert("SQLPropertyNameANDAcronymsLAST");
            Assert.AreEqual("sqlpropertynameandacronymslast", n);
        }
        
        [TestMethod]
        public void TestUpperCase()
        {
            var n = new UpperCaseNameConverter().Convert("SQLPropertyNameANDAcronymsLAST");
            Assert.AreEqual("SQLPROPERTYNAMEANDACRONYMSLAST", n);
        }
        
        [TestMethod]
        public void TestCamelCase()
        {
            var n1 = new CamelCaseNameConverter().Convert("SQLPropertyNameANDAcronymsLAST");
            Assert.AreEqual("sqlPropertyNameANDAcronymsLAST", n1);
            var n2 = new CamelCaseNameConverter(false).Convert("SQLPropertyNameANDAcronymsLAST");
            Assert.AreEqual("sQLPropertyNameANDAcronymsLAST", n2);
            var n3 = new CamelCaseNameConverter().Convert("PropertyName");
            Assert.AreEqual("propertyName", n3);
        }
        
        [TestMethod]
        public void TestSnakeCase()
        {
            var n1 = new SnakeCaseNameConverter().Convert("SQLPropertyNameANDAcronymsLAST");
            Assert.AreEqual("sql_property_name_and_acronyms_last", n1);
            var n2 = new SnakeCaseNameConverter().Convert("PropertyName");
            Assert.AreEqual("property_name", n2);
            var n3 = new SnakeCaseNameConverter().Convert("propertyNameANDAcronymF");
            Assert.AreEqual("property_name_and_acronym_f", n3);
        }
    }
}