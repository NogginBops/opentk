using ALGenerator.Process;
using GeneratorBase;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.IO.Compression;
using System.Linq;
using System.Numerics;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace ALGenerator.Parsing
{
    internal enum InputAPI
    {
        AL,
        ALC,
    }

    [Flags]
    internal enum EnumAPI
    {
        None = 0,
        AL = 1 << 0,
        ALC = 1 << 1,
    }

    internal record Specification(
        List<Function> Functions,
        List<EnumEntry> Enums,
        List<API> APIs);

    internal record API(
        InputAPI Name,
        List<FunctionReference> Functions,
        List<EnumReference> Enums);

    internal record FunctionReference(string EntryPoint, VersionInfo VersionInfo);

    internal record EnumReference(
        string EnumName,
        VersionInfo VersionInfo,
        // Is this enum reference copied from another namespace.
        bool IsCrossReferenced);

    internal record APIVersion(
        Version Name,
        List<string> EntryPoints,
        List<string> EnumValues);

    internal record EFXPreset(
        string Name,
        float Density,
        float Diffusion,
        float Gain,
        float GainHF,
        float GainLF,
        float DecayTime,
        float DecayHFRatio,
        float DecayLFRatio,
        float ReflectionsGain,
        float RelfectionsDelay,
        Vector3 ReflectionsPan,
        float LateReverbGain,
        float LateReverbDelay,
        Vector3 LateReverbPan,
        float EchoTime,
        float EchoDepth,
        float ModulationTime,
        float ModulationDepth,
        float AirAbsorptionGainHF,
        float HFReference,
        float LFReference,
        float RoomRolloffFactor,
        bool DecayHFLimit);

    internal enum HandleType
    {
        // FIXME:
        Any,
        Source,
        Buffer,
        Effect,
        EffectSlot,
        Filter,
    }
}
