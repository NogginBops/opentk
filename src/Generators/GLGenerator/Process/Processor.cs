using GeneratorBase;
using GeneratorBase.Overloading;
using GeneratorBase.Utility;
using GeneratorBase.Utility.Extensions;
using GLGenerator.Parsing;
using GLGenerator.Process;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization.DataContracts;

namespace GLGenerator.Process
{
    internal static class Processor
    {
        internal record FunctionData(
            Function NativeFunction,
            Dictionary<OutputApi, FunctionDocumentation> Documentation,
            Overload[] Overloads,
            bool ChangeNativeName);

        internal sealed record EnumGroupInfo(
            string OriginalName,
            string GroupName,
            bool IsFlags)
        {
            // To deduplicate these correctly we need special logic for the IsFlags bool
            // so we don't consider it in the equality check and hashcode to allow for that.
            //
            // Example:
            // PathFontStyle uses GL_NONE which is not marked as bitmask
            // but other entries such as GL_BOLD_BIT_NV is marked as bitmask.
            //
            // When this case happens we want to consider the entire groupName as a bitmask.
            //
            // In the current spec this case only happens for PathFontStyle.
            // - 2021-07-04
            public bool Equals(EnumGroupInfo? other) =>
                other?.GroupName == GroupName;

            public override int GetHashCode() =>
                HashCode.Combine(GroupName);
        };

