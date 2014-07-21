using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace T4TS.Build.Builder
{
    class SourceFileMerger
    {
        static readonly SourceFileVisitor Visitor = new SourceFileVisitor();

        /// <summary>
        /// Merges all the source files (they are assumed to be C# files).
        /// </summary>
        /// <returns>A string with all C# classes found in the source files.</returns>
        public string MergeSourceFileClasses(IEnumerable<FileInfo> files)
        {
            Visitor.Clear();

            foreach (var file in files)
                VisitFile(file);
            
            var sb = new StringBuilder();
            sb.AppendLine();
            
            foreach (string classDefinition in Visitor.Classes)
            {
                // Indentation is important!
                sb.Append("    ");
                sb.AppendLine(classDefinition);
                sb.AppendLine();
            }

            return sb.ToString();
        }

        void VisitFile(FileInfo file)
        {
            using (var sr = new StreamReader(file.OpenRead()))
            {
                var syntaxTree = CSharpSyntaxTree.ParseText(sr.ReadToEnd());
                var root = (CompilationUnitSyntax)syntaxTree.GetRoot();

                foreach (var child in root.ChildNodes())
                    VisitNode(child as CSharpSyntaxNode);
            }
        }

        void VisitNode(CSharpSyntaxNode node)
        {
            if (node == null)
                return;

            node.Accept(Visitor);

            var children = node.ChildNodes();
            if (children != null)
            {
                foreach (var child in children)
                    VisitNode(child as CSharpSyntaxNode);
            }
        }
    }
}
