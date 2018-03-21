using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public class TypeFullNameParser
    {
        private static TypeFullName ParseCSharp(string fullNameFromType)
        {
            string fullName = fullNameFromType.Substring(0, fullNameFromType.IndexOf("<"));
            string restPart = fullNameFromType.Substring(fullNameFromType.IndexOf("<"));

            int openBraceCount = 0;
            int count = 0;

            var typeArguments = new List<TypeFullName>();
            var chars = new List<char>();

            foreach (var c in restPart)
            {
                if ((c != '<' && c != '>' && c != ',') || openBraceCount > 1)
                {
                    chars.Add(c);
                }

                if (c == ',' && openBraceCount == 1)
                {
                    typeArguments.Add(Parse(new String(chars.ToArray()).Trim()));
                    chars.Clear();
                }

                if (c == '<')
                {
                    openBraceCount++;
                }
                else if (c == '>')
                {
                    openBraceCount--;

                    if (openBraceCount == 0)
                    {
                        typeArguments.Add(Parse(new String(chars.ToArray()).Trim()));
                        chars.Clear();
                    }
                }

                count++;
            }

            return new TypeFullName(fullName, typeArguments.ToArray());
        }

        public static TypeFullName Parse(string fullNameFromType)
        {
            if (fullNameFromType.Contains("<"))
                return ParseCSharp(fullNameFromType);

            if (!fullNameFromType.Contains("`"))
            {
                if (fullNameFromType.Contains(","))
                    return Parse(fullNameFromType.Substring(0, fullNameFromType.IndexOf(",")));

                if (fullNameFromType.EndsWith("[]"))
                {
                    var parameterName = new TypeFullName(fullNameFromType.Substring(
                        0,
                        fullNameFromType.LastIndexOf("[")));
                    return new TypeFullName(
                        fullNameFromType,
                        parameterName);
                }

                return new TypeFullName(fullNameFromType);
            }

            string fullName = fullNameFromType.Substring(0, fullNameFromType.IndexOf("`"));
            string restPart = fullNameFromType.Substring(fullNameFromType.IndexOf("[") + 1);

            restPart = restPart.Substring(0, restPart.Length - 1);

            int openBraceCount = 0;
            int count = 0;

            var typeArguments = new List<TypeFullName>();
            var chars = new List<char>();

            foreach (var c in restPart)
            {
                if ((c != '[' && c != ']') || openBraceCount > 1)
                {
                    if (c != ',' || openBraceCount != 0)
                        chars.Add(c);
                }

                if (c == '[')
                {
                    openBraceCount++;
                }
                else if (c == ']')
                {
                    openBraceCount--;

                    if (openBraceCount == 0)
                    {
                        typeArguments.Add(Parse(new String(chars.ToArray())));
                        chars.Clear();
                    }
                }

                count++;
            }

            return new TypeFullName(fullName, typeArguments.ToArray());
        }
    }
}