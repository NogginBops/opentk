using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using ALGenerator.Parsing;
using System.CodeDom.Compiler;
using GeneratorBase.Utility;
using GeneratorBase;
using GeneratorBase.Overloading;

namespace ALGenerator.Process
{
    internal record OutputData(
        List<ApiPointers> Pointers,
        List<OutputApiData> Namespaces,
        Dictionary<string, EnumMemberDocumentation> EnumMemberDocumentation);
}
