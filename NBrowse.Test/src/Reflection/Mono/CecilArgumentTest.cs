using System;
using System.Linq;
using NBrowse.Reflection;
using NUnit.Framework;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedParameter.Global

namespace NBrowse.Test.Reflection.Mono
{
	public class CecilArgumentTest
	{
		[Test]
		[TestCase("CecilArgumentMethod", 0, null)]
		[TestCase("CecilArgumentMethod", 1, 3)]
		[TestCase("CecilArgumentMethod", 2, "hello")]
		public void DefaultValue(string method, int index, object expected)
		{
			var argument = CecilArgumentTest.GetArgumentFrom(method, index);

			Assert.That(argument.DefaultValue, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilArgumentMethod", 0, false)]
		[TestCase("CecilArgumentMethod", 1, true)]
		[TestCase("CecilArgumentMethod", 2, true)]
		public void HasDefaultValue(string method, int index, bool expected)
		{
			var argument = CecilArgumentTest.GetArgumentFrom(method, index);

			Assert.That(argument.HasDefaultValue, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilArgumentMethod", 0, "a")]
		[TestCase("CecilArgumentMethod", 1, "b")]
		[TestCase("CecilArgumentMethod", 2, "c")]
		public void Name(string method, int index, string expected)
		{
			var argument = CecilArgumentTest.GetArgumentFrom(method, index);

			Assert.That(argument.Name, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilArgumentMethod", 0, NBrowse.Reflection.Modifier.Out)]
		[TestCase("CecilArgumentMethod", 1, NBrowse.Reflection.Modifier.In)]
		[TestCase("CecilArgumentMethod", 2, NBrowse.Reflection.Modifier.None)]
		public void Modifier(string method, int index, Modifier expected)
		{
			var argument = CecilArgumentTest.GetArgumentFrom(method, index);

			Assert.That(argument.Modifier, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilArgumentMethod", 0, "Boolean&")]
		[TestCase("CecilArgumentMethod", 1, "Int32&")]
		[TestCase("CecilArgumentMethod", 2, nameof(String))]
		public void Type(string method, int index, string expected)
		{
			var argument = CecilArgumentTest.GetArgumentFrom(method, index);

			Assert.That(argument.Type.Name, Is.EqualTo(expected));
		}

		private static IArgument GetArgumentFrom(string method, int index)
		{
			return CecilProjectTest.CreateProject().FindMethod(method).Arguments.ToArray()[index];
		}

		public class TestClass
		{
			public void CecilArgumentMethod(out bool a, in int b = 3, string c = "hello")
			{
				a = default;
			}
		}
	}
}