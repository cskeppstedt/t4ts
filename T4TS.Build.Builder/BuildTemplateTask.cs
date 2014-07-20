using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Collections.Generic;
using System.IO;
using T4TS.Build.Builder.Properties;

namespace T4TS.Build.Builder
{
    public class BuildTemplateTask : Task
    {
        /// <summary>
        /// The source files to merge and build the T4 template from.
        /// </summary>
        [Required]
        public ITaskItem[] SourceFiles { get; set; }
        
        /// <summary>
        /// The output dir to save T4TS.tt (and settings file) to. Defaults to "{T4TSBasePath}\build".
        /// </summary>
        public ITaskItem OutputDir { get; set; }

        /// <summary>
        /// The base directory of the T4TS project. Should be "..\" relatively from this project.
        /// Defaults to "..\".
        /// </summary>
        public ITaskItem T4TSBasePath { get; set; }
        
        /// <summary>
        /// This will be set to the full path of the generated T4TS.tt when the Task has completed.
        /// </summary>
        [Output]
        public ITaskItem TemplateOutputFile { get; set; }

        public override bool Execute()
        {
            var basePath = GetBasePath();

            var sourceFileInfos = new List<FileInfo>();
            foreach (var sourceFile in SourceFiles)
            {
                string path = Path.Combine(basePath.FullName, "T4TS", sourceFile.ItemSpec);
                sourceFileInfos.Add(new FileInfo(path));
            }

            this.Log.LogMessage("Building template from {0} files:", sourceFileInfos.Count);
            foreach (var fi in sourceFileInfos)
                this.Log.LogMessage("  - " + fi.FullName);

            var t4tsContent = TemplateBuilder.BuildT4TSFromSourceFiles(sourceFileInfos);
            
            var outputDir = GetOutputDir();
            
            var t4tsFile = new FileInfo(Path.Combine(outputDir.FullName, "T4TS.tt"));
            File.WriteAllText(t4tsFile.FullName, t4tsContent);
            LogFileWrite(t4tsFile);

            var templateSettingsFile = new FileInfo(Path.Combine(outputDir.FullName, "T4TS.tt.settings.t4"));
            File.WriteAllText(templateSettingsFile.FullName, Resources.TemplateSettings);
            LogFileWrite(templateSettingsFile);

            TemplateOutputFile = new TaskItem(t4tsFile.FullName);
            return true;
        }

        void LogFileWrite(FileInfo fileInfo)
        {
            this.Log.LogMessage("{0} written to file ({1} bytes): {2}", fileInfo.Name, fileInfo.Length, fileInfo.FullName);
        }

        DirectoryInfo GetBasePath()
        {
            if (T4TSBasePath == null)
                return new DirectoryInfo(@"..\");
            
            return new DirectoryInfo(T4TSBasePath.ItemSpec);
        }

        DirectoryInfo GetOutputDir()
        {
            if (OutputDir == null)
                return new DirectoryInfo(Path.Combine(GetBasePath().FullName, "build"));
            
            return new DirectoryInfo(OutputDir.ItemSpec);
        }
    }
}
