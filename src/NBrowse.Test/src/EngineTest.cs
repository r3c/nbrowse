using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NBrowse.Execution;
using NUnit.Framework;

namespace NBrowse.Test;

public class EngineTest
{
    [Test]
    [TestCase("x", "(project, arguments) => x")]
    [TestCase("a => x", "(a, _) => x")]
    [TestCase("(a) => x", "(a, _) => x")]
    [TestCase("(a, b) => x", "(a, b) => x")]
    [TestCase("    a => x", "(a, _) => x")]
    [TestCase("\n\ta => x", "(a, _) => x")]
    [TestCase("/* comment */ a => x", "(a, _) => x")]
    [TestCase("    /* comment */    a => x", "(a, _) => x")]
    [TestCase("// comment\na => x", "(a, _) => x")]
    [TestCase("  // comment\r\na => x", "(a, _) => x")]
    [TestCase("  /* mix */// white\n/* stuff */  a => x", "(a, _) => x")]
    public void NormalizeQuery(string query, string expected)
    {
        Assert.That(Engine.NormalizeQuery(query), Is.EqualTo(expected));
    }

    [Test]
    [TestCase("", "(_, arguments) => arguments.Count", 0)]
    [TestCase("a", "(_, arguments) => arguments[0]", "a")]
    [TestCase("a,b", "(_, arguments) => arguments[1]", "b")]
    public async Task QueryAndPrint_Arguments<T>(string arguments, string query, T expected)
    {
        await QueryAndAssert(arguments.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries), query,
            expected);
    }

    [Test]
    [TestCase("(p, a) => 42", 42)]
    [TestCase("(p, a) => \"Hello, World!\"", "Hello, World!")]
    [TestCase("(p, a) => false", false)]
    public async Task QueryAndPrint_Constant<T>(string query, T expected)
    {
        await QueryAndAssert(Array.Empty<string>(), query, expected);
    }

    private static async Task QueryAndAssert<T>(IReadOnlyList<string> arguments, string query, T expected)
    {
        var printer = new Mock<IPrinter>();

        await Engine.QueryAndPrint(new[] { typeof(EngineTest).Assembly.Location }, arguments, query,
            printer.Object);

        printer.Verify(p => p.Print<object>(expected));
        printer.VerifyNoOtherCalls();
    }
}