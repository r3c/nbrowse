using System.Runtime.CompilerServices;
using NBrowse.Reflection;
using NBrowse.Test.Reflection.Mono;
using NUnit.Framework;

namespace NBrowse.Test.Selection
{
	public class IsTest
	{
		[Test]
		[TestCase("IsGenerated", true)]
		[TestCase("IsNotGenerated", false)]
		public void AttributeOnMethod(string name, bool expected)
		{
			var method = IsTest.GetMethod(name);

			Assert.That(NBrowse.Selection.Is.Generated(method), Is.EqualTo(expected));
		}

		[Test]
		[TestCase("IsTest+IsGeneratedStructure", true)]
		[TestCase("IsTest+IsNotGeneratedStructure", false)]
		public void AttributeOnType(string name, bool expected)
		{
			var type = IsTest.GetType(name);

			Assert.That(NBrowse.Selection.Is.Generated(type), Is.EqualTo(expected));
		}

		private static IMethod GetMethod(string name)
		{
			return CecilProjectTest.CreateProject().FindMethod(name);
		}

		private static IType GetType(string name)
		{
			return CecilProjectTest.CreateProject().FindType(name);
		}

		[CompilerGenerated]
		private struct IsGeneratedStructure
		{
		}

		private abstract class IsNotGeneratedStructure
		{
			[CompilerGenerated]
			public abstract void IsGenerated();

			public abstract void IsNotGenerated();
		}
	}
}