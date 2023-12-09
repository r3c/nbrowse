using System;
using System.Linq;
using System.Runtime.Serialization;
using NBrowse.Reflection;
using NUnit.Framework;

namespace NBrowse.Test.Reflection.Mono
{
    public class CecilNFieldTest
    {
        [Test]
        [TestCase(nameof(TestClass.FieldAttributes0), "")]
        [TestCase(nameof(TestClass.FieldAttributes1), "DataMemberAttribute")]
        [TestCase(nameof(TestClass.FieldAttributes2), "ContextStaticAttribute,OptionalFieldAttribute")]
        public void Attributes(string name, string expected)
        {
            var attributes = string.Join(",", GetField(name).Attributes.Select(a => a.NType.Name));

            Assert.That(attributes, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TestClass.FieldBindingInstance), NBinding.Instance)]
        [TestCase(nameof(TestClass.FieldBindingStatic), NBinding.Static)]
        public void Binding(string name, NBinding expected)
        {
            Assert.That(GetField(name).NBinding, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TestClass.FieldBindingInstance), nameof(TestClass.FieldBindingInstance), true)]
        [TestCase(nameof(TestClass.FieldBindingInstance), nameof(TestClass.FieldBindingStatic), false)]
        public void Equals(string name1, string name2, bool expected)
        {
            var field1 = GetField(name1);
            var field2 = GetField(name2);

            Assert.That(field1.Equals(field2), Is.EqualTo(expected));
            Assert.That(field1 == field2, Is.EqualTo(expected));
            Assert.That(field1 != field2, Is.EqualTo(!expected));
        }

        [Test]
        [TestCase(nameof(TestClass.FieldName))]
        public void Name(string name)
        {
            Assert.That(GetField(name).Name, Is.EqualTo(name));
        }

        [Test]
        [TestCase(nameof(TestClass.FieldParent), nameof(CecilNFieldTest) + "+" + nameof(TestClass))]
        public void Parent(string name, string expected)
        {
            Assert.That(GetField(name).Parent.Name, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TestClass.FieldTypeInt32), nameof(Int32))]
        [TestCase(nameof(TestClass.FieldTypeString), nameof(String))]
        public void Type(string name, string expected)
        {
            Assert.That(GetField(name).NType.Name, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TestClass.FieldVisibilityInternal), NVisibility.Internal)]
        [TestCase("_fieldVisibilityPrivate", NVisibility.Private)]
        [TestCase("FieldVisibilityProtected", NVisibility.Protected)]
        [TestCase(nameof(TestClass.FieldVisibilityPublic), NVisibility.Public)]
        public void Visibility(string name, NVisibility expected)
        {
            Assert.That(GetField(name).NVisibility, Is.EqualTo(expected));
        }

        private static NField GetField(string name)
        {
            return CecilNProjectTest.CreateProject().FindType($"{nameof(CecilNFieldTest)}+{nameof(TestClass)}").Fields
                .Single(f => f.Name == name);
        }

        private abstract class TestClass
        {
#pragma warning disable 649
            public int FieldAttributes0;

            [DataMember]
            public int FieldAttributes1;

            [ContextStatic]
            [OptionalField]
            public int FieldAttributes2;

            public int FieldBindingInstance;

            public static int FieldBindingStatic;

            public int FieldName;

            public int FieldParent;

            public int FieldTypeInt32;
            public string FieldTypeString;

            internal int FieldVisibilityInternal;
#pragma warning disable 169
            private int _fieldVisibilityPrivate;
#pragma warning restore 169
            protected int FieldVisibilityProtected;
            public int FieldVisibilityPublic;
#pragma warning restore 649
        }
    }
}