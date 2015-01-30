using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace T4TS.Tests.Utils
{
    class OutputFor
    {
        readonly IReadOnlyCollection<Type> Types;
        public Settings Settings { get; private set; }

        public OutputFor(params Type[] types)
        {
            this.Types = new ReadOnlyCollection<Type>(types);
        }

        public OutputFor With(Settings settings)
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
            var settings = new Settings();
            var generator = new CodeTraverser(solution, settings);
            var data = generator.GetAllInterfaces().ToList();

            return OutputFormatter.GetOutput(data, settings);
        }

        static string Normalize(string output)
        {
            return Regex.Replace(output, @"\r\n|\n\r|\n|\r", "\n").Trim();
        }
    }
}
