using System.Linq;
using NBrowse.Reflection;
using NUnit.Framework;

namespace NBrowse.Test.Reflection.Mono
{
	public class CecilArgumentTest
	{
		[Test]
		[TestCase("CecilArgumentMethod", 0, false)]
		[TestCase("CecilArgumentMethod", 1, true)]
		public void HasDefaultValue(string method, int index, bool expected)
		{
			var argument = CecilArgumentTest.GetArgumentFrom(method, index);

			Assert.That(argument.HasDefaultValue, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilArgumentMethod", 0, "a")]
		[TestCase("CecilArgumentMethod", 1, "b")]
		public void Name(string method, int index, string expected)
		{
			var argument = CecilArgumentTest.GetArgumentFrom(method, index);

			Assert.That(argument.Name, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilArgumentMethod", 0, "String")]
		[TestCase("CecilArgumentMethod", 1, "Int32")]
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
			public void CecilArgumentMethod(string a, int b = 3)
			{
			}
		}
	}
}