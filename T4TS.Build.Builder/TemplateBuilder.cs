using System.Collections.Generic;
using System.IO;
using T4TS.Build.Builder.Properties;

namespace T4TS.Build.Builder
{
    class TemplateBuilder
    {
        /// <summary>
        /// Combines the template prefix, the class definitions, and the template suffix into
        /// the complete contents of T4TS.tt.
        /// </summary>
        public static string BuildT4TSFromSourceFiles(IEnumerable<FileInfo> fromFiles)
        {
            var merger = new SourceFileMerger();
            string combinedSource = merger.MergeSourceFileClasses(fromFiles);
            string template = Resources.TemplatePrefix + combinedSource + Resources.TemplateSuffix;
            
            return template;
        }
    }
}
