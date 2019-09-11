using System;
using System.Runtime.CompilerServices;
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
		public async Task Query_Constant_ReturnLiteral<T>(string expression, T expected)
		{
			Assert.That(await EvaluatorTest.CreateAndQuery<T>(expression), Is.EqualTo(expected));
		}

		[Test]
		public async Task Query_Has_TypeCustomAttribute()
		{
			Assert.That(
				await EvaluatorTest.CreateAndQuery<bool>(
					$"project => Has.Attribute<System.Runtime.CompilerServices.CompilerGeneratedAttribute>(project.FindType(\"{nameof(EvaluatorTest)}+{nameof(GeneratedStructure)}\"))"),
				Is.True);
		}

		[Test]
		public async Task Query_Is_TypeGenerated()
		{
			Assert.That(
				await EvaluatorTest.CreateAndQuery<bool>(
					$"project => Is.Generated(project.FindType(\"{nameof(EvaluatorTest)}+{nameof(GeneratedStructure)}\"))"),
				Is.True);
		}

		private static async Task<T> CreateAndQuery<T>(string expression)
		{
			var untyped = await Evaluator.LoadAndEvaluate(new[] {typeof(EvaluatorTest).Assembly.Location}, expression);

			if (untyped is T typed)
				return typed;

			throw new InvalidOperationException("invalid return type");
		}

		[CompilerGenerated]
		private struct GeneratedStructure
		{
		}
	}
}