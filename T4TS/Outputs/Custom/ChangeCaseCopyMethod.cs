﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Outputs.Custom
{
    public partial class ChangeCaseCopyMethod : TypeScriptMethod
    {
        public static TypeScriptMethod Create(
            OutputSettings outputSettings,
            TypeContext typeContext,
            string name,
            TypeScriptType containingType,
            string otherTypeLiteral,
            bool toContainingType,
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
                toContainingType,
                toCamelCase);

            if (toContainingType)
            {
                result.IsStatic = true;
                result.Arguments = new List<TypeScriptMember>()
                {
                    new TypeScriptMember()
                    {
                        Name = "other",
                        Type = otherType
                    }
                };
                result.Type = containingType;
            }
            else
            {
                result.Arguments = new List<TypeScriptMember>();
                result.Type = otherType;
            }

            return result;
        }
    }
}