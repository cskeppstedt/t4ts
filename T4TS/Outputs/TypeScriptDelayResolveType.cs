using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public class TypeScriptDelayResolveType : TypeScriptType
    {
        private TypeContext typeContext;
        private string fullName;

        private string name;
        private TypeScriptModule module;

        public TypeScriptDelayResolveType(
            TypeContext typeContext,
            string fullName)
        {
            this.typeContext = typeContext;
            this.fullName = fullName;
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
        }

        private void EnsureResolved()
        {
            if (this.name == null)
            {
                TypeScriptType output = this.typeContext.GetOutput(this.fullName);
                if (output != null)
                {
                    this.name = output.Name;
                    this.module = output.Module;
                }
                else
                {
                    this.name = this.typeContext.GetTypeScriptType(this.fullName).ToString();
                }
            }
        }
    }
}
