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
        private TypeName sourceType;

        private TypeName resolvedType;
        private string resolvedName;
        private TypeScriptModule resolvedModule;

        public TypeScriptDelayResolveType()
        {
        }

        public TypeContext TypeContext
        {
            get { return this.typeContext; }
            set
            {
                if (this.typeContext != value)
                {
                    this.typeContext = value;
                    this.resolvedType = null;
                    this.resolvedModule = null;
                }
            }
        }

        public TypeName SourceType
        {
            get { return this.sourceType; }
            set
            {
                if (this.sourceType != value)
                {
                    this.sourceType = value;
                    this.resolvedType = null;
                    this.resolvedModule = null;
                }
            }
        }
        
        public string Name
        {
            get
            {
                this.EnsureResolved();
                return (this.resolvedType != null)
                    ? this.resolvedType.QualifiedTypeName
                    : this.resolvedName;
            }
        }

        public TypeScriptModule Module
        {
            get
            {
                this.EnsureResolved();
                return this.resolvedModule;
            }
        }

        public string FullName
        {
            get { return this.SourceType.RawName; }
            set { throw new NotSupportedException(); }
        }

        private void EnsureResolved()
        {
            if (this.resolvedType == null)
            {
                TypeScriptOutputType resolvedOutputType = this.typeContext.GetOutput(this.sourceType);
                if (resolvedOutputType != null)
                {
                    this.resolvedType = this.SourceType
                        .ReplaceUnqualifiedName(resolvedOutputType.Name)
                        .ReplaceTypeArguments(this.SourceType.TypeArguments
                            .Select((typeArgument) => 
                                this.typeContext.GetSystemOutputType(typeArgument)
                                .Name));
                    this.resolvedModule = resolvedOutputType.Module;
                }
                else
                {
                    resolvedOutputType = this.typeContext.GetSystemOutputType(this.SourceType);
                    if (resolvedOutputType != null)
                    {
                        this.resolvedName = resolvedOutputType.Name;
                    }
                    else
                    {
                        throw new Exception("Failed to resolve type " + this.SourceType.QualifiedName);
                    }
                }
            }
        }
    }
}
