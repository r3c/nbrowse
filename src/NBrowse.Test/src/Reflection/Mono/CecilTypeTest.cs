using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using NBrowse.Reflection;
using NUnit.Framework;

// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedType.Local
// ReSharper disable UnusedTypeParameter

namespace NBrowse.Test.Reflection.Mono
{
    public class CecilTypeTest
    {
        [Test]
        [TestCase(nameof(CecilTypeAttributes0), "")]
        [TestCase(nameof(CecilTypeAttributes1), "DescriptionAttribute")]
        [TestCase(nameof(CecilTypeAttributes2), "CompilerGeneratedAttribute,DescriptionAttribute")]
        public void Attributes(string name, string expected)
        {
            Assert.That(string.Join(",", CecilTypeTest.GetType(name).Attributes.Select(a => a.Type.Name)),
                Is.EqualTo(expected));
        }

        [Test]
        [TestCase("MemberTypeParameters0", "")]
        [TestCase("MemberTypeParameters1", "CecilTypeTest+CecilMemberTypeParameters`2+TParameter1")]
        [TestCase("MemberTypeParameters2", "CecilTypeTest+CecilMemberTypeParameters`2+TParameter1,CecilTypeTest+CecilMemberTypeParameters`2+TParameter2")]
        public void Arguments(string fieldName, string expected)
        {
            var fieldType = CecilTypeTest.GetType("CecilMemberTypeParameters").Fields.First(field => field.Name == fieldName).Type;
            Assert.That(string.Join(",", fieldType.Arguments.Select(p => p.Name)),
                Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(CecilTypeBaseOrNullIsDefined), "Stream")]
        public void BaseOrNullIsDefined(string name, string expected)
        {
            Assert.That(CecilTypeTest.GetType(name).BaseOrNull, Is.Not.Null);
            Assert.That(CecilTypeTest.GetType(name).BaseOrNull.Name, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(CecilTypeBaseOrNullIsObject))]
        public void BaseOrNullIsNull(string name)
        {
            Assert.That(CecilTypeTest.GetType(name).BaseOrNull, Is.Not.Null);
            Assert.That(CecilTypeTest.GetType(name).BaseOrNull.Name, Is.EqualTo(nameof(System.Object)));
        }

        [Test]
        [TestCase(0, true, nameof(System.Boolean))]
        [TestCase(1, true, nameof(System.Int32))]
        [TestCase(2, true, nameof(System.Single))]
        [TestCase(3, false, "")]
        public void ElementOrNull(int index, bool defined, string expected)
        {
            var type = CecilTypeTest.GetType(nameof(CecilTypeElementOrNull));
            var method = type.Methods.Single(m => m.Definition == Definition.Abstract);
            var argument = method.Arguments.ToArray()[index];

            Assert.That(argument.Type.ElementOrNull, defined ? Is.Not.Null : Is.Null);
            Assert.That(argument.Type.ElementOrNull?.Name ?? string.Empty, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(CecilTypeAttributes0), nameof(CecilTypeAttributes0), true)]
        [TestCase(nameof(CecilTypeAttributes0), nameof(CecilTypeAttributes1), false)]
        public void Equals(string name1, string name2, bool expected)
        {
            var type1 = CecilTypeTest.GetType(name1);
            var type2 = CecilTypeTest.GetType(name2);

            Assert.That(type1.Equals(type2), Is.EqualTo(expected));
            Assert.That(type1 == type2, Is.EqualTo(expected));
            Assert.That(type1 != type2, Is.EqualTo(!expected));
        }

        [Test]
        [TestCase(nameof(CecilTypeFields0), "")]
        [TestCase(nameof(CecilTypeFields1), "Field1")]
        [TestCase(nameof(CecilTypeFields2), "Field1,Field2")]
        public void Fields(string name, string expected)
        {
            Assert.That(string.Join(",", CecilTypeTest.GetType(name).Fields.Select(f => f.Name)),
                Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(CecilTypeImplementationAbstract), Definition.Abstract)]
        [TestCase(nameof(CecilTypeImplementationFinal), Definition.Final)]
        [TestCase(nameof(CecilTypeImplementationVirtual), Definition.Virtual)]
        public void Implementation(string name, Definition expected)
        {
            Assert.That(CecilTypeTest.GetType(name).Definition, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(CecilTypeInterfaces0), "")]
        [TestCase(nameof(CecilTypeInterfaces1), "IDisposable")]
        [TestCase(nameof(CecilTypeInterfaces2), "IDisposable,ISerializable")]
        public void Interfaces(string name, string expected)
        {
            Assert.That(string.Join(",", CecilTypeTest.GetType(name).Interfaces.Select(i => i.Name)),
                Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(CecilTypeMethods0), ".ctor")]
        [TestCase(nameof(CecilTypeMethods1), "Method1,.ctor")]
        [TestCase(nameof(CecilTypeMethods2), "Method1,Method2,.ctor")]
        public void Methods(string name, string expected)
        {
            Assert.That(string.Join(",", CecilTypeTest.GetType(name).Methods.Select(m => m.Name)),
                Is.EqualTo(expected));
        }

