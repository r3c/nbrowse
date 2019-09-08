using System.Threading.Tasks;

namespace NBrowse.Evaluation
{
	internal interface IEvaluator
	{
		Task<TResult> Evaluate<TResult>(string expression);
	}
}