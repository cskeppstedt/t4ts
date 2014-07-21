using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace T4TS.Build.Builder
{
    class SourceFileVisitor: CSharpSyntaxVisitor
    {
        public readonly List<string> Classes = new List<string>();

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
           Classes.Add(node.ToString());
        }

        public void Clear()
        {
            Classes.Clear();
        }
    }
}
