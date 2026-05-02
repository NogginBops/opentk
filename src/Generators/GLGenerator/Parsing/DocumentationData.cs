using GeneratorBase;
using GLGenerator.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLGenerator.Parsing
{
    internal record Documentation(
        // FIXME: Better name
        Dictionary<OutputApi, Dictionary<string, FunctionDocumentation>> VersionDocumentation
        );
}
