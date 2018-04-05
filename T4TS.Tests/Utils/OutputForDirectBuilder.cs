using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using T4TS.Builders;

namespace T4TS.Tests.Utils
{
    class OutputForDirectBuilder
    {
        readonly IReadOnlyCollection<Type> Types;
        public DirectSettings Settings { get; private set; }
        public CodeTraverser.TraverserSettings TraverserSettings { get; private set; }

        public OutputForDirectBuilder(params Type[] types)
        {
            this.Types = new ReadOnlyCollection<Type>(types);
            this.Settings = new DirectSettings();
            this.TraverserSettings = new CodeTraverser.TraverserSettings()
            {
                InterfaceBuilder = new DirectInterfaceBuilder(this.Settings),
                EnumBuilder = new DirectEnumBuilder(this.Settings)
            };
        }

        public OutputForDirectBuilder WithSettings(
            Action<DirectSettings> changeSettings)
        {
            changeSettings(this.Settings);
            return this;
        }

        public OutputForDirectBuilder WithTraverserSettings(
            Action<CodeTraverser.TraverserSettings> changeSettings)
        {
            changeSettings(this.TraverserSettings);
            return this;
        }

        public void ToEqual(string expectedOutput)
        {
            var generatedOutput = GenerateOutput();
            StringCompare.AssertAreEqual(
                expectedOutput,
                generatedOutput);
        }

        private string GenerateOutput()
        {
            var solution = DTETransformer.BuildDteSolution(this.Types.ToArray());
            var typeContext = new TypeContext();
            var generator = new CodeTraverser(
                solution,
                typeContext)
            {
                Settings = this.TraverserSettings
            };
            var data = generator.GetAllInterfaces().ToList();

            return OutputFormatter.GetOutput(
                data,
                new Settings(),
                typeContext);
        }

        static string Normalize(string output)
        {
            return Regex.Replace(output, @"\r\n|\n\r|\n|\r", "\n").Trim();
        }
    }
}
