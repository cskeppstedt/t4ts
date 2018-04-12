using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Outputs.Custom
{
    public partial class ChangeCaseCopyMethod : TypeScriptMethod
    {
        private const string OtherParameterName = "other";

        public static TypeScriptConstructor CreateConstructor(
            OutputSettings outputSettings,
            TypeContext typeContext,
            TypeScriptType containingType,
            string otherTypeLiteral,
            bool toCamelCase)
        {
            TypeReference otherType = typeContext.GetLiteralReference(otherTypeLiteral);

            TypeScriptConstructor result = new TypeScriptConstructor();
            result.Appender = new ChangeCaseCopyMethod.OutputAppender(
                outputSettings,
                typeContext,
                containingType,
                otherType,
                toContainingType: true,
                toCamelCase: toCamelCase);
            
            result.Arguments = new List<TypeScriptMember>()
                {
                    new TypeScriptMember()
                    {
                        Name = ChangeCaseCopyMethod.OtherParameterName,
                        Type = otherType
                    }
                };

            if (containingType.Parent != null)
            { 
                result.BaseArguments = new List<string>()
                {
                    ChangeCaseCopyMethod.OtherParameterName
                };
            }

            if (containingType.SourceType.TypeArguments != null
                && containingType.SourceType.TypeArguments.Any())
            {
                result.TypeArguments = containingType.SourceType.TypeArguments
                    .Select((typeName) => typeContext.GetTypeReference(
                        typeName,
                        containingType))
                    .ToList();
            }

            return result;
        }

        public static TypeScriptMethod CreateMethod(
            OutputSettings outputSettings,
            TypeContext typeContext,
            string name,
            TypeScriptType containingType,
            string otherTypeLiteral,
            bool toCamelCase)
        {
            TypeReference otherType = typeContext.GetLiteralReference(otherTypeLiteral);

            TypeScriptMethod result = new TypeScriptMethod();
            result.Name = name;
            result.Appender = new ChangeCaseCopyMethod.OutputAppender(
                outputSettings,
                typeContext,
                containingType,
                otherType,
                toContainingType: false,
                toCamelCase: toCamelCase);
            result.Type = otherType;

            return result;
        }
    }
}
