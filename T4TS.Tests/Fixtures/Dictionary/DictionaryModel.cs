using System.Collections.Generic;
using T4TS.Tests.Fixtures.Basic;

namespace T4TS.Tests.Fixtures.Dictionary
{
    [TypeScriptInterface]
    public class DictionaryModel : List<BasicModel>
    {
        public Dictionary<int, BasicModel> IntKey { get; set; }
        public IDictionary<string, BasicModel> StringKey { get; set; }
    }
}
