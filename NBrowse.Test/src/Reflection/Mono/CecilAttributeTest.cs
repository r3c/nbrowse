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
		[TestCase(nameof(TestClass.CecilAttributeArguments), 1, 0, "argument0")]
		[TestCase(nameof(TestClass.CecilAttributeArguments), 1, 1, "argument1")]
		public void Arguments_List(string name, int attributeIndex, int argumentIndex, object expected)
		{
			Assert.That(
				CecilAttributeTest.GetAttributeFromMethod(name, attributeIndex).Arguments.ToList()[argumentIndex],
				Is.EqualTo(expected));
		}

		[Test]
		[TestCase(nameof(TestClass.CecilAttributeArguments), 0, 0, false)]
		public void Arguments_None(string name, int attributeIndex, int argumentIndex, object expected)
		{
			Assert.That(CecilAttributeTest.GetAttributeFromMethod(name, attributeIndex).Arguments, Is.Empty);
		}

		[Test]
		[TestCase(nameof(TestClass.CecilAttributeConstructor), 0, ".ctor")]
		public void Constructor(string name, int index, string expected)
		{
			Assert.That(CecilAttributeTest.GetAttributeFromMethod(name, index).Constructor.Name, Is.EqualTo(expected));
		}

		[Test]
		[TestCase(nameof(TestClass.CecilAttributeArguments), 0, nameof(TestClass.CecilAttributeArguments), 0, true)]
		[TestCase(nameof(TestClass.CecilAttributeArguments), 0, nameof(TestClass.CecilAttributeArguments), 1, false)]
		[TestCase(nameof(TestClass.CecilAttributeType), 0, nameof(TestClass.CecilAttributeArguments), 0, false)]
		public void Equals(string name1, int index1, string name2, int index2, bool expected)
		{
			var attribute1 = CecilAttributeTest.GetAttributeFromMethod(name1, index1);
			var attribute2 = CecilAttributeTest.GetAttributeFromMethod(name2, index2);

			Assert.That(attribute1.Equals(attribute2), Is.EqualTo(expected));
			Assert.That(attribute1 == attribute2, Is.EqualTo(expected));
			Assert.That(attribute1 != attribute2, Is.EqualTo(!expected));
		}

		[Test]
		[TestCase(nameof(TestClass.CecilAttributeType), 0, "CompilerGeneratedAttribute")]
		[TestCase(nameof(TestClass.CecilAttributeType), 1, "DescriptionAttribute")]
		public void TypeOfMethodAttribute(string name, int index, string expected)
		{
			Assert.That(CecilAttributeTest.GetAttributeFromMethod(name, index).Type.Name, Is.EqualTo(expected));
		}

		[Test]
		[TestCase(nameof(CecilAttributeTest) + "+" + nameof(AttributeClass), 0, "CompilerGeneratedAttribute")]
		[TestCase(nameof(CecilAttributeTest) + "+" + nameof(AttributeClass), 1, "DescriptionAttribute")]
		public void TypeOfTypeAttribute(string name, int index, string expected)
		{
			Assert.That(CecilAttributeTest.GetAttributeFromType(name, index).Type.Name, Is.EqualTo(expected));
		}

		private static Attribute GetAttributeFromMethod(string name, int index)
		{
			return CecilProjectTest.CreateProject().FindMethod(name).Attributes.ToArray()[index];
		}

		private static Attribute GetAttributeFromType(string name, int index)
		{
			return CecilProjectTest.CreateProject().FindType(name).Attributes.ToArray()[index];
		}

		[CompilerGenerated]
		[Description("attribute class")]
		private static class AttributeClass
		{
		}

		private abstract class TestClass
		{
			[System.STAThread]
			[Author("argument0", "argument1")]
			public abstract void CecilAttributeArguments();

			[Description("constructor")]
			public abstract void CecilAttributeConstructor();

			[CompilerGenerated]
			[Description("type")]
			public abstract void CecilAttributeType();
		}
	}
}