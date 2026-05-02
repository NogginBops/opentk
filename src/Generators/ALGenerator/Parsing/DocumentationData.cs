using ALGenerator.Process;
using GeneratorBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALGenerator.Parsing
{
    internal record Documentation(
        Dictionary<string, EnumMemberDocumentation> EnumDocs,
        Dictionary<string, FunctionDocumentation> CommandDocs
        );
}
