namespace Dapper.SqlGenerator.NameConverters
{
    public class CamelCaseNameConverter : INameConverter
    {
        private readonly bool lowerInitialAcronym;

        public CamelCaseNameConverter(bool lowerInitialAcronym = true)
        {
            this.lowerInitialAcronym = lowerInitialAcronym;
        }
        
        public string Convert(string name)
        {
            var i = 0;
            while (i < name.Length && char.IsUpper(name[i]))
            {
                i++;
            }

            if (i == 0 || name.Length == 0)
            {
                return name;
            }

            if (i > 1 && lowerInitialAcronym)
            {
                return name.Substring(0, i - 1).ToLowerInvariant() + name.Substring(i - 1);
            }
            
            return char.ToLowerInvariant(name[0]) + name.Substring(1);
        }
    }
}