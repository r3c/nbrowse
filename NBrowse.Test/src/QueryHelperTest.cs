using System.Threading.Tasks;
using Moq;
using NBrowse.Execution;
using NUnit.Framework;

namespace NBrowse.Test
{
	public class QueryHelperTest
	{
		[Test]
		[TestCase("42", 42)]
		[TestCase("\"Hello, World!\"", "Hello, World!")]
		[TestCase("false", false)]
		[TestCase("(project) => 17", 17)]
		[TestCase("project => 17", 17)]
		public async Task LoadAndEvaluate_Constant<T>(string query, T expected)
		{
			await QueryHelperTest.QueryAndAssert(query, expected);
		}

		private static async Task QueryAndAssert<T>(string query, T expected)
		{
			var printer = new Mock<IPrinter>();

			await QueryHelper.QueryAndPrint(new[] {typeof(QueryHelperTest).Assembly.Location}, query, printer.Object);

			printer.Verify(p => p.Print<object>(expected));
			printer.VerifyNoOtherCalls();
		}
	}
}