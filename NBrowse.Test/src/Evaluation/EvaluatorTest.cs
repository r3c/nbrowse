using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NBrowse.Evaluation;
using NBrowse.Reflection;
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

		[Test]
		public async Task Query_Project_FilterAssemblies()
		{
			var assemblies = await EvaluatorTest.CreateAndQuery<IAssembly[]>(
				$"project => project.FilterAssemblies(new [] {{\"Missing1\", \"{typeof(EvaluatorTest).Assembly.GetName().Name}\", \"Missing2\"}}).ToArray()");

			Assert.That(assemblies.Length, Is.EqualTo(1));
			Assert.That(assemblies[0].Name, Is.EqualTo(typeof(EvaluatorTest).Assembly.GetName().Name));
		}

		[Test]
		public async Task Query_Project_FindExistingAssembly()
		{
			var assembly = await EvaluatorTest.CreateAndQuery<IAssembly>(
				$"project => project.FindAssembly(\"{typeof(EvaluatorTest).Assembly.GetName().Name}\")");

			StringAssert.EndsWith("NBrowse.Test.dll", assembly.FileName);
		}

		[Test]
		public void Query_Project_FindMissingAssembly()
		{
			Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
				await EvaluatorTest.CreateAndQuery<IAssembly>("project => project.FindAssembly(\"DoesNotExist\")"));
		}

		[Test]
		public async Task Query_Project_FindExistingTypeByIdentifier()
		{
			var type = await EvaluatorTest.CreateAndQuery<IType>(
				$"project => project.FindType(\"{typeof(EvaluatorTest).FullName}\")");

			Assert.That(type.Identifier, Is.EqualTo(typeof(EvaluatorTest).FullName));
		}

		[Test]
		public async Task Query_Project_FindExistingTypeByName()
		{
			var type = await EvaluatorTest.CreateAndQuery<IType>(
				"project => project.FindType(\"" + nameof(EvaluatorTest) + "\")");

			Assert.That(type.Identifier, Is.EqualTo(typeof(EvaluatorTest).FullName));
		}

		[Test]
		public void Query_Project_FindMissingType()
		{
			Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () =>
				await EvaluatorTest.CreateAndQuery<IType>("project => project.FindType(\"DoesNotExist\")"));
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