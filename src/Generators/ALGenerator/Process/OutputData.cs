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
        List<Namespace> Namespaces,
        Dictionary<string, EnumMemberDocumentation> EnumMemberDocumentation);

    // FIXME: Maybe change to API.. something? "namespace" is quite generic.
    internal record Namespace(
        OutputApi Name,
        List<VendorFunctions> VendorFunctions,
        List<EnumType> EnumGroups,
        Dictionary<Function, FunctionDocumentation> FunctionDocumentation);
}
