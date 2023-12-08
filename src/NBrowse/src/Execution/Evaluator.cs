using NBrowse.Execution.Evaluators;

namespace NBrowse.Execution
{
    internal static class Evaluator
    {
        public static IEvaluator CreateRoslyn()
        {
            return new RoslynEvaluator();
        }
    }
}