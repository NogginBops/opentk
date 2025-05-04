using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using ALGenerator.Parsing;
using ALGenerator.Process;
using ALGenerator.Utility;

namespace ALGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            using Logger logger = Logger.CreateLogger(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!, "log.txt"));

            Stopwatch watch = Stopwatch.StartNew();

            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;

            using FileStream alSpecificationStream = Reader.ReadALSpecFromGithub();
            using FileStream alcSpecificationStream = Reader.ReadALCSpecFromGithub();

            var alSpecData = SpecificationParser.Parse(alSpecificationStream);
            var alcSpecData = SpecificationParser.Parse(alcSpecificationStream);

            Processor.ApplyFeatureEnums(alSpecData);
            Processor.ApplyExtensionEnums(alSpecData);
            Processor.ApplyVideoEnums(alSpecData, alcSpecData);
            Processor.ResolveEnumUnderlyingTypes(alSpecData);
            Processor.ResolveEnumUnderlyingTypes(alcSpecData);
            var typeMap = Processor.BuildTypeMap(alSpecData, alcSpecData);

            Processor.ApplyExtensionConstants(alSpecData);
            Processor.ApplyExtensionConstants(alcSpecData);
            var constMap = Processor.BuildConstantsMap(alSpecData, alcSpecData);

            Processor.ResolveStructMemberTypes(alSpecData, typeMap, constMap);
            Processor.ResolveHandleParent(alSpecData);
            Processor.ResolveCommandTypes(alSpecData, typeMap);
            Processor.ResolveVersionInfo(alSpecData, typeMap);

            Processor.ResolveStructMemberTypes(alcSpecData, typeMap, constMap);

            Processor.SortMembers(alSpecData);
            Processor.SortMembers(alcSpecData);

            Writer.Write(alSpecData, alcSpecData);

            watch.Stop();

            Console.WriteLine($"Wrote vulkan bindings in {watch.Elapsed.TotalMilliseconds}ms");
        }
    }
}
