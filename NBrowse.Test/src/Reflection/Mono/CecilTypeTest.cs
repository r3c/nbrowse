using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using NBrowse.Reflection;
using NUnit.Framework;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedTypeParameter

namespace NBrowse.Test.Reflection.Mono
{
	public class CecilTypeTest
	{
		[Test]
		[TestCase("CecilTypeAttributes0", "")]
		[TestCase("CecilTypeAttributes1", "ObsoleteAttribute")]
		[TestCase("CecilTypeAttributes2", "CompilerGeneratedAttribute,ObsoleteAttribute")]
		public void Attributes(string name, string expected)
		{
			Assert.That(string.Join(",", CecilTypeTest.GetType(name).Attributes.Select(a => a.Type.Name)),
				Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilTypeBaseOrNullIsDefined", "Stream")]
		public void BaseOrNullIsDefined(string name, string expected)
		{
			Assert.That(CecilTypeTest.GetType(name).BaseOrNull, Is.Not.Null);
			Assert.That(CecilTypeTest.GetType(name).BaseOrNull.Name, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilTypeBaseOrNullIsObject")]
		public void BaseOrNullIsNull(string name)
		{
			Assert.That(CecilTypeTest.GetType(name).BaseOrNull, Is.Not.Null);
			Assert.That(CecilTypeTest.GetType(name).BaseOrNull.Name, Is.EqualTo(nameof(Object)));
		}

		[Test]
		[TestCase("CecilTypeFields0", "")]
		[TestCase("CecilTypeFields1", "Field1")]
		[TestCase("CecilTypeFields2", "Field1,Field2")]
		public void Fields(string name, string expected)
		{
			Assert.That(string.Join(",", CecilTypeTest.GetType(name).Fields.Select(f => f.Name)),
				Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilTypeImplementationAbstract", NBrowse.Implementation.Abstract)]
		[TestCase("CecilTypeImplementationFinal", NBrowse.Implementation.Final)]
		[TestCase("CecilTypeImplementationVirtual", NBrowse.Implementation.Virtual)]
		public void Implementation(string name, Implementation expected)
		{
			Assert.That(CecilTypeTest.GetType(name).Implementation, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilTypeInterfaces0", "")]
		[TestCase("CecilTypeInterfaces1", "IDisposable")]
		[TestCase("CecilTypeInterfaces2", "IDisposable,ISerializable")]
		public void Interfaces(string name, string expected)
		{
			Assert.That(string.Join(",", CecilTypeTest.GetType(name).Interfaces.Select(i => i.Name)),
				Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilTypeMethods0", ".ctor")]
		[TestCase("CecilTypeMethods1", "Method1,.ctor")]
		[TestCase("CecilTypeMethods2", "Method1,Method2,.ctor")]
		public void Methods(string name, string expected)
		{
			Assert.That(string.Join(",", CecilTypeTest.GetType(name).Methods.Select(m => m.Name)),
				Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilTypeModelClass", NBrowse.Model.Class)]
		[TestCase("CecilTypeModelEnumeration", NBrowse.Model.Enumeration)]
		[TestCase("ICecilTypeModelInterface", NBrowse.Model.Interface)]
		[TestCase("CecilTypeModelStructure", NBrowse.Model.Structure)]
		public void Model(string name, Model expected)
		{
			Assert.That(CecilTypeTest.GetType(name).Model, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilTypeName")]
		public void Name(string name)
		{
			Assert.That(CecilTypeTest.GetType(name).Name, Is.EqualTo($"{nameof(CecilTypeTest)}+{name}"));
		}

		[Test]
		[TestCase("CecilTypeNamespace")]
		public void Namespace(string name)
		{
			Assert.That(CecilTypeTest.GetType(name).Namespace, Is.EqualTo(typeof(CecilTypeTest).Namespace));
		}

		[Test]
		[TestCase("CecilTypeNestedTypes0", "")]
		[TestCase("CecilTypeNestedTypes1", "CecilTypeTest+CecilTypeNestedTypes1+NestedType1")]
		[TestCase("CecilTypeNestedTypes2", "CecilTypeTest+CecilTypeNestedTypes2+NestedType1,CecilTypeTest+CecilTypeNestedTypes2+NestedType2")]
		public void NestedTypes(string name, string expected)
		{
			Assert.That(string.Join(",", CecilTypeTest.GetType(name).NestedTypes.Select(n => n.Name)),
				Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilTypeParameters0", "")]
		[TestCase("CecilTypeParameters1", "TParameter1")]
		[TestCase("CecilTypeParameters2", "TParameter1,TParameter2")]
		public void Parameters(string name, string expected)
		{
			Assert.That(string.Join(",", CecilTypeTest.GetType(name).Parameters.Select(p => p.Name)),
				Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilTypeParent", "NBrowse.Test")]
		public void Parent(string name, string expected)
		{
			Assert.That(CecilTypeTest.GetType(name).Parent.Name, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilTypeVisibilityInternal", NBrowse.Visibility.Internal)]
		[TestCase("CecilTypeVisibilityPrivate", NBrowse.Visibility.Private)]
		[TestCase("CecilTypeVisibilityProtected", NBrowse.Visibility.Protected)]
		[TestCase("CecilTypeVisibilityPublic", NBrowse.Visibility.Public)]
		public void Visibility(string name, Visibility expected)
		{
			Assert.That(CecilTypeTest.GetType(name).Visibility, Is.EqualTo(expected));
		}

		private static IType GetType(string name)
		{
			return CecilProjectTest.CreateProject().FindType($"{nameof(CecilTypeTest)}+{name}");
		}

#pragma warning disable 649
		private class CecilTypeAttributes0
		{
		}

		[Obsolete]
		private class CecilTypeAttributes1
		{
		}

		[CompilerGenerated]
		[Obsolete]
		private class CecilTypeAttributes2
		{
		}

		private abstract class CecilTypeBaseOrNullIsDefined : Stream
		{
		}

		private class CecilTypeBaseOrNullIsObject
		{
		}

		private class CecilTypeFields0
		{
		}

		private class CecilTypeFields1
		{
			public int Field1;
		}

		private class CecilTypeFields2
		{
			public int Field1;
			public int Field2;
		}

		private abstract class CecilTypeImplementationAbstract
		{
		}

		private class CecilTypeImplementationVirtual
		{
		}

		private sealed class CecilTypeImplementationFinal
		{
		}

		private class CecilTypeInterfaces0
		{
		}

		private class CecilTypeInterfaces1 : IDisposable
		{
			public void Dispose()
			{
			}
		}

		private class CecilTypeInterfaces2 : IDisposable, ISerializable
		{
			public void Dispose()
			{
				throw new NotImplementedException();
			}

			public void GetObjectData(SerializationInfo info, StreamingContext context)
			{
				throw new NotImplementedException();
			}
		}

		private class CecilTypeMethods0
		{
		}

		private abstract class CecilTypeMethods1
		{
			public abstract void Method1();
		}

		private abstract class CecilTypeMethods2
		{
			public abstract void Method1();
			public abstract void Method2();
		}

		private class CecilTypeModelClass
		{
		}

		private enum CecilTypeModelEnumeration
		{
		}

		private interface ICecilTypeModelInterface
		{
		}

		private struct CecilTypeModelStructure
		{
		}

		private class CecilTypeName
		{
		}

		private class CecilTypeNamespace
		{
		}

		private class CecilTypeNestedTypes0
		{
		}

		private class CecilTypeNestedTypes1
		{
			private class NestedType1
			{
			}
		}

		private class CecilTypeNestedTypes2
		{
			private class NestedType1
			{
			}

			private class NestedType2
			{
			}
		}

		private class CecilTypeParameters0
		{
		}

		private class CecilTypeParameters1<TParameter1>
		{
		}

		private class CecilTypeParameters2<TParameter1, TParameter2>
		{
		}

		private class CecilTypeParent
		{
		}

		internal class CecilTypeVisibilityInternal
		{
		}

		private class CecilTypeVisibilityPrivate
		{
		}

		protected class CecilTypeVisibilityProtected
		{
		}

		public class CecilTypeVisibilityPublic
		{
		}
#pragma warning restore 649
	}
}