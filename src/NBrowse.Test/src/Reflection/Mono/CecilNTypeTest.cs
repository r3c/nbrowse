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
    public class CecilNTypeTest
    {
        [Test]
        [TestCase(nameof(TypeAttributes0), "")]
        [TestCase(nameof(TypeAttributes1), "DescriptionAttribute")]
        [TestCase(nameof(TypeAttributes2), "CompilerGeneratedAttribute,DescriptionAttribute")]
        public void Attributes(string name, string expected)
        {
            Assert.That(string.Join(",", GetType(name).Attributes.Select(a => a.NType.Name)), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("MemberTypeParameters0", "")]
        [TestCase("MemberTypeParameters1", "CecilNTypeTest+TypeParameters2`2+TParameter1")]
        [TestCase("MemberTypeParameters2",
            "CecilNTypeTest+TypeParameters2`2+TParameter1,CecilNTypeTest+TypeParameters2`2+TParameter2")]
        public void Arguments(string fieldName, string expected)
        {
            var fieldType = GetType("TypeParameters2").Fields.First(field => field.Name == fieldName).NType;

            Assert.That(string.Join(",", fieldType.Arguments.Select(p => p.Name)), Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TypeBaseOrNullIsDefined), "Stream")]
        public void BaseOrNullIsDefined(string name, string expected)
        {
            Assert.That(GetType(name).BaseOrNull, Is.Not.Null);
            Assert.That(GetType(name).BaseOrNull.Name, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TypeBaseOrNullIsObject))]
        public void BaseOrNullIsNull(string name)
        {
            Assert.That(GetType(name).BaseOrNull, Is.Not.Null);
            Assert.That(GetType(name).BaseOrNull.Name, Is.EqualTo(nameof(System.Object)));
        }

        [Test]
        [TestCase(0, true, nameof(System.Boolean))]
        [TestCase(1, true, nameof(System.Int32))]
        [TestCase(2, true, nameof(System.Single))]
        [TestCase(3, false, "")]
        public void ElementOrNull(int index, bool defined, string expected)
        {
            var type = GetType(nameof(TypeElementOrNull));
            var method = type.Methods.Single(m => m.NDefinition == NDefinition.Abstract);
            var argument = method.Arguments.ToArray()[index];

            Assert.That(argument.NType.ElementOrNull, defined ? Is.Not.Null : Is.Null);
            Assert.That(argument.NType.ElementOrNull?.Name ?? string.Empty, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TypeAttributes0), nameof(TypeAttributes0), true)]
        [TestCase(nameof(TypeAttributes0), nameof(TypeAttributes1), false)]
        public void Equals(string name1, string name2, bool expected)
        {
            var type1 = GetType(name1);
            var type2 = GetType(name2);

            Assert.That(type1.Equals(type2), Is.EqualTo(expected));
            Assert.That(type1 == type2, Is.EqualTo(expected));
            Assert.That(type1 != type2, Is.EqualTo(!expected));
        }

        [Test]
        [TestCase(nameof(TypeFields0), "")]
        [TestCase(nameof(TypeFields1), "Field1")]
        [TestCase(nameof(TypeFields2), "Field1,Field2")]
        public void Fields(string name, string expected)
        {
            Assert.That(string.Join(",", GetType(name).Fields.Select(f => f.Name)),
                Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TypeImplementationAbstract), NDefinition.Abstract)]
        [TestCase(nameof(TypeImplementationFinal), NDefinition.Final)]
        [TestCase(nameof(TypeImplementationVirtual), NDefinition.Virtual)]
        public void Implementation(string name, NDefinition expected)
        {
            Assert.That(GetType(name).NDefinition, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TypeInterfaces0), "")]
        [TestCase(nameof(TypeInterfaces1), "IDisposable")]
        [TestCase(nameof(TypeInterfaces2), "IDisposable,ISerializable")]
        public void Interfaces(string name, string expected)
        {
            Assert.That(string.Join(",", GetType(name).Interfaces.Select(i => i.Name)),
                Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TypeMethods0), ".ctor")]
        [TestCase(nameof(TypeMethods1), "Method1,.ctor")]
        [TestCase(nameof(TypeMethods2), "Method1,Method2,.ctor")]
        public void Methods(string name, string expected)
        {
            Assert.That(string.Join(",", GetType(name).Methods.Select(m => m.Name)),
                Is.EqualTo(expected));
        }

        [Test]
        [TestCase(0, NModel.Array)]
        [TestCase(1, NModel.Class)]
        [TestCase(2, NModel.Enumeration)]
        [TestCase(3, NModel.Interface)]
        [TestCase(4, NModel.Pointer)]
        [TestCase(5, NModel.Reference)]
        [TestCase(6, NModel.Structure)]
        public void Model(int index, NModel expected)
        {
            var type = GetType(nameof(TypeModel));
            var method = type.Methods.Single(m => m.NDefinition == NDefinition.Abstract);
            var argument = method.Arguments.ToArray()[index];

            Assert.That(argument.NType.NModel, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TypeName))]
        public void Name(string name)
        {
            Assert.That(GetType(name).Name, Is.EqualTo($"{nameof(CecilNTypeTest)}+{name}"));
        }

        [Test]
        [TestCase(nameof(TypeNamespace))]
        public void Namespace(string name)
        {
            Assert.That(GetType(name).Namespace, Is.EqualTo(typeof(CecilNTypeTest).Namespace));
        }

        [Test]
        [TestCase(nameof(TypeNestedTypes0), "")]
        [TestCase(nameof(TypeNestedTypes1), "CecilNTypeTest+TypeNestedTypes1+NestedType1")]
        [TestCase(nameof(TypeNestedTypes2),
            "CecilNTypeTest+TypeNestedTypes2+NestedType1,CecilNTypeTest+TypeNestedTypes2+NestedType2")]
        public void NestedTypes(string name, string expected)
        {
            Assert.That(string.Join(",", GetType(name).NestedTypes.Select(n => n.Name)),
                Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TypeParameters0), "")]
        [TestCase("TypeParameters1", "TParameter1")]
        [TestCase("TypeParameters2", "TParameter1,TParameter2")]
        public void Parameters(string name, string expected)
        {
            Assert.That(string.Join(",", GetType(name).Parameters.Select(p => p.Name)),
                Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TypeParent), "NBrowse.Test")]
        public void Parent(string name, string expected)
        {
            Assert.That(GetType(name).Parent.Name, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TypeVisibilityInternal), NVisibility.Internal)]
        [TestCase(nameof(TypeVisibilityPrivate), NVisibility.Private)]
        [TestCase(nameof(TypeVisibilityProtected), NVisibility.Protected)]
        [TestCase(nameof(TypeVisibilityPublic), NVisibility.Public)]
        public void Visibility(string name, NVisibility expected)
        {
            Assert.That(GetType(name).NVisibility, Is.EqualTo(expected));
        }

        private static NMethod GetMethod(string name)
        {
            return CecilNProjectTest.CreateProject().FindMethod(name);
        }

        private static NType GetType(string name)
        {
            return CecilNProjectTest.CreateProject().FindType($"{nameof(CecilNTypeTest)}+{name}");
        }

        private static class TypeAttributes0
        {
        }

        [Description("attribute1")]
        private static class TypeAttributes1
        {
        }

        [CompilerGenerated]
        [Description("attribute2")]
        private static class TypeAttributes2
        {
        }

        private abstract class TypeBaseOrNullIsDefined : Stream
        {
        }

        private static class TypeBaseOrNullIsObject
        {
        }

        private abstract class TypeElementOrNull
        {
            public abstract unsafe void Method(bool[] array, int* pointer, ref float reference, bool none);
        }

        private static class TypeFields0
        {
        }

