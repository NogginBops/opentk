using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using GLGenerator.Parsing;
using System.CodeDom.Compiler;
using GeneratorBase.Utility;
using GeneratorBase;
using GeneratorBase.Overloading;

namespace GLGenerator.Process
{
    internal record OutputData(
        List<ApiPointers> Pointers,
        List<OutputApiData> Namespaces);
}
