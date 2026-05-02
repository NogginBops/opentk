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
        /* FIXME: Maybe do like this?
        Namespace GL,
        Namespace GLCompat,
        Namespace GLES1,
        Namespace GLES2,
        Namespace WGL,
        Namespace GLX,
        */

        List<ApiPointers> Pointers,
        List<Namespace> Namespaces,
        Dictionary<string, EnumMemberDocumentation> EnumMemberDocumentation);

    // FIXME: Maybe change to API.. something? "namespace" is quite generic.
    internal record Namespace(
        OutputApi Name,
        List<VendorFunctions> VendorFunctions,
        List<EnumType> EnumGroups,
        Dictionary<Function, FunctionDocumentation> FunctionDocumentation);

    internal record FunctionDocumentation(
        string Name,
        string Purpose,
        ParameterDocumentation[] Parameters,
        List<Link> RefPagesLinks,
        List<string> AddedIn,
        List<string>? RemovedIn
        );

    internal enum OutputApi
    {
        Invalid,
        AL,
        ALC,
    }

    [Flags]
    internal enum OutputApiFlags
    {
        None = 0,
        AL = 1 << 0,
        ALC = 1 << 1,
    }
}
