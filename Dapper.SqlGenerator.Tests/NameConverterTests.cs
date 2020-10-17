using Dapper.SqlGenerator.NameConverters;
using NUnit.Framework;

namespace Dapper.SqlGenerator.Tests
{
    [TestFixture]
    public class NameConverterTests
    {
        [Test]
        [TestCase("SQLPropertyNameANDAcronymsLAST", "sqlpropertynameandacronymslast")]
        public void TestLowerCase(string text, string expected)
        {
            var n = new LowerCaseNameConverter().Convert(text);
            Assert.AreEqual(expected, n);
        }
        
        [Test]
        [TestCase("SQLPropertyNameANDAcronymsLAST", "SQLPROPERTYNAMEANDACRONYMSLAST")]
        public void TestUpperCase(string text, string expected)
        {
            var n = new UpperCaseNameConverter().Convert(text);
            Assert.AreEqual(expected, n);
        }
        
        [Test]
        [TestCase("SQLPropertyNameANDAcronymsLAST", true, "sqlPropertyNameANDAcronymsLAST")]
        [TestCase("SQLPropertyNameANDAcronymsLAST", false, "sQLPropertyNameANDAcronymsLAST")]
        [TestCase("PropertyName", true, "propertyName")]
        [TestCase("PropertyName", false, "propertyName")]
        public void TestCamelCase(string text, bool lowerInitialAcronym, string expected)
        {
            var n = new CamelCaseNameConverter(lowerInitialAcronym).Convert(text);
            Assert.AreEqual(expected, n);
        }
        
        [Test]
        [TestCase("SQLPropertyNameANDAcronymsLAST", "SQL_Property_Name_AND_Acronyms_LAST")]
        [TestCase("PropertyName", "Property_Name")]
        [TestCase("propertyNameANDAcronymF", "property_Name_AND_Acronym_F")]
        [TestCase("propertyNameANDAcronymF", "property=>Name=>AND=>Acronym=>F", "=>")]
        public void TestSnakeCase(string text, string expected, string separator = null)
        {
            var converter = separator == null ? new SnakeCaseNameConverter() : new SnakeCaseNameConverter(separator);
            var n = converter.Convert(text);
            Assert.AreEqual(expected, n);
        }

        [Test]
        [TestCase("mushroom", "mushrooms")]
        [TestCase("Settings", "Settings")]
        [TestCase("user_role", "user_roles")]
        public void TestPlural(string text, string expected)
        {
            var n = new PluralNameConverter().Convert(text);
            Assert.AreEqual(expected, n);
        }
    }
}