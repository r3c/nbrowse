using System;
using NBrowse.Reflection;
using NUnit.Framework;

// ReSharper disable UnusedMember.Global

namespace NBrowse.Test.Reflection.Mono
{
	public class CecilImplementationTest
	{
		[Test]
		public void Parent()
		{
			var caller = CecilImplementationTest.GetMethod("CecilImplementationParent");

			Assert.That(caller.Implementation, Is.Not.Null);
			Assert.That(caller.Implementation.Parent, Is.EqualTo(caller));
		}

		[Test]
		[TestCase("CecilImplementationReferencedMethodsInvoke", true)]
		[TestCase("CecilImplementationReferencedMethodsNothing", false)]
		[TestCase("CecilImplementationReferencedMethodsValue", true)]
		public void ReferencedMethods(string name, bool expected)
		{
			var caller = CecilImplementationTest.GetMethod("CecilImplementationReferencedMethods");
			var callee = CecilImplementationTest.GetMethod(name);

			Assert.That(caller.Implementation, Is.Not.Null);
			Assert.That(caller.Implementation.ReferencedMethods,
				expected ? Does.Contain(callee) : Does.Not.Contain(callee));
		}

		[Test]
		[TestCase("CecilImplementationReferencedTypesArgument", true)]
		[TestCase("CecilImplementationReferencedTypesConstructorReference", true)]
		[TestCase("CecilImplementationReferencedTypesConstructorValue", true)]
		[TestCase("CecilImplementationReferencedTypesField", true)]
		[TestCase("CecilImplementationReferencedTypesNothing", false)]
		[TestCase("CecilImplementationReferencedTypesProperty", true)]
		[TestCase("CecilImplementationReferencedTypesReturn", true)]
		public void ReferencedTypes(string name, bool expected)
		{
			var caller = CecilImplementationTest.GetMethod("CecilImplementationReferencedTypes");
			var type = CecilImplementationTest.GetType(name);

			Assert.That(caller.Implementation, Is.Not.Null);
			Assert.That(caller.Implementation.ReferencedTypes, expected ? Does.Contain(type) : Does.Not.Contain(type));
		}

		private static Method GetMethod(string name)
		{
			return CecilProjectTest.CreateProject().FindMethod(name);
		}

		private static NBrowse.Reflection.Type GetType(string name)
		{
			return CecilProjectTest.CreateProject().FindType($"{nameof(CecilImplementationTest)}+{name}");
		}

		protected CecilImplementationReferencedTypesReturn CecilImplementationReferencedTypes(CecilImplementationReferencedTypesArgument a)
		{
			var b = new CecilImplementationReferencedTypesConstructorReference();
			var c = new CecilImplementationReferencedTypesConstructorValue();
			var d = CecilImplementationReferencedTypesField.Field;
			var e = CecilImplementationReferencedTypesProperty.Property;

			Console.WriteLine(a.ToString() + b + c + d + e);

			return new CecilImplementationReferencedTypesReturn();
		}

		protected struct CecilImplementationReferencedTypesArgument
		{
		}

		private class CecilImplementationReferencedTypesConstructorReference
		{
		}

		private struct CecilImplementationReferencedTypesConstructorValue
		{
		}

		private struct CecilImplementationReferencedTypesField
		{
			public static readonly long Field = DateTime.UtcNow.Ticks;
		}

		private struct CecilImplementationReferencedTypesProperty
		{
			public static long Property => DateTime.UtcNow.Ticks;
		}

		protected abstract class CecilImplementationReferencedTypesNothing
		{
		}

		protected struct CecilImplementationReferencedTypesReturn
		{
		}

		public abstract class TestClass
		{
			protected void CecilImplementationReferencedMethods()
			{
				this.CecilImplementationReferencedMethodsInvoke();

				Action action = this.CecilImplementationReferencedMethodsValue;

				Console.WriteLine(action);
			}

			protected void CecilImplementationParent()
			{
			}

			protected abstract void CecilImplementationReferencedMethodsInvoke();
			protected abstract void CecilImplementationReferencedMethodsNothing();
			protected abstract void CecilImplementationReferencedMethodsValue();
		}
	}
}