using System;
using System.Collections;
using System.Collections.Generic;
using EnvDTE;

namespace T4TS.Tests
{
    class BaseList<TItem> : List<TItem>
    {
        public TItem Item(object index)
        {
            return this[(int)index];
        }

        public new IEnumerator GetEnumerator()
        {
            return base.GetEnumerator();
        }

        public object Parent { get { throw new NotImplementedException(); } }
        public DTE DTE { get { throw new NotImplementedException(); } }
        public string Kind { get { throw new NotImplementedException(); } }

        public void Reserved1(object element)
        {
            throw new NotImplementedException();
        }

        public bool CreateUniqueID(string prefix, ref string newName)
        {
            throw new NotImplementedException();
        }
    }

    class CodeElemens<TItem> : BaseList<TItem>, CodeElements
    {
        public new CodeElement Item(object index)
        {
            throw new NotImplementedException();
        }
    }
}