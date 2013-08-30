using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public abstract class OutputAppender<TSegment> where TSegment: class
    {
        protected StringBuilder Output { get; private set; }
        protected int BaseIndentation { get; private set; }
        protected Settings Settings { get; private set; }

        public OutputAppender(StringBuilder output, int baseIndentation, Settings settings)
        {
            if (output == null)
                throw new ArgumentNullException("output");
            
            if (settings == null)
                throw new ArgumentNullException("settings");

            this.Output = output;
            this.BaseIndentation = baseIndentation;
            this.Settings = settings;
        }

        public abstract void AppendOutput(TSegment segment);

        protected void AppendIndented(string text)
        {
            AppendIndendation();
            Output.Append(text);
        }

        protected void AppendIndentedLine(string line)
        {
            AppendIndendation();
            Output.AppendLine(line);
        }

        protected void AppendIndendation()
        {
            Output.Append(' ', BaseIndentation);
        }

        public override string ToString()
        {
            return Output.ToString();
        }
    }
}
