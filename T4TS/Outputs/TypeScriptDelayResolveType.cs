using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public class TypeScriptDelayResolveType : TypeScriptOutputType
    {
        private TypeContext typeContext;
        private bool resolveOutputOnly;
        private string fullName;

        private string name;
        private TypeScriptModule module;

        public TypeScriptDelayResolveType(
            TypeContext typeContext,
            bool resolveOutputOnly)
        {
            this.typeContext = typeContext;
            this.resolveOutputOnly = resolveOutputOnly;
        }
        
        public string Name
        {
            get
            {
                this.EnsureResolved();
                return this.name;
            }
        }

        public TypeScriptModule Module
        {
            get
            {
                this.EnsureResolved();
                return this.module;
            }
        }

        public string FullName
        {
            get { return this.fullName; }
            set
            {
                if (this.fullName != value)
                {
                    this.fullName = value;
                    this.name = null;
                    this.module = null;
                }
            }
        }

        private void EnsureResolved()
        {
            if (this.name == null)
            {
                TypeScriptOutputType output = this.typeContext.GetOutput(this.fullName);
                if (output != null)
                {
                    this.name = output.Name;
                    this.module = output.Module;
                }
                else if (!this.resolveOutputOnly)
                {
                    this.name = this.typeContext.GetTypeScriptType(this.fullName).ToString();
                }
            }
        }
    }
}
