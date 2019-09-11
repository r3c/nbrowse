using System;
using NBrowse.Reflection;
using NBrowse.Test.Reflection.Mono;
using NUnit.Framework;

namespace NBrowse.Test.Selection
{
	public class HasTest
	{
		[Test]
		[TestCase("HasObsoleteAttribute", true)]
		[TestCase("HasNoObsoleteAttribute", false)]
		public void AttributeOnMethod(string name, bool expected)
		{
			var method = HasTest.GetMethod(name);

			Assert.That(NBrowse.Selection.Has.Attribute<ObsoleteAttribute>(method), Is.EqualTo(expected));
		}

		[Test]
		[TestCase("HasTest+ObsoleteStructure", true)]
		[TestCase("HasTest+TestStructure", false)]
		public void AttributeOnType(string name, bool expected)
		{
			var type = HasTest.GetType(name);

			Assert.That(NBrowse.Selection.Has.Attribute<ObsoleteAttribute>(type), Is.EqualTo(expected));
		}

		private static IMethod GetMethod(string name)
		{
			return CecilProjectTest.CreateProject().FindMethod(name);
		}

		private static IType GetType(string name)
		{
			return CecilProjectTest.CreateProject().FindType(name);
		}

		[Obsolete]
		private struct ObsoleteStructure
		{
		}

		private abstract class TestStructure
		{
			[Obsolete]
			public abstract void HasObsoleteAttribute();

			public abstract void HasNoObsoleteAttribute();
		}
	}
}