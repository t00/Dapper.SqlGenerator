using System.Text;

namespace Dapper.SqlGenerator.NameConverters
{
    public class SnakeCaseNameConverter : INameConverter
    {
        private readonly string separator;

        public SnakeCaseNameConverter(string separator = "_")
        {
            this.separator = separator;
        }
        
        public string Convert(string name)
        {
            var sb = new StringBuilder();
            var firstUpper = -1;
            for (var i = 0; i < name.Length; i++)
            {
                var n = name[i];
                if (char.IsUpper(n))
                {
                    if (firstUpper < 0)
                    {
                        firstUpper = i;
                        if (i > 0)
                        {
                            sb.Append(separator);
                        }
                    }
                }
                else
                {
                    if (firstUpper >= 0)
                    {
                        var acronymLenfth = i - 1 - firstUpper;
                        if (acronymLenfth > 0)
                        {
                            sb.Append(name.Substring(firstUpper, i - 1 - firstUpper));
                            sb.Append(separator);
                        }

                        sb.Append(name[i - 1]);
                        firstUpper = -1;
                    }

                    sb.Append(n);
                }
            }

            if (firstUpper > 0)
            {
                sb.Append(name.Substring(firstUpper));
            }

            return sb.ToString();
        }
    }
}