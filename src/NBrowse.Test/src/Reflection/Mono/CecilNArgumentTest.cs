using System;
using System.Linq;
using NBrowse.Reflection;
using NUnit.Framework;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedParameter.Global

namespace NBrowse.Test.Reflection.Mono;

public class CecilNArgumentTest
{
    [Test]
    [TestCase(nameof(TestClass.Argument), 0, null)]
    [TestCase(nameof(TestClass.Argument), 1, 3)]
    [TestCase(nameof(TestClass.Argument), 2, "hello")]
    public void DefaultValue(string method, int index, object expected)
    {
        var argument = GetArgumentFrom(method, index);

        Assert.That(argument.DefaultValue, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(nameof(TestClass.Argument), 0, false)]
    [TestCase(nameof(TestClass.Argument), 1, true)]
    [TestCase(nameof(TestClass.Argument), 2, true)]
    public void HasDefaultValue(string method, int index, bool expected)
    {
        var argument = GetArgumentFrom(method, index);

        Assert.That(argument.HasDefaultValue, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(nameof(TestClass.Argument), 0, "a")]
    [TestCase(nameof(TestClass.Argument), 1, "b")]
    [TestCase(nameof(TestClass.Argument), 2, "c")]
    public void Name(string method, int index, string expected)
    {
        var argument = GetArgumentFrom(method, index);

        Assert.That(argument.Name, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(nameof(TestClass.Argument), 0, NModifier.Out)]
    [TestCase(nameof(TestClass.Argument), 1, NModifier.In)]
    [TestCase(nameof(TestClass.Argument), 2, NModifier.None)]
    public void Modifier(string method, int index, NModifier expected)
    {
        var argument = GetArgumentFrom(method, index);

        Assert.That(argument.NModifier, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(nameof(TestClass.Argument), 0, "Boolean&")]
    [TestCase(nameof(TestClass.Argument), 1, "Int32&")]
    [TestCase(nameof(TestClass.Argument), 2, nameof(String))]
    public void Type(string method, int index, string expected)
    {
        var argument = GetArgumentFrom(method, index);

        Assert.That(argument.NType.Name, Is.EqualTo(expected));
    }

    private static NArgument GetArgumentFrom(string method, int index)
    {
        return CecilNProjectTest.CreateProject().FindMethod(method).Arguments.ToArray()[index];
    }

    private static class TestClass
    {
        public static void Argument(out bool a, in int b = 3, string c = "hello")
        {
            a = default;
        }
    }
}