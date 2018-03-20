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

        public OutputForDirectBuilder(params Type[] types)
        {
            this.Types = new ReadOnlyCollection<Type>(types);
            this.Settings = new DirectSettings();
        }

        public OutputForDirectBuilder With(DirectSettings settings)
        {
            this.Settings = settings;
            return this;
        }

        public void ToEqual(string expectedOutput)
        {
            var generatedOutput = GenerateOutput();

            string assertMessage = null;
            for (int index = 0; index < expectedOutput.Length; index++)
            {
                if (generatedOutput.Length <= index)
                {
                    assertMessage = "Reached the end of the actual string while comparing with the expected";
                    break;
                }
                else if (expectedOutput[index] != generatedOutput[index])
                {
                    assertMessage = "Actual string differs from expected starting at index "
                        + index.ToString()
                        + " expected segment: \""
                        + expectedOutput
                            .Substring(index)
                            .Substring(0,
                                Math.Min(
                                    10,
                                    (expectedOutput.Length - index) - 1))
                        + "\" actual segment: \""
                        + generatedOutput
                            .Substring(index)
                            .Substring(0,
                                Math.Min(
                                    10,
                                    (generatedOutput.Length - index) - 1))
                        + "\"";
                    break;
                }
            }
            Assert.IsNull(
                assertMessage,
                assertMessage
                    + "\r\nExpected: "
                    + expectedOutput
                    + "\r\nActual: "
                    + generatedOutput);
        }

        private string GenerateOutput()
        {
            var solution = DTETransformer.BuildDteSolution(this.Types.ToArray());
            var attributeBuilder = new DirectInterfaceBuilder(this.Settings);
            var typeContext = new TypeContext(useNativeDates: false);
            var generator = new CodeTraverser(
                solution,
                typeContext)
            {
                InterfaceBuilder = attributeBuilder,
                EnumBuilder = new DirectEnumBuilder(this.Settings)
            };
            var data = generator.GetAllInterfaces().ToList();

            return OutputFormatter.GetOutput(data, new Settings());
        }

        static string Normalize(string output)
        {
            return Regex.Replace(output, @"\r\n|\n\r|\n|\r", "\n").Trim();
        }
    }
}
