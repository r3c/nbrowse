using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace NBrowse.Reflection;

public abstract class NImplementation
{
    [Description("Implementation parent method")]
    [JsonIgnore]
    public abstract NMethod Parent { get; }

    [Description("Methods referenced in code")]
    [JsonIgnore]
    public abstract IEnumerable<NMethod> ReferencedMethods { get; }

    [Description("Types referenced in code")]
    [JsonIgnore]
    public abstract IEnumerable<NType> ReferencedTypes { get; }
}