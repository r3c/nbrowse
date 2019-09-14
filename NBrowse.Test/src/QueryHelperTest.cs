using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NBrowse.Execution;
using NUnit.Framework;

namespace NBrowse.Test
{
	public class QueryHelperTest
	{
		[Test]
		[TestCase("", "arguments.Count", 0)]
		[TestCase("a", "arguments[0]", "a")]
		[TestCase("a,b", "arguments[1]", "b")]
		public async Task LoadAndEvaluate_Arguments<T>(string arguments, string query, T expected)
		{
			await QueryHelperTest.QueryAndAssert(arguments.Split(',', StringSplitOptions.RemoveEmptyEntries), query,
				expected);
		}

		[Test]
		[TestCase("42", 42)]
		[TestCase("\"Hello, World!\"", "Hello, World!")]
		[TestCase("false", false)]
		[TestCase("(project) => 17", 17)]
		[TestCase("project => 17", 17)]
		public async Task LoadAndEvaluate_Constant<T>(string query, T expected)
		{
			await QueryHelperTest.QueryAndAssert(Array.Empty<string>(), query, expected);
		}

		private static async Task QueryAndAssert<T>(IReadOnlyList<string> arguments, string query, T expected)
		{
			var printer = new Mock<IPrinter>();

			await QueryHelper.QueryAndPrint(new[] {typeof(QueryHelperTest).Assembly.Location}, arguments, query,
				printer.Object);

			printer.Verify(p => p.Print<object>(expected));
			printer.VerifyNoOtherCalls();
		}
	}
}