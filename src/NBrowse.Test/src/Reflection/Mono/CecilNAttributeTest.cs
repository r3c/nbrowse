using System.Linq;
using System.Runtime.CompilerServices;
using NBrowse.Reflection;
using NUnit.Framework;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace NBrowse.Test.Reflection.Mono
{
    public class CecilNAttributeTest
    {
        [Test]
        [TestCase(nameof(TestClass.AttributeArguments), 1, 0, "argument0")]
        [TestCase(nameof(TestClass.AttributeArguments), 1, 1, "argument1")]
        public void Arguments_List(string name, int attributeIndex, int argumentIndex, object expected)
        {
            Assert.That(
                GetAttributeFromMethod(name, attributeIndex).Arguments.ToList()[argumentIndex],
                Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TestClass.AttributeArguments), 0, 0, false)]
        public void Arguments_None(string name, int attributeIndex, int argumentIndex, object expected)
        {
            Assert.That(GetAttributeFromMethod(name, attributeIndex).Arguments, Is.Empty);
        }

        [Test]
        [TestCase(nameof(TestClass.AttributeConstructor), 0, ".ctor")]
        public void Constructor(string name, int index, string expected)
        {
            Assert.That(GetAttributeFromMethod(name, index).Constructor.Name, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TestClass.AttributeArguments), 0, nameof(TestClass.AttributeArguments), 0, true)]
        [TestCase(nameof(TestClass.AttributeArguments), 0, nameof(TestClass.AttributeArguments), 1, false)]
        [TestCase(nameof(TestClass.AttributeType), 0, nameof(TestClass.AttributeArguments), 0, false)]
        public void Equals(string name1, int index1, string name2, int index2, bool expected)
        {
            var attribute1 = GetAttributeFromMethod(name1, index1);
            var attribute2 = GetAttributeFromMethod(name2, index2);

            Assert.That(attribute1.Equals(attribute2), Is.EqualTo(expected));
            Assert.That(attribute1 == attribute2, Is.EqualTo(expected));
            Assert.That(attribute1 != attribute2, Is.EqualTo(!expected));
        }

        [Test]
        [TestCase(nameof(TestClass.AttributeType), 0, "CompilerGeneratedAttribute")]
        [TestCase(nameof(TestClass.AttributeType), 1, "DescriptionAttribute")]
        public void TypeOfMethodAttribute(string name, int index, string expected)
        {
            Assert.That(GetAttributeFromMethod(name, index).NType.Name, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(CecilNAttributeTest) + "+" + nameof(AttributeClass), 0, "CompilerGeneratedAttribute")]
        [TestCase(nameof(CecilNAttributeTest) + "+" + nameof(AttributeClass), 1, "DescriptionAttribute")]
        public void TypeOfTypeAttribute(string name, int index, string expected)
        {
            Assert.That(GetAttributeFromType(name, index).NType.Name, Is.EqualTo(expected));
        }

        private static NAttribute GetAttributeFromMethod(string name, int index)
        {
            return CecilNProjectTest.CreateProject().FindMethod(name).Attributes.ToArray()[index];
        }

        private static NAttribute GetAttributeFromType(string name, int index)
        {
            return CecilNProjectTest.CreateProject().FindType(name).Attributes.ToArray()[index];
        }

        [CompilerGenerated]
        [Description("attribute class")]
        private static class AttributeClass
        {
        }

        private abstract class TestClass
        {
            [System.STAThread]
            [Author("argument0", "argument1")]
            public abstract void AttributeArguments();

            [Description("constructor")]
            public abstract void AttributeConstructor();

            [CompilerGenerated]
            [Description("type")]
            public abstract void AttributeType();
        }
    }
}