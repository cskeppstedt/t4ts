using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public partial class TypeName
    {
        private static class DteParser
        {
            public static TypeName Parse(string rawName)
            {
                string unqualifiedName;
                IList<TypeName> typeArguments;

                int openAngleIndex = rawName.IndexOf('<');
                if (openAngleIndex >= 0)
                {
                    unqualifiedName = rawName.Substring(
                        0,
                        openAngleIndex);

                    int closeAngleIndex = rawName.LastIndexOf('>');
                    typeArguments = DteParser.ParseTypeArguments(
                        rawName.Substring(
                            openAngleIndex + 1,
                            closeAngleIndex - (openAngleIndex + 1)));
                }
                else if (rawName.EndsWith(TypeName.ArraySuffix))
                {
                    unqualifiedName = TypeName.ArraySuffix;
                    typeArguments = new List<TypeName>()
                    {
                        DteParser.Parse(
                            rawName.Substring(
                                0,
                                rawName.Length - TypeName.ArraySuffix.Length))
                    };
                }
                else
                {
                    unqualifiedName = rawName;
                    typeArguments = new List<TypeName>();
                }

                return new TypeName(
                    rawName,
                    unqualifiedName,
                    typeArguments);
            }

            private static IList<TypeName> ParseTypeArguments(string argumentsString)
            {
                IList<TypeName> result = new List<TypeName>();

                int openBraceCount = 0;
                int argumentStartIndex = 0;
                int index = 0;
                foreach (char currentChar in argumentsString)
                {
                    if (currentChar == ','
                        && openBraceCount == 0)
                    {
                        result.Add(DteParser.Parse(
                            argumentsString.Substring(
                                argumentStartIndex,
                                index - argumentStartIndex)));
                        argumentStartIndex = index + 1;
                    }

                    if (currentChar == '<')
                    {
                        openBraceCount++;
                    }
                    else if (currentChar == '>')
                    {
                        openBraceCount--;
                    }
                    index++;
                }

                result.Add(DteParser.Parse(
                    argumentsString.Substring(
                        argumentStartIndex,
                        index - argumentStartIndex)));

                return result;
            }
        }
    }
}
