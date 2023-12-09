using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace NBrowse.Reflection;

public abstract class Project
{
    [Description("Loaded assemblies")]
    [JsonIgnore]
    public abstract IEnumerable<Assembly> Assemblies { get; }

    [Description("Find multiple assemblies by name")]
    public abstract IEnumerable<Assembly> FilterAssemblies(IEnumerable<string> name);

    [Description("Find assembly by name")]
    public abstract Assembly FindAssembly(string name);

    [Description("Find method by name")]
    public abstract Method FindMethod(string search);

    [Description("Find type by name")]
    public abstract Type FindType(string search);
}