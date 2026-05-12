using GeneratorBase;
using GLGenerator.Process;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.IO.Compression;
using System.Linq;

namespace GLGenerator.Parsing
{
    internal record SpecificationFile(
        ApiFile File,
        List<Function> Functions,
        List<EnumEntry> Enums,
        List<Feature> Features,
        List<Extension> Extensions);

    internal record API(
        OutputApi Name,
        List<FunctionReference> Functions,
        List<EnumReference> Enums);

    internal record ResolvedApi(OutputApi Api, List<Function> Functions, List<EnumMember> Enums);

    internal record FunctionReference(
        string EntryPoint,
        VersionInfo VersionInfo,
        GLProfile Profile);

    internal record EnumReference(
        string EnumName,
        VersionInfo VersionInfo,
        // FIXME! there can be multiple profiles??
        GLProfile Profile,
        // Is this enum reference copied from another namespace.
        bool IsCrossReferenced);

    internal record ConstantReference(
        string ConstantName,
        VersionInfo VersionInfo,
        GLProfile Profile);

    internal record APIVersion(
        Version Name,
        List<string> EntryPoints,
        List<string> EnumValues);

    internal enum HandleType
    {
        AnyHandle,
        ProgramHandle,
        ProgramPipelineHandle,
        TextureHandle,
        BufferHandle,
        ShaderHandle,
        QueryHandle,
        FramebufferHandle,
        RenderbufferHandle,
        SamplerHandle,
        TransformFeedbackHandle,
        VertexArrayHandle,
        DisplayListHandle,
        PerfQueryHandle,
    }
}
