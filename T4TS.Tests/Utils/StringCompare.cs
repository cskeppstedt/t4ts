using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace T4TS.Tests.Utils
{
    public class StringCompare
    {
        public static void AssertAreEqual(string expected, string actual)
        {
            string assertMessage = null;
            if (expected.Length == 0
                && actual.Length != 0)
            {
                assertMessage = "Expected empty string";
            }
            for (int index = 0; index < expected.Length; index++)
            {
                if (actual.Length <= index)
                {
                    assertMessage = "Reached the end of the actual string while comparing with the expected";
                    break;
                }
                else if (expected[index] != actual[index])
                {
                    assertMessage = "Actual string differs from expected starting at index "
                        + index.ToString()
                        + " expected segment: \""
                        + expected
                            .Substring(index)
                            .Substring(0,
                                Math.Min(
                                    10,
                                    (expected.Length - index) - 1))
                        + "\" actual segment: \""
                        + actual
                            .Substring(index)
                            .Substring(0,
                                Math.Min(
                                    10,
                                    (actual.Length - index) - 1))
                        + "\"";
                    break;
                }
            }
            Assert.IsNull(
                assertMessage,
                assertMessage
                    + "\r\nExpected: "
                    + expected
                    + "\r\nActual: "
                    + actual);
        }
    }
}
