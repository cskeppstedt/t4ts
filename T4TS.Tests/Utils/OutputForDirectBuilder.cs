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
        public Settings OutputSettings { get; private set; }
        public DirectSettings DirectSettings { get; private set; }
        public CodeTraverser.TraverserSettings TraverserSettings { get; private set; }
        public TypeContext TypeContext { get; private set; }

        public OutputForDirectBuilder(params Type[] types)
        {
            this.Types = new ReadOnlyCollection<Type>(types);
            this.OutputSettings = new Settings();
            this.DirectSettings = new DirectSettings();
            this.TraverserSettings = new CodeTraverser.TraverserSettings()
            {
                ClassToInterfaceBuilder = new DirectInterfaceBuilder(this.DirectSettings),
                InterfaceToInterfaceBuilder = new DirectInterfaceBuilder(this.DirectSettings),
                EnumBuilder = new DirectEnumBuilder(this.DirectSettings)
            };
            this.TypeContext = new TypeContext();
        }

        public OutputForDirectBuilder WithSettings(
            Action<DirectSettings> changeSettings)
        {
            changeSettings(this.DirectSettings);
            return this;
        }

        public OutputForDirectBuilder WithTraverserSettings(
            Action<CodeTraverser.TraverserSettings> changeSettings)
        {
            changeSettings(this.TraverserSettings);
            return this;
        }

        public OutputForDirectBuilder Edit(Action<OutputForDirectBuilder> applyEdits)
        {
            applyEdits(this);
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
            var generator = new CodeTraverser(
                solution,
                this.TypeContext)
            {
                Settings = this.TraverserSettings
            };
            var data = generator.GetAllInterfaces().ToList();

            return OutputFormatter.GetOutput(
                data,
                this.OutputSettings,
                this.TypeContext);
        }

        static string Normalize(string output)
        {
            return Regex.Replace(output, @"\r\n|\n\r|\n|\r", "\n").Trim();
        }
    }
}
