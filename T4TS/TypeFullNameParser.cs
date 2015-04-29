using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    class TypeFullNameParser
    {
        private static TypeFullNameResult ParseCSharp(string fullNameFromType)
        {
            string fullName = fullNameFromType.Substring(0, fullNameFromType.IndexOf("<"));
            string restPart = fullNameFromType.Substring(fullNameFromType.IndexOf("<"));
            int openBraceCount = 0;
            int count = 0;
            List<TypeFullNameResult> typeArguments = new List<TypeFullNameResult>();
            List<char> chars = new List<char>();
            foreach (var c in restPart)
            {
                if ((c != '<' && c != '>' && c != ',') || openBraceCount > 1)
                {
                        chars.Add(c);
                }
                if(c == ',' && openBraceCount==1)
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
            return new TypeFullNameResult() { FullName = fullName, TypeArgumentFullNames =typeArguments.ToArray() };
        }
        public static TypeFullNameResult Parse(string fullNameFromType)
        {
            
            if (fullNameFromType.Contains("<"))
                return ParseCSharp(fullNameFromType);
            if (!fullNameFromType.Contains("`"))
            {
                if (fullNameFromType.Contains(","))
                    return Parse(fullNameFromType.Substring(0, fullNameFromType.IndexOf(",")));
                return new TypeFullNameResult() { FullName = fullNameFromType };
            }

            string fullName = fullNameFromType.Substring(0, fullNameFromType.IndexOf("`"));
            string restPart = fullNameFromType.Substring(fullNameFromType.IndexOf("[") + 1);
            restPart = restPart.Substring(0, restPart.Length - 1);
            int openBraceCount = 0;
            int count = 0;
            List<TypeFullNameResult> typeArguments = new List<TypeFullNameResult>();
            List<char> chars = new List<char>();
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
            return new TypeFullNameResult() { FullName = fullName, TypeArgumentFullNames = typeArguments.ToArray() };
            //System.Collections.Generic.Dictionary`2[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Object, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]] 
            //System.Collections.Generic.Dictionary`2[[System.Collections.Generic.List`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Object, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]
        }
    }
    public class TypeFullNameResult
    {
        public string FullName { get; set; }
        public TypeFullNameResult[] TypeArgumentFullNames { get; set; }
    }
}