#pragma warning disable 649
        private class TypeFields1
        {
            public int Field1;
        }

        private class TypeFields2
        {
            public int Field1;
            public int Field2;
        }
#pragma warning restore 649

        private abstract class TypeImplementationAbstract
        {
        }

        private class TypeImplementationVirtual
        {
        }

        private sealed class TypeImplementationFinal
        {
        }

        private static class TypeInterfaces0
        {
        }

        private class TypeInterfaces1 : System.IDisposable
        {
            public void Dispose()
            {
            }
        }

        private class TypeInterfaces2 : System.IDisposable, ISerializable
        {
            public void Dispose()
            {
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
            }
        }

        private class TypeMethods0
        {
        }

        private abstract class TypeMethods1
        {
            public abstract void Method1();
        }

        private abstract class TypeMethods2
        {
            public abstract void Method1();
            public abstract void Method2();
        }

        private abstract class TypeModel
        {
            public abstract unsafe void Method(int[] array, object classType, System.DateTimeKind enumeration,
                System.IDisposable iface, int* pointer, ref int reference, bool structure);
        }

        private static class TypeName
        {
        }

        private static class TypeNamespace
        {
        }

        private static class TypeNestedTypes0
        {
        }

        private static class TypeNestedTypes1
        {
            private class NestedType1
            {
            }
        }

        private static class TypeNestedTypes2
        {
            private class NestedType1
            {
            }

            private class NestedType2
            {
            }
        }

        private class TypeParameters0
        {
        }

        private class TypeParameters1<TParameter1>
        {
        }

        private class TypeParameters2<TParameter1, TParameter2>
        {
#pragma warning disable CS0649
            public TypeParameters0 MemberTypeParameters0;
            public TypeParameters1<TParameter1> MemberTypeParameters1;
            public TypeParameters2<TParameter1, TParameter2> MemberTypeParameters2;
#pragma warning restore CS0649
        }

        private static class TypeParent
        {
        }

        internal static class TypeVisibilityInternal
        {
        }

        private static class TypeVisibilityPrivate
        {
        }

        protected static class TypeVisibilityProtected
        {
        }

        public static class TypeVisibilityPublic
        {
        }
    }
}