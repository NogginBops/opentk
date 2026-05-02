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
        List<Namespace> Namespaces);

    // FIXME: Maybe change to API.. something? "namespace" is quite generic.
    internal record Namespace(
        OutputApi Name,
        List<VendorFunctions> VendorFunctions,
        List<EnumType> EnumGroups,
        Dictionary<Function, FunctionDocumentation> Documentation);
}
