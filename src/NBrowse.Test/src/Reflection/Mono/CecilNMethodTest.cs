using System;
using System.Linq;
using NBrowse.Reflection;
using NUnit.Framework;

// ReSharper disable ConvertToConstant.Local
// ReSharper disable UnusedMember.Global

namespace NBrowse.Test.Reflection.Mono
{
    public class CecilNMethodTest
    {
        [Test]
        [TestCase(nameof(TestClass.MethodArguments0), "")]
        [TestCase(nameof(TestClass.MethodArguments1), "a")]
        [TestCase(nameof(TestClass.MethodArguments2), "a,b")]
        public void Arguments(string name, string expected)
        {
            var arguments = string.Join(",", GetMethod(name).Arguments.Select(a => a.Name));

            Assert.That(arguments, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TestClass.MethodAttributes0), "")]
        [TestCase(nameof(TestClass.MethodAttributes1), "DescriptionAttribute")]
        [TestCase(nameof(TestClass.MethodAttributes2), "MTAThreadAttribute,STAThreadAttribute")]
        public void Attributes(string name, string expected)
        {
            var attributes = string.Join(",", GetMethod(name).Attributes.Select(a => a.NType.Name));

            Assert.That(attributes, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(CecilNMethodTest) + "+" + nameof(TestClass) + "..ctor", NBinding.Constructor)]
        [TestCase(nameof(TestClass.MethodBindingInstance), NBinding.Instance)]
        [TestCase(nameof(TestClass.MethodBindingStatic), NBinding.Static)]
        public void Binding(string name, NBinding expected)
        {
            Assert.That(GetMethod(name).NBinding, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TestClass.MethodDefinitionAbstract), NDefinition.Abstract)]
        [TestCase(nameof(TestClass.MethodDefinitionConcrete), NDefinition.Concrete)]
        [TestCase(nameof(CecilNMethodTest) + "+" + nameof(TestClass) + "." + nameof(TestClass.GetHashCode),
            NDefinition.Final)]
        [TestCase(nameof(TestClass.MethodDefinitionVirtual), NDefinition.Virtual)]
        public void Definition(string name, NDefinition expected)
        {
            Assert.That(GetMethod(name).NDefinition, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TestClass.MethodArguments0), nameof(TestClass.MethodArguments0), true)]
        [TestCase(nameof(TestClass.MethodArguments0), nameof(TestClass.MethodArguments1), false)]
        public void Equals(string name1, string name2, bool expected)
        {
            var method1 = GetMethod(name1);
            var method2 = GetMethod(name2);

            Assert.That(method1.Equals(method2), Is.EqualTo(expected));
            Assert.That(method1 == method2, Is.EqualTo(expected));
            Assert.That(method1 != method2, Is.EqualTo(!expected));
        }

        [Test]
        [TestCase(nameof(TestClass.MethodName), "MethodName")]
        public void Name(string name, string expected)
        {
            Assert.That(GetMethod(name).Name, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TestClass.MethodParameters0), "")]
        [TestCase(nameof(TestClass.MethodParameters1), "TParameter1")]
        [TestCase(nameof(TestClass.MethodParameters2), "TParameter1,TParameter2")]
        public void Parameters(string name, string expected)
        {
            var attributes = string.Join(",", GetMethod(name).Parameters.Select(p => p.Name));

            Assert.That(attributes, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TestClass.MethodParent), "CecilNMethodTest+TestClass")]
        public void Parent(string name, string expected)
        {
            Assert.That(GetMethod(name).Parent.Name, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TestClass.MethodReturnTypeInt32), nameof(Int32))]
        [TestCase(nameof(TestClass.MethodReturnTypeVoid), "Void")]
        public void ReturnType(string name, string expected)
        {
            Assert.That(GetMethod(name).ReturnNType.Name, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TestClass.MethodVisibilityInternal), NVisibility.Internal)]
        [TestCase("MethodVisibilityPrivate", NVisibility.Private)]
        [TestCase("MethodVisibilityProtected", NVisibility.Protected)]
        [TestCase(nameof(TestClass.MethodVisibilityPublic), NVisibility.Public)]
        public void Visibility(string name, NVisibility expected)
        {
            Assert.That(GetMethod(name).NVisibility, Is.EqualTo(expected));
        }

        private static NMethod GetMethod(string name)
        {
            return CecilNProjectTest.CreateProject().FindMethod(name);
        }

        private abstract class TestClass
        {
            public abstract void MethodArguments0();
            public abstract void MethodArguments1(int a);
            public abstract void MethodArguments2(int a, int b);

            public abstract void MethodAttributes0();

            [Description("attribute")]
            public abstract void MethodAttributes1();

            [MTAThread]
            [STAThread]
            public abstract void MethodAttributes2();

            public abstract void MethodBindingInstance();

            public static void MethodBindingStatic()
            {
            }

            public abstract void MethodDefinitionAbstract();

            public void MethodDefinitionConcrete()
            {
            }

            public virtual void MethodDefinitionVirtual()
            {
            }

            public abstract void MethodName();

            public abstract void MethodParameters0();
            public abstract void MethodParameters1<TParameter1>();
            public abstract void MethodParameters2<TParameter1, TParameter2>();
            public abstract void MethodParent();
            public abstract int MethodReturnTypeInt32();
            public abstract void MethodReturnTypeVoid();

            internal abstract void MethodVisibilityInternal();

            // ReSharper disable once UnusedMember.Local
            private void MethodVisibilityPrivate()
            {
            }

            protected abstract void MethodVisibilityProtected();
            public abstract void MethodVisibilityPublic();

            public sealed override int GetHashCode()
            {
                return 0;
            }
        }
    }
}