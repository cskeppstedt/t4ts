using System;
using System.Collections;
using System.Collections.Generic;
using EnvDTE;

namespace T4TS.Tests.Mocks
{
    internal class BaseList<TItem> : List<TItem>
    {
        public object Parent
        {
            get { throw new NotImplementedException(); }
        }

        public DTE DTE
        {
            get { throw new NotImplementedException(); }
        }

        public string Kind
        {
            get { throw new NotImplementedException(); }
        }

        public TItem Item(object index)
        {
            return this[(int) index];
        }

        public new IEnumerator GetEnumerator()
        {
            return base.GetEnumerator();
        }

        public void Reserved1(object element)
        {
            throw new NotImplementedException();
        }

        public bool CreateUniqueID(string prefix, ref string newName)
        {
            throw new NotImplementedException();
        }
    }

    internal class CodeElemens<TItem> : BaseList<TItem>, CodeElements
    {
        public new CodeElement Item(object index)
        {
            return (CodeElement)this[(int)index - 1];
        }
    }
}