using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS
{
    public partial class TypeName
    {
        public string RawName { get; private set; }

        public string UnqualifiedName { get; private set; }

        public IList<TypeName> TypeArguments { get; private set; }

        public string UniversalName
        {
            get
            {
                string result;
                if (this.TypeArguments.Count == 0)
                {
                    result = this.UnqualifiedName;
                }
                else
                {
                    result = String.Format(
                        "{0}`{1}",
                        this.UnqualifiedName,
                        this.TypeArguments.Count);
                }
                return result;
            }
        }

        public string QualifiedName
        {
            get
            {
                string result;
                if (this.TypeArguments.Count == 0)
                {
                    result = this.UnqualifiedName;
                }
                else
                {
                    result = String.Format(
                        "{0}<{1}>",
                        this.UnqualifiedName,
                        String.Join(
                            ",",
                            this.TypeArguments.Select(
                                (typeArgument) => typeArgument.QualifiedName)));
                }
                return result;
            }
        }

        public string Namespace
        {
            get
            {
                string result;
                int namespaceEndIndex = this.UniversalName.LastIndexOf('.');
                if (namespaceEndIndex < 0)
                {
                    result = String.Empty;
                }
                else
                {
                    result = this.UniversalName.Substring(
                        0,
                        namespaceEndIndex);
                }
                return result;
            }
        }

        public string UnqualifiedTypeName
        {
            get
            {
                string result;
                int typeStartIndex = this.UnqualifiedName.LastIndexOf('.');
                if (typeStartIndex < 0)
                {
                    result = this.UnqualifiedName;
                }
                else
                {
                    result = this.UnqualifiedName.Substring(
                        typeStartIndex + 1);
                }
                return result;
            }
        }

        public string QualifiedTypeName
        {
            get
            {
                string result;
                int typeStartIndex = this.QualifiedName.LastIndexOf('.');
                if (typeStartIndex < 0)
                {
                    result = this.QualifiedName;
                }
                else
                {
                    result = this.QualifiedName.Substring(
                        typeStartIndex + 1);
                }
                return result;
            }
        }

        protected TypeName(
            string rawName,
            string unqualifiedName,
            IList<TypeName> typeArguments)
        {
            this.RawName = rawName;
            this.UnqualifiedName = unqualifiedName;
            this.TypeArguments = typeArguments;
        }

        public static TypeName ParseDte(string rawName)
        {
            return DteParser.Parse(rawName);
        }

        public TypeName ReplaceUnqualifiedName(string newUnqualifiedName)
        {
            return new TypeName(
                this.RawName.Replace(
                    this.UnqualifiedName,
                    newUnqualifiedName),
                newUnqualifiedName,
                new List<TypeName>(this.TypeArguments));
        }

        public TypeName ReplaceTypeArguments(
            IEnumerable<string> typeArgumentNames)
        {
            IList<TypeName> typeArguments = typeArgumentNames
                .Select((typeArgumentName) => TypeName.ParseDte(typeArgumentName))
                .ToList();
            if (typeArguments.Count != this.TypeArguments.Count)
            {
                throw new InvalidOperationException();
            }

            return new TypeName(
                this.RawName,
                this.UnqualifiedName,
                typeArguments);
        }
    }
}
