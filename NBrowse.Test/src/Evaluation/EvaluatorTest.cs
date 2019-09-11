using System;
using System.Threading.Tasks;
using NBrowse.Evaluation;
using NUnit.Framework;

namespace NBrowse.Test.Evaluation
{
	public class EvaluatorTest
	{
		[Test]
		[TestCase("project => 42", 42)]
		[TestCase("project => \"Hello, World!\"", "Hello, World!")]
		[TestCase("project => false", false)]
		public async Task LoadAndEvaluate_Constant<T>(string expression, T expected)
		{
			Assert.That(await EvaluatorTest.LoadAndEvaluate<T>(expression), Is.EqualTo(expected));
		}

		private static async Task<T> LoadAndEvaluate<T>(string expression)
		{
			var untyped = await Evaluator.LoadAndEvaluate(new[] {typeof(EvaluatorTest).Assembly.Location}, expression);

			if (untyped is T typed)
				return typed;

			throw new InvalidOperationException("invalid return type");
		}
	}
}