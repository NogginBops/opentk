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
        List<Pointers> Pointers,
        List<Namespace> Namespaces);

    // FIXME: Maybe change to API.. something? "namespace" is quite generic.
    internal record Namespace(
        OutputApi Name,
        List<VendorFunctions> VendorFunctions,
        List<EnumType> EnumGroups,
        Dictionary<Function, FunctionDocumentation> Documentation);

    internal record Pointers(
        APIFile File,
        List<Function> NativeFunctions);

    internal record FunctionDocumentation(
        string Name,
        string Purpose,
        ParameterDocumentation[] Parameters,
        List<string> RefPagesLinks,
        List<string> AddedIn,
        List<string>? RemovedIn
        );

    internal record VendorFunctions(
        string Vendor,
        List<OverloadedFunction> Functions,
        HashSet<Function> NativeFunctionsWithPostfix);

    internal enum OutputApi
    {
        Invalid,
        GL,
        GLCompat,
        GLES1,
        GLES2,
        WGL,
        GLX,
        EGL,
    }

    [Flags]
    internal enum OutputApiFlags
    {
        None = 0,
        GL = 1 << 0,
        GLCompat = 1 << 1,
        GLES1 = 1 << 2,
        GLES2 = 1 << 3,
        WGL = 1 << 4,
        GLX = 1 << 5,
        EGL = 1 << 6,
    }
}
