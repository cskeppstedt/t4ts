using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using T4TS.Builders;
using T4TS.Outputs;

namespace T4TS.Tests.Utils
{
    class OutputForAttributeBuilder
    {
        readonly IReadOnlyCollection<Type> Types;

        public Settings Settings { get; private set; }

        public OutputSettings OutputSettings { get; private set; }

        public TypeContext.Settings TypeSettings { get; private set; }


        public OutputForAttributeBuilder(params Type[] types)
        {
            this.Types = new ReadOnlyCollection<Type>(types);
            this.Settings = new Settings();
            this.OutputSettings = new OutputSettings();
            this.TypeSettings = new TypeContext.Settings();
        }

        public OutputForAttributeBuilder With(Settings settings)
        {
            this.Settings = settings;
            return this;
        }

        public OutputForAttributeBuilder WithTypeSettings(TypeContext.Settings typeSettings)
        {
            this.TypeSettings = typeSettings;
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
            var attributeBuilder = new AttributeInterfaceBuilder(this.Settings);
            var typeContext = new TypeContext(this.TypeSettings);
            var generator = new CodeTraverser(
                solution,
                typeContext)
            {
                Settings =
                {
                    ClassToInterfaceBuilder = attributeBuilder
                }
            };
            var data = generator.GetAllInterfaces().ToList();

            return OutputFormatter.GetOutput(
                data,
                this.OutputSettings,
                typeContext);
        }

        static string Normalize(string output)
        {
            return Regex.Replace(output, @"\r\n|\n\r|\n|\r", "\n").Trim();
        }
    }
}
