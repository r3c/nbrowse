using System;
using System.Linq;
using NBrowse.Reflection;
using NUnit.Framework;

namespace NBrowse.Test.Reflection.Mono
{
	public class CecilFieldTest
	{
		[Test]
		[TestCase("CecilFieldBindingInstance", NBrowse.Reflection.Binding.Instance)]
		[TestCase("CecilFieldBindingStatic", NBrowse.Reflection.Binding.Static)]
		public void Binding(string name, Binding expected)
		{
			Assert.That(CecilFieldTest.GetField(name).Binding, Is.EqualTo(expected));			
		}

		[Test]
		[TestCase("CecilFieldName")]
		public void Name(string name)
		{
			Assert.That(CecilFieldTest.GetField(name).Name, Is.EqualTo(name));
		}

		[Test]
		[TestCase("CecilFieldParent", "CecilFieldTest+TestClass")]
		public void Parent(string name, string expected)
		{
			Assert.That(CecilFieldTest.GetField(name).Parent.Name, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilFieldTypeInt32", nameof(Int32))]
		[TestCase("CecilFieldTypeString", nameof(String))]
		public void Type(string name, string expected)
		{
			Assert.That(CecilFieldTest.GetField(name).Type.Name, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilFieldVisibilityInternal", NBrowse.Reflection.Visibility.Internal)]
		[TestCase("cecilFieldVisibilityPrivate", NBrowse.Reflection.Visibility.Private)]
		[TestCase("CecilFieldVisibilityProtected", NBrowse.Reflection.Visibility.Protected)]
		[TestCase("CecilFieldVisibilityPublic", NBrowse.Reflection.Visibility.Public)]
		public void Visibility(string name, Visibility expected)
		{
			Assert.That(CecilFieldTest.GetField(name).Visibility, Is.EqualTo(expected));
		}

		private static IField GetField(string name)
		{
			return CecilProjectTest.CreateProject().FindType($"{nameof(CecilFieldTest)}+{nameof(TestClass)}").Fields
				.Single(f => f.Name == name);
		}

		private abstract class TestClass
		{
#pragma warning disable 169
#pragma warning disable 649
			public int CecilFieldBindingInstance;

			public static int CecilFieldBindingStatic;

			public int CecilFieldName;

			public int CecilFieldParent;

			public int CecilFieldTypeInt32;
			public string CecilFieldTypeString;

			internal int CecilFieldVisibilityInternal;
			private int cecilFieldVisibilityPrivate;
			protected int CecilFieldVisibilityProtected;
			public int CecilFieldVisibilityPublic;
#pragma warning restore 169
#pragma warning restore 649
		}
	}
}