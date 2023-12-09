using System.Collections.Generic;
using System.Threading.Tasks;
using NBrowse.Reflection;

namespace NBrowse.Execution;

internal interface IEvaluator
{
    Task<TResult> Evaluate<TResult>(NProject nProject, IReadOnlyList<string> arguments, string expression);
}