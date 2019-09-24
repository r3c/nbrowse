using System;
using System.Linq;
using NBrowse.Reflection;
using NUnit.Framework;

// ReSharper disable ConvertToConstant.Local
// ReSharper disable UnusedMember.Global

namespace NBrowse.Test.Reflection.Mono
{
	public class CecilMethodTest
	{
		[Test]
		[TestCase("CecilMethodArguments0", "")]
		[TestCase("CecilMethodArguments1", "a")]
		[TestCase("CecilMethodArguments2", "a,b")]
		public void Arguments(string name, string expected)
		{
			var arguments = string.Join(",", CecilMethodTest.GetMethod(name).Arguments.Select(a => a.Name));

			Assert.That(arguments, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilMethodAttributes0", "")]
		[TestCase("CecilMethodAttributes1", "ObsoleteAttribute")]
		[TestCase("CecilMethodAttributes2", "MTAThreadAttribute,STAThreadAttribute")]
		public void Attributes(string name, string expected)
		{
			var attributes = string.Join(",", CecilMethodTest.GetMethod(name).Attributes.Select(a => a.Type.Name));

			Assert.That(attributes, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilMethodTest+TestClass..ctor", NBrowse.Reflection.Binding.Constructor)]
		[TestCase("CecilMethodBindingInstance", NBrowse.Reflection.Binding.Instance)]
		[TestCase("CecilMethodBindingStatic", NBrowse.Reflection.Binding.Static)]
		public void Binding(string name, Binding expected)
		{
			Assert.That(CecilMethodTest.GetMethod(name).Binding, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilMethodDefinitionAbstract", NBrowse.Reflection.Definition.Abstract)]
		[TestCase("CecilMethodDefinitionConcrete", NBrowse.Reflection.Definition.Concrete)]
		[TestCase("CecilMethodTest+TestClass.GetHashCode", NBrowse.Reflection.Definition.Final)]
		[TestCase("CecilMethodDefinitionVirtual", NBrowse.Reflection.Definition.Virtual)]
		public void Definition(string name, Definition expected)
		{
			Assert.That(CecilMethodTest.GetMethod(name).Definition, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilMethodName", "CecilMethodName")]
		public void Name(string name, string expected)
		{
			Assert.That(CecilMethodTest.GetMethod(name).Name, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilMethodParameters0", "")]
		[TestCase("CecilMethodParameters1", "TParameter1")]
		[TestCase("CecilMethodParameters2", "TParameter1,TParameter2")]
		public void Parameters(string name, string expected)
		{
			var attributes = string.Join(",", CecilMethodTest.GetMethod(name).Parameters.Select(p => p.Name));

			Assert.That(attributes, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilMethodParent", "CecilMethodTest+TestClass")]
		public void Parent(string name, string expected)
		{
			Assert.That(CecilMethodTest.GetMethod(name).Parent.Name, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilMethodReturnTypeInt32", nameof(Int32))]
		[TestCase("CecilMethodReturnTypeVoid", "Void")]
		public void ReturnType(string name, string expected)
		{
			Assert.That(CecilMethodTest.GetMethod(name).ReturnType.Name, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilMethodVisibilityInternal", NBrowse.Reflection.Visibility.Internal)]
		[TestCase("CecilMethodVisibilityPrivate", NBrowse.Reflection.Visibility.Private)]
		[TestCase("CecilMethodVisibilityProtected", NBrowse.Reflection.Visibility.Protected)]
		[TestCase("CecilMethodVisibilityPublic", NBrowse.Reflection.Visibility.Public)]
		public void Visibility(string name, Visibility expected)
		{
			Assert.That(CecilMethodTest.GetMethod(name).Visibility, Is.EqualTo(expected));
		}

		private static IMethod GetMethod(string name)
		{
			return CecilProjectTest.CreateProject().FindMethod(name);
		}

		public abstract class TestClass
		{
			protected abstract void CecilMethodArguments0();
			protected abstract void CecilMethodArguments1(int a);
			protected abstract void CecilMethodArguments2(int a, int b);

			protected abstract void CecilMethodAttributes0();

			[Obsolete]
			protected abstract void CecilMethodAttributes1();

			[MTAThread]
			[STAThread]
			protected abstract void CecilMethodAttributes2();

			protected abstract void CecilMethodBindingInstance();

			protected static void CecilMethodBindingStatic()
			{
			}

			protected abstract void CecilMethodDefinitionAbstract();

			protected void CecilMethodDefinitionConcrete()
			{
			}

			protected virtual void CecilMethodDefinitionVirtual()
			{
			}

			protected abstract void CecilMethodName();

			protected abstract void CecilMethodParameters0();
			protected abstract void CecilMethodParameters1<TParameter1>();
			protected abstract void CecilMethodParameters2<TParameter1, TParameter2>();
			protected abstract void CecilMethodParent();
			protected abstract int CecilMethodReturnTypeInt32();
			protected abstract void CecilMethodReturnTypeVoid();

			internal abstract void CecilMethodVisibilityInternal();

			// ReSharper disable once UnusedMember.Local
			private void CecilMethodVisibilityPrivate()
			{
			}

			protected abstract void CecilMethodVisibilityProtected();
			public abstract void CecilMethodVisibilityPublic();

			public sealed override int GetHashCode()
			{
				return 0;
			}
		}
	}
}