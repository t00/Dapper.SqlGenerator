namespace Dapper.SqlGenerator.NameConverters
{
    public class UpperCaseNameConverter : INameConverter
    {
        public string Convert(string name)
        {
            return name.ToUpper();
        }
    }
}