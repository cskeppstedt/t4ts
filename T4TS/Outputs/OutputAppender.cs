using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Outputs
{
    public abstract partial class OutputAppender<TSegment> where TSegment: class
    {
        protected OutputSettings Settings { get; private set; }
        protected TypeContext TypeContext { get; private set; }

        public OutputAppender(
            OutputSettings settings,
            TypeContext typeContext)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            
            this.Settings = settings;
            this.TypeContext = typeContext;
        }

        public abstract void AppendOutput(
            StringBuilder output,
            int baseIndentation,
            TSegment segment);

        protected void AppendIndented(
            StringBuilder output,
            int indentation, 
            string text)
        {
            this.AppendIndendation(
                output,
                indentation);
            output.Append(text);
        }

        protected void AppendIndentedLine(
            StringBuilder output,
            int indentation,
            string line)
        {
            this.AppendIndendation(
                output,
                indentation);
            output.AppendLine(line);
        }

        protected void AppendIndendation(
            StringBuilder output,
            int indentation)
        {
            output.Append(' ', indentation);
        }
    }
}
