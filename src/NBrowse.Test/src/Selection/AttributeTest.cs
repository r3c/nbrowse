using System;
using System.Runtime.CompilerServices;
using NBrowse.Reflection;
using NBrowse.Selection;
using NBrowse.Test.Reflection.Mono;
using NUnit.Framework;

namespace NBrowse.Test.Selection
{
    public class AttributeTest
    {
        [Test]
        [TestCase("MethodHasAttributeNone", false)]
        [TestCase("MethodHasAttributeOne", true)]
        public void HasAttributeOnMethod(string name, bool expected)
        {
            var method = GetMethod(name);

            Assert.That(method.HasAttribute<ObsoleteAttribute>(), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("AttributeTest+ClassHasAttributeNone", false)]
        [TestCase("AttributeTest+ClassHasAttributeOne", true)]
        public void HasAttributeOnType(string name, bool expected)
        {
            var type = GetType(name);

            Assert.That(type.HasAttribute<ObsoleteAttribute>(), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("IsGenerated", true)]
        [TestCase("IsNotGenerated", false)]
        public void IsGeneratedOnMethod(string name, bool expected)
        {
            var method = GetMethod(name);

            Assert.That(method.IsGenerated(), Is.EqualTo(expected));
        }

        [Test]
        [TestCase("AttributeTest+IsGeneratedStructure", true)]
        [TestCase("AttributeTest+IsNotGeneratedStructure", false)]
        public void IsGeneratedOnType(string name, bool expected)
        {
            var type = GetType(name);

            Assert.That(type.IsGenerated(), Is.EqualTo(expected));
        }

        private static Method GetMethod(string name)
        {
            return CecilProjectTest.CreateProject().FindMethod(name);
        }

        private static NBrowse.Reflection.Type GetType(string name)
        {
            return CecilProjectTest.CreateProject().FindType(name);
        }

        [CompilerGenerated]
        private struct IsGeneratedStructure
        {
        }

        private abstract class IsNotGeneratedStructure
        {
            [CompilerGenerated]
            public abstract void IsGenerated();

            public abstract void IsNotGenerated();
        }

        [Obsolete]
        private struct ClassHasAttributeOne
        {
        }

        private abstract class ClassHasAttributeNone
        {
            public abstract void MethodHasAttributeNone();

            [Obsolete]
            public abstract void MethodHasAttributeOne();
        }
    }
}