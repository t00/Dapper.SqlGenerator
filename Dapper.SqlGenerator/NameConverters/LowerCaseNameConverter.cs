namespace Dapper.SqlGenerator.NameConverters
{
    public class LowerCaseNameConverter : INameConverter
    {
        public string Convert(string name)
        {
            return name.ToLowerInvariant();
        }
    }
}