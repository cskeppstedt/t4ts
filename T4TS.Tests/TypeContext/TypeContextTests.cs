using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace T4TS.Tests
{
    [TestClass]
    public class TypeContextTests
    {
        //[TestMethod]
        //public void ShouldSupportDatetimesAsNativeDates()
        //{
        //    var context = new TypeContext(useNativeDates: true);

        //    var resolvedType = context.GetTypeScriptType(typeof(DateTime).FullName);
        //    Assert.IsInstanceOfType(resolvedType, typeof(DateTimeType));
        //}

        //[TestMethod]
        //public void ShouldSupportDatetimesAsStrings()
        //{
        //    var context = new TypeContext(useNativeDates: false);

        //    var resolvedType = context.GetTypeScriptType(typeof(DateTime).FullName);
        //    Assert.IsInstanceOfType(resolvedType, typeof(StringType));
        //}

        //[TestMethod]
        //public void ShouldSupportDatetimeOffsetAsNativeDates()
        //{
        //    var context = new TypeContext(useNativeDates: true);

        //    var resolvedType = context.GetTypeScriptType(typeof(DateTimeOffset).FullName);
        //    Assert.IsInstanceOfType(resolvedType, typeof(DateTimeType));
        //}

        //[TestMethod]
        //public void ShouldSupportDatetimeOffsetAsStrings()
        //{
        //    var context = new TypeContext(useNativeDates: false);

        //    var resolvedType = context.GetTypeScriptType(typeof(DateTimeOffset).FullName);
        //    Assert.IsInstanceOfType(resolvedType, typeof(StringType));
        //}

        //[TestMethod]
        //public void ShouldSupportNumberTypes()
        //{
        //    var inputTypes = new [] {
        //        typeof(byte),
        //        typeof(sbyte),
        //        typeof(short),
        //        typeof(ushort),
        //        typeof(int),
        //        typeof(uint),
        //        typeof(long),
        //        typeof(ulong),
        //        typeof(float),
        //        typeof(decimal),
        //        typeof(double)
        //    };

        //    var context = new TypeContext(useNativeDates: false);
        //    var expectedType = typeof(NumberType);

        //    foreach (var type in inputTypes)
        //    {
        //        var resolvedType = context.GetTypeScriptType(type.FullName);
        //        Assert.IsInstanceOfType(resolvedType, expectedType);
        //    }
        //}

        //[TestMethod]
        //public void ShouldSupportIEnumerableTypes()
        //{
        //    var context = new TypeContext(useNativeDates: false);

        //    var resolvedType = context.GetTypeScriptType(typeof(IEnumerable<String>).FullName);
        //    Assert.IsInstanceOfType(resolvedType, typeof(ArrayType));

        //    resolvedType = context.GetTypeScriptType("System.Collections.Generic.IEnumerable<System.String>");
        //    Assert.IsInstanceOfType(resolvedType, typeof(ArrayType));
        //}

        //[TestMethod]
        //public void ShouldSupportDicionaryTypes()
        //{
        //    var context = new TypeContext(useNativeDates: false);

        //    var resolvedType = context.GetTypeScriptType(typeof(Dictionary<String, float>).FullName);
        //    Assert.IsInstanceOfType(resolvedType, typeof(DictionaryType));

        //    resolvedType = context.GetTypeScriptType("System.Collections.Generic.Dictionary<System.String, System.Float>");
        //    Assert.IsInstanceOfType(resolvedType, typeof(DictionaryType));
        //}
    }
}
