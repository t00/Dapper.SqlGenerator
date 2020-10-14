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
            Assert.AreEqual("SQL_Property_Name_AND_Acronyms_LAST", n1);
            var n2 = new SnakeCaseNameConverter().Convert("PropertyName");
            Assert.AreEqual("Property_Name", n2);
            var n3 = new SnakeCaseNameConverter().Convert("propertyNameANDAcronymF");
            Assert.AreEqual("property_Name_AND_Acronym_F", n3);
        }

        [TestMethod]
        public void TestPlural()
        {
            var n1 = new PluralNameConverter().Convert("mushroom");
            Assert.AreEqual("mushrooms", n1);
            var n2 = new PluralNameConverter().Convert("Settings");
            Assert.AreEqual("Settings", n2);
            var n3 = new PluralNameConverter().Convert("user_role");
            Assert.AreEqual("user_roles", n3);
        }
    }
}