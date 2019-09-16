using System;
using System.Linq;
using System.Runtime.CompilerServices;
using NBrowse.Reflection;
using NUnit.Framework;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NBrowse.Test.Reflection.Mono
{
	public class CecilAttributeTest
	{
		[Test]
		[TestCase("CecilAttributeArguments", 1, 0, "AttributeArgument")]
		[TestCase("CecilAttributeArguments", 1, 1, false)]
		public void ArgumentsList(string name, int attributeIndex, int argumentIndex, object expected)
		{
			Assert.That(
				CecilAttributeTest.GetAttributeFromMethod(name, attributeIndex).Arguments.ToList()[argumentIndex],
				Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilAttributeArguments", 0, 0, false)]
		public void ArgumentsNone(string name, int attributeIndex, int argumentIndex, object expected)
		{
			Assert.That(CecilAttributeTest.GetAttributeFromMethod(name, attributeIndex).Arguments, Is.Empty);
		}

		[Test]
		[TestCase("CecilAttributeConstructor", 0, ".ctor")]
		public void Constructor(string name, int index, string expected)
		{
			Assert.That(CecilAttributeTest.GetAttributeFromMethod(name, index).Constructor.Name, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilAttributeType", 0, "CompilerGeneratedAttribute")]
		[TestCase("CecilAttributeType", 1, "ObsoleteAttribute")]
		public void TypeOfMethodAttribute(string name, int index, string expected)
		{
			Assert.That(CecilAttributeTest.GetAttributeFromMethod(name, index).Type.Name, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilAttributeTest+ICecilAttributeType", 0, "CompilerGeneratedAttribute")]
		[TestCase("CecilAttributeTest+ICecilAttributeType", 1, "ObsoleteAttribute")]
		public void TypeOfTypeAttribute(string name, int index, string expected)
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
			[STAThread]
			[Obsolete("AttributeArgument", false)]
			protected abstract void CecilAttributeArguments();

			[Obsolete]
			protected abstract void CecilAttributeConstructor();

			[CompilerGenerated]
			[Obsolete]
			protected abstract void CecilAttributeType();
		}
	}
}