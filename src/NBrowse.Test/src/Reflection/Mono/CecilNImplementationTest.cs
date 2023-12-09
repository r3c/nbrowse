using System;
using NBrowse.Reflection;
using NUnit.Framework;

// ReSharper disable UnusedMember.Global

namespace NBrowse.Test.Reflection.Mono
{
    public class CecilNImplementationTest
    {
        [Test]
        public void Parent()
        {
            var caller = GetMethod("ImplementationParent");

            Assert.That(caller.NImplementation, Is.Not.Null);
            Assert.That(caller.NImplementation.Parent, Is.EqualTo(caller));
        }

        [Test]
        [TestCase("ImplementationReferencedMethodsInvoke", true)]
        [TestCase("ImplementationReferencedMethodsNothing", false)]
        [TestCase("ImplementationReferencedMethodsValue", true)]
        public void ReferencedMethods(string name, bool expected)
        {
            var caller = GetMethod("ImplementationReferencedMethods");
            var callee = GetMethod(name);

            Assert.That(caller.NImplementation, Is.Not.Null);
            Assert.That(caller.NImplementation.ReferencedMethods,
                expected ? Does.Contain(callee) : Does.Not.Contain(callee));
        }

        [Test]
        [TestCase("ImplementationReferencedTypesArgument", true)]
        [TestCase("ImplementationReferencedTypesConstructorReference", true)]
        [TestCase("ImplementationReferencedTypesConstructorValue", true)]
        [TestCase("ImplementationReferencedTypesField", true)]
        [TestCase("ImplementationReferencedTypesNothing", false)]
        [TestCase("ImplementationReferencedTypesProperty", true)]
        [TestCase("ImplementationReferencedTypesReturn", true)]
        public void ReferencedTypes(string name, bool expected)
        {
            var caller = GetMethod("ImplementationReferencedTypes");
            var type = GetType(name);

            Assert.That(caller.NImplementation, Is.Not.Null);
            Assert.That(caller.NImplementation.ReferencedTypes, expected ? Does.Contain(type) : Does.Not.Contain(type));
        }

        private static NMethod GetMethod(string name)
        {
            return CecilNProjectTest.CreateProject().FindMethod(name);
        }

        private static NType GetType(string name)
        {
            return CecilNProjectTest.CreateProject().FindType($"{nameof(CecilNImplementationTest)}+{name}");
        }

        protected ImplementationReferencedTypesReturn ImplementationReferencedTypes(
            ImplementationReferencedTypesArgument a)
        {
            var b = new ImplementationReferencedTypesConstructorReference();
            var c = new ImplementationReferencedTypesConstructorValue();
            var d = ImplementationReferencedTypesField.Field;
            var e = ImplementationReferencedTypesProperty.Property;

            Console.WriteLine(a.ToString() + b + c + d + e);

            return new ImplementationReferencedTypesReturn();
        }

        protected struct ImplementationReferencedTypesArgument
        {
        }

        private class ImplementationReferencedTypesConstructorReference
        {
        }

        private struct ImplementationReferencedTypesConstructorValue
        {
        }

        private struct ImplementationReferencedTypesField
        {
            public static readonly long Field = DateTime.UtcNow.Ticks;
        }

        private struct ImplementationReferencedTypesProperty
        {
            public static long Property => DateTime.UtcNow.Ticks;
        }

        protected abstract class ImplementationReferencedTypesNothing
        {
        }

        protected struct ImplementationReferencedTypesReturn
        {
        }

        public abstract class TestClass
        {
            protected void ImplementationReferencedMethods()
            {
                ImplementationReferencedMethodsInvoke();

                var action = ImplementationReferencedMethodsValue;

                Console.WriteLine(action);
            }

            protected void ImplementationParent()
            {
            }

            protected abstract void ImplementationReferencedMethodsInvoke();
            protected abstract void ImplementationReferencedMethodsNothing();
            protected abstract void ImplementationReferencedMethodsValue();
        }
    }
}