        [Test]
        [TestCase(0, NBrowse.Reflection.Model.Array)]
        [TestCase(1, NBrowse.Reflection.Model.Class)]
        [TestCase(2, NBrowse.Reflection.Model.Enumeration)]
        [TestCase(3, NBrowse.Reflection.Model.Interface)]
        [TestCase(4, NBrowse.Reflection.Model.Pointer)]
        [TestCase(5, NBrowse.Reflection.Model.Reference)]
        [TestCase(6, NBrowse.Reflection.Model.Structure)]
        public void Model(int index, Model expected)
        {
            var type = CecilTypeTest.GetType(nameof(CecilTypeModel));
            var method = type.Methods.Single(m => m.Definition == Definition.Abstract);
            var argument = method.Arguments.ToArray()[index];

            Assert.That(argument.Type.Model, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(CecilTypeName))]
        public void Name(string name)
        {
            Assert.That(CecilTypeTest.GetType(name).Name, Is.EqualTo($"{nameof(CecilTypeTest)}+{name}"));
        }

        [Test]
        [TestCase(nameof(CecilTypeNamespace))]
        public void Namespace(string name)
        {
            Assert.That(CecilTypeTest.GetType(name).Namespace, Is.EqualTo(typeof(CecilTypeTest).Namespace));
        }

        [Test]
        [TestCase(nameof(CecilTypeNestedTypes0), "")]
        [TestCase(nameof(CecilTypeNestedTypes1), "CecilTypeTest+CecilTypeNestedTypes1+NestedType1")]
        [TestCase(nameof(CecilTypeNestedTypes2), "CecilTypeTest+CecilTypeNestedTypes2+NestedType1,CecilTypeTest+CecilTypeNestedTypes2+NestedType2")]
        public void NestedTypes(string name, string expected)
        {
            Assert.That(string.Join(",", CecilTypeTest.GetType(name).NestedTypes.Select(n => n.Name)),
                Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(CecilTypeParameters0), "")]
        [TestCase("CecilTypeParameters1", "TParameter1")]
        [TestCase("CecilTypeParameters2", "TParameter1,TParameter2")]
        public void Parameters(string name, string expected)
        {
            Assert.That(string.Join(",", CecilTypeTest.GetType(name).Parameters.Select(p => p.Name)),
                Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(CecilTypeParent), "NBrowse.Test")]
        public void Parent(string name, string expected)
        {
            Assert.That(CecilTypeTest.GetType(name).Parent.Name, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(CecilTypeVisibilityInternal), NBrowse.Reflection.Visibility.Internal)]
        [TestCase(nameof(CecilTypeVisibilityPrivate), NBrowse.Reflection.Visibility.Private)]
        [TestCase(nameof(CecilTypeVisibilityProtected), NBrowse.Reflection.Visibility.Protected)]
        [TestCase(nameof(CecilTypeVisibilityPublic), NBrowse.Reflection.Visibility.Public)]
        public void Visibility(string name, Visibility expected)
        {
            Assert.That(CecilTypeTest.GetType(name).Visibility, Is.EqualTo(expected));
        }

        private static Method GetMethod(string name)
        {
            return CecilProjectTest.CreateProject().FindMethod(name);
        }

        private static Type GetType(string name)
        {
            return CecilProjectTest.CreateProject().FindType($"{nameof(CecilTypeTest)}+{name}");
        }

        private static class CecilTypeAttributes0
        {
        }

        [Description("attribute1")]
        private static class CecilTypeAttributes1
        {
        }

        [CompilerGenerated]
        [Description("attribute2")]
        private static class CecilTypeAttributes2
        {
        }

        private abstract class CecilTypeBaseOrNullIsDefined : Stream
        {
        }

        private static class CecilTypeBaseOrNullIsObject
        {
        }

        private abstract class CecilTypeElementOrNull
        {
            public abstract unsafe void Method(bool[] array, int* pointer, ref float reference, bool none);
        }

        private static class CecilTypeFields0
        {
        }

#pragma warning disable 649
        private class CecilTypeFields1
        {
            public int Field1;
        }

        private class CecilTypeFields2
        {
            public int Field1;
            public int Field2;
        }
#pragma warning restore 649

        private abstract class CecilTypeImplementationAbstract
        {
        }

        private class CecilTypeImplementationVirtual
        {
        }

        private sealed class CecilTypeImplementationFinal
        {
        }

        private static class CecilTypeInterfaces0
        {
        }

        private class CecilTypeInterfaces1 : System.IDisposable
        {
            public void Dispose()
            {
            }
        }

        private class CecilTypeInterfaces2 : System.IDisposable, ISerializable
        {
            public void Dispose()
            {
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
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

        private abstract class CecilTypeModel
        {
            public abstract unsafe void Method(int[] array, object classType, System.DateTimeKind enumeration,
                System.IDisposable iface, int* pointer, ref int reference, bool structure);
        }

        private static class CecilTypeName
        {
        }

        private static class CecilTypeNamespace
        {
        }

        private static class CecilTypeNestedTypes0
        {
        }

        private static class CecilTypeNestedTypes1
        {
            private class NestedType1
            {
            }
        }

        private static class CecilTypeNestedTypes2
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

        private class CecilMemberTypeParameters<TParameter1, TParameter2>
        {
#pragma warning disable CS0649
            public CecilTypeParameters0 MemberTypeParameters0;
            public CecilTypeParameters1<TParameter1> MemberTypeParameters1;
            public CecilTypeParameters2<TParameter1, TParameter2> MemberTypeParameters2;
#pragma warning restore CS0649
        }

        private static class CecilTypeParent
        {
        }

        internal static class CecilTypeVisibilityInternal
        {
        }

        private static class CecilTypeVisibilityPrivate
        {
        }

        protected static class CecilTypeVisibilityProtected
        {
        }

        public static class CecilTypeVisibilityPublic
        {
        }
    }
}