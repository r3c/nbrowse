using System;
using System.Linq;
using System.Runtime.Serialization;
using NBrowse.Reflection;
using NUnit.Framework;

namespace NBrowse.Test.Reflection.Mono
{
    public class CecilFieldTest
    {
        [Test]
        [TestCase(nameof(TestClass.CecilFieldAttributes0), "")]
        [TestCase(nameof(TestClass.CecilFieldAttributes1), "DataMemberAttribute")]
        [TestCase(nameof(TestClass.CecilFieldAttributes2), "ContextStaticAttribute,OptionalFieldAttribute")]
        public void Attributes(string name, string expected)
        {
            var attributes = string.Join(",", GetField(name).Attributes.Select(a => a.Type.Name));

            Assert.That(attributes, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TestClass.CecilFieldBindingInstance), NBrowse.Reflection.Binding.Instance)]
        [TestCase(nameof(TestClass.CecilFieldBindingStatic), NBrowse.Reflection.Binding.Static)]
        public void Binding(string name, Binding expected)
        {
            Assert.That(GetField(name).Binding, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TestClass.CecilFieldBindingInstance), nameof(TestClass.CecilFieldBindingInstance), true)]
        [TestCase(nameof(TestClass.CecilFieldBindingInstance), nameof(TestClass.CecilFieldBindingStatic), false)]
        public void Equals(string name1, string name2, bool expected)
        {
            var field1 = GetField(name1);
            var field2 = GetField(name2);

            Assert.That(field1.Equals(field2), Is.EqualTo(expected));
            Assert.That(field1 == field2, Is.EqualTo(expected));
            Assert.That(field1 != field2, Is.EqualTo(!expected));
        }

        [Test]
        [TestCase(nameof(TestClass.CecilFieldName))]
        public void Name(string name)
        {
            Assert.That(GetField(name).Name, Is.EqualTo(name));
        }

        [Test]
        [TestCase(nameof(TestClass.CecilFieldParent), nameof(CecilFieldTest) + "+" + nameof(TestClass))]
        public void Parent(string name, string expected)
        {
            Assert.That(GetField(name).Parent.Name, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TestClass.CecilFieldTypeInt32), nameof(Int32))]
        [TestCase(nameof(TestClass.CecilFieldTypeString), nameof(String))]
        public void Type(string name, string expected)
        {
            Assert.That(GetField(name).Type.Name, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TestClass.CecilFieldVisibilityInternal), NBrowse.Reflection.Visibility.Internal)]
        [TestCase("_cecilFieldVisibilityPrivate", NBrowse.Reflection.Visibility.Private)]
        [TestCase("CecilFieldVisibilityProtected", NBrowse.Reflection.Visibility.Protected)]
        [TestCase(nameof(TestClass.CecilFieldVisibilityPublic), NBrowse.Reflection.Visibility.Public)]
        public void Visibility(string name, Visibility expected)
        {
            Assert.That(GetField(name).Visibility, Is.EqualTo(expected));
        }

        private static Field GetField(string name)
        {
            return CecilProjectTest.CreateProject().FindType($"{nameof(CecilFieldTest)}+{nameof(TestClass)}").Fields
                .Single(f => f.Name == name);
        }

        private abstract class TestClass
        {
#pragma warning disable 649
            public int CecilFieldAttributes0;

            [DataMember]
            public int CecilFieldAttributes1;

            [ContextStatic]
            [OptionalField]
            public int CecilFieldAttributes2;

            public int CecilFieldBindingInstance;

            public static int CecilFieldBindingStatic;

            public int CecilFieldName;

            public int CecilFieldParent;

            public int CecilFieldTypeInt32;
            public string CecilFieldTypeString;

            internal int CecilFieldVisibilityInternal;
#pragma warning disable 169
            private int _cecilFieldVisibilityPrivate;
#pragma warning restore 169
            protected int CecilFieldVisibilityProtected;
            public int CecilFieldVisibilityPublic;
#pragma warning restore 649
        }
    }
}