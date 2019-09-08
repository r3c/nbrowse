using System;
using System.Linq;
using System.Runtime.CompilerServices;
using NBrowse.Reflection;
using NUnit.Framework;

namespace NBrowse.Test.Reflection.Mono
{
	public class CecilAttributeTest
	{
		[Test]
		[TestCase("CecilAttributeType", 0, "CompilerGeneratedAttribute")]
		[TestCase("CecilAttributeType", 1, "ObsoleteAttribute")]
		public void TypeFromMethod(string name, int index, string expected)
		{
			Assert.That(CecilAttributeTest.GetAttributeFromMethod(name, index).Type.Name, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilAttributeTest+ICecilAttributeType", 0, "CompilerGeneratedAttribute")]
		[TestCase("CecilAttributeTest+ICecilAttributeType", 1, "ObsoleteAttribute")]
		public void TypeFromType(string name, int index, string expected)
		{
			Assert.That(CecilAttributeTest.GetAttributeFromType(name, index).Type.Name, Is.EqualTo(expected));
		}

		private static IAttribute GetAttributeFromMethod(string name, int index)
		{
			return CecilProjectTest.CreateProject().FindMethod(name).Attributes.ToArray()[index];
		}

		private static IAttribute GetAttributeFromType(string name, int index)
		{
			return CecilProjectTest.CreateProject().FindType(name).Attributes.ToArray()[index];
		}

		[CompilerGenerated]
		[Obsolete]
		private interface ICecilAttributeType
		{
		}

		private abstract class TestClass
		{
			[CompilerGenerated]
			[Obsolete]
			protected abstract void CecilAttributeType();
		}
	}
}