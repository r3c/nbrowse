using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace NBrowse.Reflection;

public abstract class NProject
{
    [Description("Loaded assemblies")]
    [JsonIgnore]
    public abstract IEnumerable<NAssembly> Assemblies { get; }

    [Description("Find multiple assemblies by name")]
    public abstract IEnumerable<NAssembly> FilterAssemblies(IEnumerable<string> name);

    [Description("Find assembly by name")]
    public abstract NAssembly FindAssembly(string name);

    [Description("Find method by name")]
    public abstract NMethod FindMethod(string search);

    [Description("Find type by name")]
    public abstract NType FindType(string search);
}