        internal static OutputData ProcessSpec(Specification spec, Documentation docs)
        {
            // The first thing we do is process all of the vendorFunctions defined into a dictionary of Functions.
            List<Function> allEntryPoints = new List<Function>(spec.Functions.Count);
            Dictionary<string, FunctionData> allFunctions = new Dictionary<string, FunctionData>(spec.Functions.Count);
            foreach (Function function in spec.Functions)
            {
                Dictionary<OutputApi, FunctionDocumentation> functionDocumentation = MakeDocumentationForNativeFunction(function, docs);
                FunctionData overloadedFunction = GenerateOverloads(function, functionDocumentation);

                allEntryPoints.Add(function);
                allFunctions.Add(function.EntryPoint, overloadedFunction);
            }

            Dictionary<OutputApi, Dictionary<string, EnumMember>> allEnumsPerAPI = new Dictionary<OutputApi, Dictionary<string, EnumMember>>();
            Dictionary<OutputApi, HashSet<EnumGroupInfo>> allEnumGroups = new Dictionary<OutputApi, HashSet<EnumGroupInfo>>();
            foreach (OutputApi outputApi in Enum.GetValues<OutputApi>())
            {
                if (outputApi == OutputApi.Invalid) continue;
                allEnumsPerAPI.Add(outputApi, new Dictionary<string, EnumMember>());
                allEnumGroups.Add(outputApi, new HashSet<EnumGroupInfo>());
            }

            foreach (EnumEntry @enum in spec.Enums)
            {
                foreach ((string originalName, string translatedName, ApiFile @namespace) in @enum.Groups)
                {
                    if (@namespace == ApiFile.GL)
                    {
                        AddToGroup(allEnumGroups, OutputApi.GL, originalName, translatedName, @enum.IsFlags);
                        AddToGroup(allEnumGroups, OutputApi.GLCompat, originalName, translatedName, @enum.IsFlags);
                        AddToGroup(allEnumGroups, OutputApi.GLES1, originalName, translatedName, @enum.IsFlags);
                        AddToGroup(allEnumGroups, OutputApi.GLES2, originalName, translatedName, @enum.IsFlags);
                    }
                    else if (@namespace == ApiFile.WGL)
                    {
                        AddToGroup(allEnumGroups, OutputApi.WGL, originalName, translatedName, @enum.IsFlags);
                    }
                    else if (@namespace == ApiFile.GLX)
                    {
                        AddToGroup(allEnumGroups, OutputApi.GLX, originalName, translatedName, @enum.IsFlags);
                    }
                    else if (@namespace == ApiFile.EGL)
                    {
                        AddToGroup(allEnumGroups, OutputApi.EGL, originalName, translatedName, @enum.IsFlags);
                    }

                    static void AddToGroup(Dictionary<OutputApi, HashSet<EnumGroupInfo>> allEnumGroups, OutputApi api, string originalName, string translatedName, bool isFlag)
                    {
                        // If the first groupNameToEnumGroup tag wasn't flagged as a bitmask, but later ones in the same groupName are.
                        // Then we want the groupName to be considered a bitmask.
                        if (allEnumGroups[api].TryGetValue(new EnumGroupInfo(originalName, translatedName, isFlag), out EnumGroupInfo? actual))
                        {
                            // In the current spec this case never happens, but it could.
                            // - 2021-07-04
                            if (isFlag == true && actual.IsFlags == false)
                            {
                                allEnumGroups[api].Remove(actual);
                                allEnumGroups[api].Add(actual with { IsFlags = true });
                            }
                        }
                        else
                        {
                            allEnumGroups[api].Add(new EnumGroupInfo(originalName, translatedName, isFlag));
                        }
                    }
                }

                EnumMember member = new EnumMember()
                {
                    Name = @enum.Name,
                    OriginalName = @enum.OriginalName,
                    Value = @enum.Value,
                    Groups = @enum.Groups,
                    IsFlag = @enum.IsFlags,
                };

                if (@enum.Apis == OutputApiFlags.None)
                {
                    throw new Exception();
                }

                if (@enum.Apis.HasFlag(OutputApiFlags.GL))
                {
                    allEnumsPerAPI.AddToNestedDict(OutputApi.GL, @enum.OriginalName, member);
                }

                if (@enum.Apis.HasFlag(OutputApiFlags.GLCompat))
                {
                    allEnumsPerAPI.AddToNestedDict(OutputApi.GLCompat, @enum.OriginalName, member);
                }

                if (@enum.Apis.HasFlag(OutputApiFlags.GLES1))
                {
                    allEnumsPerAPI.AddToNestedDict(OutputApi.GLES1, @enum.OriginalName, member);
                }

                if (@enum.Apis.HasFlag(OutputApiFlags.GLES2))
                {
                    allEnumsPerAPI.AddToNestedDict(OutputApi.GLES2, @enum.OriginalName, member);
                }

                if (@enum.Apis.HasFlag(OutputApiFlags.WGL))
                {
                    allEnumsPerAPI.AddToNestedDict(OutputApi.WGL, @enum.OriginalName, member);
                }

                if (@enum.Apis.HasFlag(OutputApiFlags.GLX))
                {
                    allEnumsPerAPI.AddToNestedDict(OutputApi.GLX, @enum.OriginalName, member);
                }

                if (@enum.Apis.HasFlag(OutputApiFlags.EGL))
                {
                    allEnumsPerAPI.AddToNestedDict(OutputApi.EGL, @enum.OriginalName, member);
                }
            }

            foreach (var (api, _, enums) in spec.APIs)
            {
                OutputApi outAPI = api switch
                {
                    InputAPI.GL => OutputApi.GL,
                    // FIXME?
                    //InputAPI.GLCompat => OutputApi.GLCompat,
                    InputAPI.GLES1 => OutputApi.GLES1,
                    InputAPI.GLES2 => OutputApi.GLES2,
                    InputAPI.WGL => OutputApi.WGL,
                    InputAPI.GLX => OutputApi.GLX,
                    InputAPI.EGL => OutputApi.EGL,

                    _ => throw new Exception(),
                };

                // FIXME: Do we need this here?
                ApiFile file = api switch
                {
                    InputAPI.GL => ApiFile.GL,
                    InputAPI.GLES1 => ApiFile.GL,
                    InputAPI.GLES2 => ApiFile.GL,
                    InputAPI.WGL => ApiFile.WGL,
                    InputAPI.GLX => ApiFile.GLX,
                    InputAPI.EGL => ApiFile.EGL,

                    _ => throw new Exception(),
                };

                CrossReferenceEnums(outAPI, file);

                // FIXME: Do we need to do this for GLCompat?
                // Could there be enums there that needs to be cross referenced?
                if (outAPI == OutputApi.GL)
                {
                    CrossReferenceEnums(OutputApi.GLCompat, file);
                }

                void CrossReferenceEnums(OutputApi outAPI, ApiFile glFile)
                {
                    bool removeFunctions = outAPI switch
                    {
                        OutputApi.GL => true,
                        OutputApi.GLES2 => true,
                        _ => false,
                    };

                    Dictionary<string, EnumMember>? enumsDict = allEnumsPerAPI[outAPI];

                    foreach (EnumReference enumRef in enums)
                    {
                        if (enumRef.IsCrossReferenced)
                            continue;

                        if (removeFunctions)
                        {
                            // FIXME: Should we check the profile of the extension??
                            if (enumRef.VersionInfo.RemovedBy.Count > 0 || enumRef.Profile == GLProfile.Compatibility)
                            {
                                // FIXME: Add the enum if an extension uses it??
                                continue;
                            }
                        }

                        // FIXME! This is a big hack!
                        // We don't want to process this "enum" as it is a string.
                        if (enumRef.EnumName == "GLX_EXTENSION_NAME") continue;

                        if (enumsDict.TryGetValue(enumRef.EnumName, out EnumMember? @enum))
                        {
                            foreach (var groupRef in @enum.Groups!)
                            {
                                ApiFile @namespace = groupRef.Namespace;
                                if (@namespace != glFile)
                                {
                                    if (@namespace == ApiFile.GL)
                                    {
                                        // FIXME: Cleanup

                                        // FIXME: Should we really add it to all GL apis?
                                        // Is there some good way to detect which ones we should add it to?
                                        AddEnumToAPI(OutputApi.GL, @enum);
                                        AddEnumToAPI(OutputApi.GLCompat, @enum);
                                        AddEnumToAPI(OutputApi.GLES1, @enum);
                                        AddEnumToAPI(OutputApi.GLES2, @enum);
                                    }
                                    else if (@namespace == ApiFile.WGL)
                                    {
                                        AddEnumToAPI(OutputApi.WGL, @enum);
                                    }
                                    else if (@namespace == ApiFile.GLX)
                                    {
                                        AddEnumToAPI(OutputApi.GLX, @enum);
                                    }
                                    else if (@namespace == ApiFile.EGL)
                                    {
                                        AddEnumToAPI(OutputApi.EGL, @enum);
                                    }

                                    void AddEnumToAPI(OutputApi outputApi, EnumMember @enum)
                                    {
                                        // FIXME: There is an issue where a cross referenced enum gets readded here.
                                        // We want to avoid this.

                                        if (allEnumsPerAPI[outputApi].ContainsKey(@enum.OriginalName) == false)
                                        {
                                            allEnumsPerAPI.AddToNestedDict(outputApi, @enum.OriginalName, @enum);
                                        }

                                        foreach (var api in spec.APIs)
                                        {
                                            if (MatchesAPI(api.Name, outputApi))
                                            {
                                                api.Enums.Add(new EnumReference(@enum.OriginalName, new VersionInfo(null, []), GLProfile.None, true));
                                                Logger.Info($"Added enum entry '{@enum.Name}' to {outputApi}.");
                                            }
                                        }

                                        AddToGroup(allEnumGroups, outputApi, groupRef, @enum.IsFlag);

                                        static bool MatchesAPI(InputAPI api, OutputApi output)
                                        {
                                            switch (api)
                                            {
                                                case InputAPI.GL: return output == OutputApi.GL || output == OutputApi.GLCompat;
                                                case InputAPI.GLES1: return output == OutputApi.GLES1;
                                                case InputAPI.GLES2: return output == OutputApi.GLES2;
                                                case InputAPI.WGL: return output == OutputApi.WGL;
                                                case InputAPI.GLX: return output == OutputApi.GLX;
                                                case InputAPI.EGL: return output == OutputApi.EGL;
                                                default: throw new Exception();
                                            }
                                        }

                                        // FIXME: Duplicate implementation, see above.
                                        static void AddToGroup(Dictionary<OutputApi, HashSet<EnumGroupInfo>> allEnumGroups, OutputApi api, GroupRef @ref, bool isFlag)
                                        {
                                            // If the first groupNameToEnumGroup tag wasn't flagged as a bitmask, but later ones in the same groupName are.
                                            // Then we want the groupName to be considered a bitmask.
                                            if (allEnumGroups[api].TryGetValue(new EnumGroupInfo(@ref.OriginalName, @ref.TranslatedName, isFlag), out EnumGroupInfo? actual))
                                            {
                                                // In the current spec this case never happens, but it could.
                                                // - 2021-07-04
                                                if (isFlag == true && actual.IsFlags == false)
                                                {
                                                    allEnumGroups[api].Remove(actual);
                                                    allEnumGroups[api].Add(actual with { IsFlags = true });
                                                }
                                            }
                                            else
                                            {
                                                allEnumGroups[api].Add(new EnumGroupInfo(@ref.OriginalName, @ref.TranslatedName, isFlag));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new Exception($"Could not find any enum called '{enumRef.EnumName}'.");
                        }
                    }
                }
            }

            List<OutputApiData> outputNamespaces = new List<OutputApiData>();

            foreach (var (api, functions, enums) in spec.APIs)
            {
                // FIXME: Probably make these the same enum!
                OutputApi outAPI = api switch
                {
                    InputAPI.GL => OutputApi.GL,
                    // FIXME?
                    //InputAPI.GLCompat => OutputApi.GLCompat,
                    InputAPI.GLES1 => OutputApi.GLES1,
                    InputAPI.GLES2 => OutputApi.GLES2,
                    InputAPI.WGL => OutputApi.WGL,
                    InputAPI.GLX => OutputApi.GLX,
                    InputAPI.EGL => OutputApi.EGL,

                    _ => throw new Exception(),
                };

                // FIXME: Do we need this here?
                ApiFile file = api switch
                {
                    InputAPI.GL => ApiFile.GL,
                    InputAPI.GLES1 => ApiFile.GL,
                    InputAPI.GLES2 => ApiFile.GL,
                    InputAPI.WGL => ApiFile.WGL,
                    InputAPI.GLX => ApiFile.GLX,
                    InputAPI.EGL => ApiFile.EGL,

                    _ => throw new Exception(),
                };

                outputNamespaces.Add(CreateOutputAPI(outAPI, file));

                if (outAPI == OutputApi.GL)
                {
                    outputNamespaces.Add(CreateOutputAPI(OutputApi.GLCompat, file));
                }

                OutputApiData CreateOutputAPI(OutputApi outAPI, ApiFile glFile)
                {
                    // Function processing

                    bool removeFunctions = outAPI switch
                    {
                        OutputApi.GL => true,
                        OutputApi.GLES2 => true,
                        _ => false,
                    };

                    HashSet<GroupRef> groupsReferencedByFunctions = new HashSet<GroupRef>();
                    Dictionary<string, List<FunctionData>> functionsByVendor = new Dictionary<string, List<FunctionData>>();
                    foreach (var functionRef in functions)
                    {
                        if (allFunctions.TryGetValue(functionRef.EntryPoint, out FunctionData? overloadedFunction))
                        {
                            bool referenced = false;

                            if (functionRef.VersionInfo.Version != null)
                            {
                                if (removeFunctions && (functionRef.VersionInfo.RemovedBy.Count > 0 || functionRef.Profile == GLProfile.Compatibility))
                                {
                                    // Do not add this function
                                }
                                else
                                {
                                    bool added = functionsByVendor.AddToNestedListIfNotPresent("", overloadedFunction);
                                    Debug.Assert(added);
                                    referenced = true;
                                }
                            }

                            foreach (var extension in functionRef.VersionInfo.Extensions)
                            {
                                functionsByVendor.AddToNestedListIfNotPresent(extension.Vendor, overloadedFunction);
                                referenced = true;
                            }

                            if (referenced)
                            {
                                groupsReferencedByFunctions.UnionWith(overloadedFunction.NativeFunction.ReferencedEnumGroups);
                            }
                        }
                        else
                        {
                            Logger.Error($"Function '{functionRef.EntryPoint}' not found.");
                        }
                    }

                    Dictionary<GroupRef, List<(string Vendor, Function Function)>> enumGroupToNativeFunctionsUsingThatEnumGroup = new Dictionary<GroupRef, List<(string Vendor, Function Function)>>();
                    Dictionary<string, VendorFunctions> vendors = new Dictionary<string, VendorFunctions>();
                    foreach (var (vendor, vendorFunctions) in functionsByVendor)
                    {
                        foreach (var function in vendorFunctions)
                        {
                            if (!vendors.TryGetValue(vendor, out VendorFunctions? group))
                            {
                                group = new VendorFunctions() { Vendor = vendor, Functions = [], NativeFunctionsWithPostfix = [] };
                                vendors.Add(vendor, group);
                            }

                            group.Functions.Add(new OverloadedFunction() { NativeFunction = function.NativeFunction, Overloads = function.Overloads });

                            if (function.ChangeNativeName)
                            {
                                group.NativeFunctionsWithPostfix.Add(function.NativeFunction);
                            }

                            foreach (var enumGroup in function.NativeFunction.ReferencedEnumGroups)
                            {
                                if (enumGroupToNativeFunctionsUsingThatEnumGroup.TryGetValue(enumGroup, out var listOfFunctions) == false)
                                {
                                    listOfFunctions = new List<(string Vendor, Function Function)>();
                                    enumGroupToNativeFunctionsUsingThatEnumGroup.Add(enumGroup, listOfFunctions);
                                }

                                if (listOfFunctions.Contains((vendor, function.NativeFunction)) == false)
                                {
                                    listOfFunctions.Add((vendor, function.NativeFunction));
                                }
                            }
                        }
                    }

                    List<VendorFunctions> sortedVendorFunctions = [.. vendors.Values];
                    foreach (VendorFunctions functions in sortedVendorFunctions)
                    {
                        functions.Functions.Sort();
                    }
                    sortedVendorFunctions.Sort((e1, e2) => e1.Vendor.CompareTo(e2.Vendor));

                    Dictionary<Function, FunctionDocumentation> documentation = new Dictionary<Function, FunctionDocumentation>();
                    foreach (var (vendor, vendorFunctions) in functionsByVendor)
                    {
                        foreach (var function in vendorFunctions)
                        {
                            FunctionReference func = functions.Find(f => f.EntryPoint == function.NativeFunction.EntryPoint) ?? throw new Exception($"Could not find function {function.NativeFunction.EntryPoint}!");

                            List<Link> extensionURLs = [];
                            foreach (var extension in func.VersionInfo.Extensions)
                            {
                                string ext = NameMangler.MaybeRemoveStart(extension.Name, "GL_");

                                string url;
                                if (vendor == "ANGLE" || vendor == "CHROMIUM")
                                {
                                    url = $"https://chromium.googlesource.com/angle/angle/+/refs/heads/main/extensions/{ext}.txt";
                                }
                                else
                                {
                                    string apiString = api switch
                                    {
                                        InputAPI.GL => "OpenGL",
                                        InputAPI.GLES1 => "OpenGL",
                                        InputAPI.GLES2 => "OpenGL",
                                        InputAPI.WGL => "OpenGL",
                                        InputAPI.GLX => "OpenGL",
                                        InputAPI.EGL => "EGL",
                                        _ => throw new Exception()
                                    };

                                    url = $"https://registry.khronos.org/{apiString}/extensions/{vendor}/{ext}.txt";
                                }
                                extensionURLs.Add(new Link(url, $"{ext}.txt"));
                            }

                            if (function.Documentation.TryGetValue(outAPI, out FunctionDocumentation? commandDocumentation))
                            {
                                documentation[function.NativeFunction] = commandDocumentation with {
                                    VersionInfo = func.VersionInfo,
                                    RefPagesLinks = [.. commandDocumentation.RefPagesLinks, .. extensionURLs]
                                };
                            }
                            else
                            {
                                // Extensions don't have documentation, so we don't warn about them.
                                if (vendor == "")
                                {
                                    Logger.Warning($"{function.NativeFunction.EntryPoint} doesn't have any documentation for {api}");
                                }

                                documentation[function.NativeFunction] = new FunctionDocumentation()
                                {
                                    FunctionName = function.NativeFunction.EntryPoint,
                                    Purpose = "",
                                    Parameters = [],
                                    RefPagesLinks = extensionURLs,
                                    VersionInfo = func.VersionInfo,
                                };
                            }
                        }
                    }

                    /// Enum processing

                    Dictionary<string, EnumMember>? enumsDict = allEnumsPerAPI[outAPI];

                    Dictionary<string, List<EnumMember>> groupNameToEnumGroup = new Dictionary<string, List<EnumMember>>();

                    HashSet<EnumMember> theAllEnumGroup = new HashSet<EnumMember>();

                    // FIXME: Here we are trusting that the enum refs in the <require> tags tell us all of the
                    // enums to include. But this is not necessarily true as is the case with WGL as it references
                    // some enums from OpenGL without them going through the require tag...
                    // - Noggin_bops 2023-08-26
                    foreach (var enumRef in enums)
                    {
                        if (removeFunctions)
                        {
                            // FIXME: Should we check the profile of the extension??
                            if (enumRef.VersionInfo.RemovedBy.Count > 0 || enumRef.Profile == GLProfile.Compatibility)
                            {
                                // FIXME: Add the enum if an extension uses it??
                                continue;
                            }
                        }

                        // FIXME! This is a big hack!
                        // We don't want to process this "enum" as it is a string.
                        if (enumRef.EnumName == "GLX_EXTENSION_NAME") continue;

                        if (enumsDict.TryGetValue(enumRef.EnumName, out EnumMember? @enum))
                        {
                            foreach (var (originalName, translatedName, @namespace) in @enum.Groups!)
                            {
                                if (@namespace != glFile)
                                    continue;

                                if (groupNameToEnumGroup.TryGetValue(translatedName, out List<EnumMember>? groupMembers) == false)
                                {
                                    groupMembers = new List<EnumMember>();
                                    groupNameToEnumGroup.Add(translatedName, groupMembers);
                                }

                                if (groupMembers.Find(g => g.Name == @enum.Name) == null)
                                {
                                    groupMembers.Add(@enum);
                                }
                            }

                            if (@enum.Value <= uint.MaxValue)
                            {
                                theAllEnumGroup.Add(@enum);
                            }
                        }
                        else
                        {
                            throw new Exception($"Could not find any enum called '{enumRef.EnumName}'.");
                        }
                    }

                    // Go through all of the groupNameToEnumGroup and put them into their groups

                    // Add keys + lists for all enumName names
                    List<EnumType> finalGroups = new List<EnumType>();
                    foreach ((string originalName, string translatedName, bool isFlags) in allEnumGroups[outAPI])
                    {
                        if (groupNameToEnumGroup.TryGetValue(translatedName, out List<EnumMember>? members) == false)
                        {
                            members = [];
                            groupNameToEnumGroup.Add(translatedName, members);
                        }

                        // SpecialNumbers is not an enumName groupName that we want to output.
                        // We handle these entries differently as some of the entries don't fit in an int.
                        if (originalName == "SpecialNumbers")
                            continue;

                        // Remove all empty enumName groups, except the empty groups referenced by included vendorFunctions.
                        // In GL 4.1 to 4.5 there are vendorFunctions that use the groupName "ShaderBinaryFormat"
                        // while not including any members for that enumName groupName.
                        // This is needed to solve that case.
                        if (members.Count <= 0 && groupsReferencedByFunctions.Contains(new GroupRef(originalName, translatedName, glFile)) == false)
                            continue;

                        if (enumGroupToNativeFunctionsUsingThatEnumGroup.TryGetValue(new GroupRef(originalName, translatedName, glFile), out var functionsUsingEnumGroup) == false)
                        {
                            functionsUsingEnumGroup = null;
                        }

                        // If there is a list, sort it by name
                        if (functionsUsingEnumGroup != null)
                            functionsUsingEnumGroup.Sort((f1, f2) => {
                                // We want to prioritize "core" vendorFunctions before extensions.
                                if (f1.Vendor == "" && f2.Vendor != "") return -1;
                                if (f1.Vendor != "" && f2.Vendor == "") return 1;

                                return f1.Function.Name.CompareTo(f2.Function.Name);
                            });

                        members.Sort(EnumMember.DefaultComparison);

                        finalGroups.Add(new EnumType()
                        {
                            Name = translatedName,
                            IsFlags = isFlags,
                            Members = members,
                            ReferencedBy = [],
                            FunctionsUsingEnumGroup = functionsUsingEnumGroup,
                        });
                    }
                    foreach (var group in groupsReferencedByFunctions)
                    {
                        // This group is not part of this file, so we can't do anything here about adding it.
                        // For now this is not a problem as all referenced groups from between the different
                        // files are always populated, so we will never have to add them to the other file.
                        // - Noggin_bops 2025-08-05
                        if (group.Namespace != file)
                        {
                            continue;
                        }

                        if (groupNameToEnumGroup.TryGetValue(group.TranslatedName, out List<EnumMember>? members) == false)
                        {
                            if (enumGroupToNativeFunctionsUsingThatEnumGroup.TryGetValue(group, out var functionsUsingEnumGroup) == false)
                            {
                                functionsUsingEnumGroup = null;
                            }

                            finalGroups.Add(new EnumType()
                            {
                                Name = group.TranslatedName,
                                IsFlags = false,
                                Members = [],
                                ReferencedBy = [],
                                FunctionsUsingEnumGroup = functionsUsingEnumGroup,
                            });
                        }
                    }

                    // Sort enum groups be name
                    finalGroups.Sort((g1, g2) => g1.Name.CompareTo(g2.Name));

                    List<EnumMember> allEnumGroup = theAllEnumGroup.ToList();
                    allEnumGroup.Sort(EnumMember.DefaultComparison);

                    // Add the All enum group first.
                    finalGroups.Insert(0, new EnumType()
                    {
                        Name = "All",
                        IsFlags = false,
                        Members = allEnumGroup,
                        ReferencedBy = [],
                        FunctionsUsingEnumGroup = null,
                    });

                    return new OutputApiData(outAPI, sortedVendorFunctions, finalGroups, documentation);
                }
            }

            // FIXME: This requires us to merge all input data!
            // FIXME: Potentially split the GLES function pointers from the GL ones.
            List<ApiPointers> pointers =
            [
                CreatePointersList(ApiFile.GL, outputNamespaces),
                CreatePointersList(ApiFile.WGL, outputNamespaces),
                CreatePointersList(ApiFile.GLX, outputNamespaces),
                CreatePointersList(ApiFile.EGL, outputNamespaces),
            ];

            return new OutputData(pointers, outputNamespaces);

            ApiPointers CreatePointersList(ApiFile file, List<OutputApiData> namespaces)
            {
                SortedList<string, Function> allFunctions = new SortedList<string, Function>();
                foreach (OutputApiData @namespace in namespaces)
                {
                    bool addFunctions = false;
                    switch (file)
                    {
                        case ApiFile.GL:
                            if (@namespace.Api == OutputApi.GL ||
                                @namespace.Api == OutputApi.GLCompat ||
                                @namespace.Api == OutputApi.GLES1 ||
                                @namespace.Api == OutputApi.GLES2)
                            {
                                addFunctions = true;
                            }
                            break;
                        case ApiFile.WGL:
                            if (@namespace.Api == OutputApi.WGL)
                            {
                                addFunctions = true;
                            }
                            break;
                        case ApiFile.GLX:
                            if (@namespace.Api == OutputApi.GLX)
                            {
                                addFunctions = true;
                            }
                            break;
                        case ApiFile.EGL:
                            if (@namespace.Api == OutputApi.EGL)
                            {
                                addFunctions = true;
                            }
                            break;
                    }

                    if (addFunctions)
                    {
                        foreach (var functions in @namespace.VendorFunctions)
                        {
                            foreach (var function in functions.Functions)
                            {
                                if (allFunctions.ContainsKey(function.NativeFunction.EntryPoint) == false)
                                {
                                    allFunctions.Add(function.NativeFunction.EntryPoint, function.NativeFunction);
                                }
                            }
                        }
                    }
                }

                return new ApiPointers(file, allFunctions.Values.ToList());
            }
        }

        internal static Dictionary<OutputApi, FunctionDocumentation> MakeDocumentationForNativeFunction(Function function, Documentation documentation)
        {
            Dictionary<OutputApi, FunctionDocumentation> commandDocs = new Dictionary<OutputApi, FunctionDocumentation>();

            foreach (var (version, versionDocumentation) in documentation.VersionDocumentation)
            {
                if (versionDocumentation.TryGetValue(function.EntryPoint, out FunctionDocumentation? commandDoc))
                {
                    if (function.Parameters.Count != commandDoc.Parameters.Length)
                    {
                        Logger.Warning($"Function {function.EntryPoint} has differnet number of parameters than the parsed documentation. (gl.xml:{function.Parameters.Count}, documentation:{commandDoc.Parameters.Length})");
                    }

                    for (int i = 0; i < Math.Min(function.Parameters.Count, commandDoc.Parameters.Length); i++)
                    {
                        if (function.Parameters[i].OriginalName != commandDoc.Parameters[i].ParameterName)
                        {
                            Logger.Warning($"[{version}][{function.EntryPoint}] Function parameter '{function.Parameters[i].OriginalName}' doesn't have the same name in the documentation. ('{commandDoc.Parameters[i].ParameterName}')");
                        }
                    }

                    commandDocs.Add(version, commandDoc);
                }
            }

            return commandDocs;
        }

        public static readonly IOverloader[] Overloaders = [
                new TrimNameOverloader(TrimNameOverloader.EndingsNotToTrimOpenGL),

                new StringReturnOverloader(),
                new BoolReturnOverloader(),

                new ColorTypeOverloader(),
                new MathTypeOverloader(),
                new FunctionPtrToDelegateOverloader(),
                new PointerToOffsetOverloader(),
                new ObjectPtrLabelOverloader(),
                new VoidPtrToIntPtrOverloader(),
                new GenCreateAndDeleteOverloader(
                    GenCreateAndDeleteOverloader.PluralNameToSingularNameOpenGL,
                    GenCreateAndDeleteOverloader.PluralParameterNameToSingularNameOpenGL),
                new StringOverloader(),
                new StringArrayOverloader(),
                new SpanAndArrayOverloader(),
                new RefInsteadOfPointerOverloader(),
                new OutToReturnOverloader(),
            ];

        // Maybe we can do the return type overloading in a post processing step?
        internal static FunctionData GenerateOverloads(Function nativeFunction, Dictionary<OutputApi, FunctionDocumentation> functionDocumentation)
        {
            List<Overload> overloads = new List<Overload>
            {
                // Make a "base" overload
                new Overload(null, null, nativeFunction.Parameters.ToArray(), nativeFunction, nativeFunction.StrongReturnType!,
                    new NameTable(), /*"returnValue",*/ Array.Empty<string>(), nativeFunction.Name),
            };

            bool overloadedOnce = false;
            foreach (IOverloader overloader in Overloaders)
            {
                List<Overload> newOverloads = new List<Overload>();
                foreach (Overload overload in overloads)
                {
                    if (overloader.TryGenerateOverloads(overload, out List<Overload>? overloaderOverloads))
                    {
                        overloadedOnce = true;

                        newOverloads.AddRange(overloaderOverloads);
                    }
                    else
                    {
                        newOverloads.Add(overload);
                    }
                }
                // Replace the old overloads with the new overloads
                overloads = newOverloads;
            }
            Overload[] overloadArray = overloadedOnce ? overloads.ToArray() : Array.Empty<Overload>();

            bool changeNativeName = false;
            foreach (Overload overload in overloadArray)
            {
                if (AreSignaturesDifferent(nativeFunction, overload) == false)
                {
                    changeNativeName = true;
                }
            }

            return new FunctionData(nativeFunction, functionDocumentation, overloadArray, changeNativeName);

            static bool AreSignaturesDifferent(Function nativeFunction, Overload overload)
            {
                if (nativeFunction.Parameters.Count != overload.InputParameters.Length)
                {
                    return true;
                }

                if (overload.OverloadName != nativeFunction.Name)
                {
                    return true;
                }

                for (int i = 0; i < nativeFunction.Parameters.Count; i++)
                {
                    if (nativeFunction.Parameters[i].StrongType!.Equals(overload.InputParameters[i].StrongType!) == false)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
