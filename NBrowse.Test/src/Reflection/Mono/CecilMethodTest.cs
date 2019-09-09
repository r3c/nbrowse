using System;
using System.Linq;
using NBrowse.Reflection;
using NUnit.Framework;

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
		[TestCase("CecilMethodTest+TestClass..ctor", NBrowse.Binding.Constructor)]
		[TestCase("CecilMethodBindingInstance", NBrowse.Binding.Instance)]
		[TestCase("CecilMethodBindingStatic", NBrowse.Binding.Static)]
		public void Binding(string name, Binding expected)
		{
			Assert.That(CecilMethodTest.GetMethod(name).Binding, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilMethodImplementationAbstract", NBrowse.Implementation.Abstract)]
		[TestCase("CecilMethodImplementationConcrete", NBrowse.Implementation.Concrete)]
		[TestCase("CecilMethodTest+TestClass.GetHashCode", NBrowse.Implementation.Final)]
		[TestCase("CecilMethodImplementationVirtual", NBrowse.Implementation.Virtual)]
		public void Implementation(string name, Implementation expected)
		{
			Assert.That(CecilMethodTest.GetMethod(name).Implementation, Is.EqualTo(expected));
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
		[TestCase("CecilMethodVisibilityInternal", NBrowse.Visibility.Internal)]
		[TestCase("CecilMethodVisibilityPrivate", NBrowse.Visibility.Private)]
		[TestCase("CecilMethodVisibilityProtected", NBrowse.Visibility.Protected)]
		[TestCase("CecilMethodVisibilityPublic", NBrowse.Visibility.Public)]
		public void Visibility(string name, Visibility expected)
		{
			Assert.That(CecilMethodTest.GetMethod(name).Visibility, Is.EqualTo(expected));
		}

		[Test]
		public void IsUsing()
		{
			var caller = CecilMethodTest.GetMethod("CecilMethodIsUsingCaller");
			var useAsArgument = CecilMethodTest.GetMethod("CecilMethodIsUsingCalleeArgument");
			var useAsInvoke = CecilMethodTest.GetMethod("CecilMethodIsUsingCalleeInvoke");
			var noUse = CecilMethodTest.GetMethod("CecilMethodIsUsingDummy");

			Assert.That(caller.IsUsing(useAsArgument), Is.True);
			Assert.That(caller.IsUsing(useAsInvoke), Is.True);
			Assert.That(caller.IsUsing(noUse), Is.False);
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

			protected abstract void CecilMethodImplementationAbstract();

			protected void CecilMethodImplementationConcrete()
			{
			}

			protected virtual void CecilMethodImplementationVirtual()
			{
			}

			protected void CecilMethodIsUsingCaller()
			{
				this.CecilMethodIsUsingCalleeInvoke();

				Action action = this.CecilMethodIsUsingCalleeArgument;

				if (action == null)
					throw new Exception();
			}

			protected abstract void CecilMethodIsUsingCalleeArgument();
			protected abstract void CecilMethodIsUsingCalleeInvoke();
			protected abstract void CecilMethodIsUsingDummy();

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