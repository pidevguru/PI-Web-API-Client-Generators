namespace GeneratorPIWebApiClient.Core.Extensions
{
    public static class StringExtensions
    {
        public static string RemoveBracketsFromString(this string str)
        {
            return str
                .Replace("[", string.Empty)
                .Replace("]", string.Empty)
                .Replace("[", string.Empty)
                .Replace("]", string.Empty)
                .Replace("[", string.Empty)
                .Replace("]", string.Empty);
        }

        public static string ToFirstLetterLowerCase(this string name)
        {
            return name[0].ToString().ToLower() + name.Substring(1);
        }

        public static string ToFirstLetterUpperCase(this string name)
        {
            return name[0].ToString().ToUpper() + name.Substring(1);
        }

        public static string ToPIName(this string name)
        {
            return "PWA" + name.ToFirstLetterUpperCase().RemoveBracketsFromString();
        }

        public static string ToPythonFileName(this string name)
        {

            return name.RemoveBracketsFromString().ToPythonVariableName("pwa_").ToLower();
        }

        public static string ToPythonVariableName(this string name, string prefix = null)
        {
            string pythonName = string.Empty;
            for (int i = 0; i < name.Length; i++)
            {
                if (char.IsUpper(name[i]))
                {
                    pythonName += "_" + name[i].ToString().ToLower();
                }
                else
                {
                    pythonName += name[i].ToString();
                }
            }
            if (char.IsUpper(name[0]))
            {
                pythonName = pythonName.Substring(1);
            }


            if (string.IsNullOrEmpty(prefix) == true)
            {
                return pythonName;
            }
            else
            {
                return prefix + pythonName;
            }
        }
    }
}
