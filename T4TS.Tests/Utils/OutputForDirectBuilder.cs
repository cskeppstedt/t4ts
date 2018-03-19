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
        public DirectInterfaceBuilder.Settings Settings { get; private set; }

        public OutputForDirectBuilder(params Type[] types)
        {
            this.Types = new ReadOnlyCollection<Type>(types);
            this.Settings = new DirectInterfaceBuilder.Settings();
        }

        public OutputForDirectBuilder With(DirectInterfaceBuilder.Settings settings)
        {
            this.Settings = settings;
            return this;
        }

        public void ToEqual(string expectedOutput)
        {
            var generatedOutput = GenerateOutput();
            Assert.AreEqual(Normalize(expectedOutput), Normalize(generatedOutput));
        }

        private string GenerateOutput()
        {
            var solution = DTETransformer.BuildDteSolution(this.Types.ToArray());
            var attributeBuilder = new DirectInterfaceBuilder(this.Settings);
            var typeContext = new TypeContext(useNativeDates: false);
            var generator = new CodeTraverser(
                solution,
                typeContext,
                attributeBuilder);
            var data = generator.GetAllInterfaces().ToList();

            return OutputFormatter.GetOutput(data, new Settings());
        }

        static string Normalize(string output)
        {
            return Regex.Replace(output, @"\r\n|\n\r|\n|\r", "\n").Trim();
        }
    }
}
