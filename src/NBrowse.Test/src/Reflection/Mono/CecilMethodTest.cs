using System;
using System.Linq;
using NBrowse.Reflection;
using NUnit.Framework;

// ReSharper disable ConvertToConstant.Local
// ReSharper disable UnusedMember.Global

namespace NBrowse.Test.Reflection.Mono
{
    public class CecilMethodTest
    {
        [Test]
        [TestCase(nameof(TestClass.CecilMethodArguments0), "")]
        [TestCase(nameof(TestClass.CecilMethodArguments1), "a")]
        [TestCase(nameof(TestClass.CecilMethodArguments2), "a,b")]
        public void Arguments(string name, string expected)
        {
            var arguments = string.Join(",", GetMethod(name).Arguments.Select(a => a.Name));

            Assert.That(arguments, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TestClass.CecilMethodAttributes0), "")]
        [TestCase(nameof(TestClass.CecilMethodAttributes1), "DescriptionAttribute")]
        [TestCase(nameof(TestClass.CecilMethodAttributes2), "MTAThreadAttribute,STAThreadAttribute")]
        public void Attributes(string name, string expected)
        {
            var attributes = string.Join(",", GetMethod(name).Attributes.Select(a => a.Type.Name));

            Assert.That(attributes, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(CecilMethodTest) + "+" + nameof(TestClass) + "..ctor", NBrowse.Reflection.Binding.Constructor)]
        [TestCase(nameof(TestClass.CecilMethodBindingInstance), NBrowse.Reflection.Binding.Instance)]
        [TestCase(nameof(TestClass.CecilMethodBindingStatic), NBrowse.Reflection.Binding.Static)]
        public void Binding(string name, Binding expected)
        {
            Assert.That(GetMethod(name).Binding, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TestClass.CecilMethodDefinitionAbstract), NBrowse.Reflection.Definition.Abstract)]
        [TestCase(nameof(TestClass.CecilMethodDefinitionConcrete), NBrowse.Reflection.Definition.Concrete)]
        [TestCase(nameof(CecilMethodTest) + "+" + nameof(TestClass) + "." + nameof(TestClass.GetHashCode),
            NBrowse.Reflection.Definition.Final)]
        [TestCase(nameof(TestClass.CecilMethodDefinitionVirtual), NBrowse.Reflection.Definition.Virtual)]
        public void Definition(string name, Definition expected)
        {
            Assert.That(GetMethod(name).Definition, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TestClass.CecilMethodArguments0), nameof(TestClass.CecilMethodArguments0), true)]
        [TestCase(nameof(TestClass.CecilMethodArguments0), nameof(TestClass.CecilMethodArguments1), false)]
        public void Equals(string name1, string name2, bool expected)
        {
            var method1 = GetMethod(name1);
            var method2 = GetMethod(name2);

            Assert.That(method1.Equals(method2), Is.EqualTo(expected));
            Assert.That(method1 == method2, Is.EqualTo(expected));
            Assert.That(method1 != method2, Is.EqualTo(!expected));
        }

        [Test]
        [TestCase(nameof(TestClass.CecilMethodName), "CecilMethodName")]
        public void Name(string name, string expected)
        {
            Assert.That(GetMethod(name).Name, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TestClass.CecilMethodParameters0), "")]
        [TestCase(nameof(TestClass.CecilMethodParameters1), "TParameter1")]
        [TestCase(nameof(TestClass.CecilMethodParameters2), "TParameter1,TParameter2")]
        public void Parameters(string name, string expected)
        {
            var attributes = string.Join(",", GetMethod(name).Parameters.Select(p => p.Name));

            Assert.That(attributes, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TestClass.CecilMethodParent), "CecilMethodTest+TestClass")]
        public void Parent(string name, string expected)
        {
            Assert.That(GetMethod(name).Parent.Name, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TestClass.CecilMethodReturnTypeInt32), nameof(Int32))]
        [TestCase(nameof(TestClass.CecilMethodReturnTypeVoid), "Void")]
        public void ReturnType(string name, string expected)
        {
            Assert.That(GetMethod(name).ReturnType.Name, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TestClass.CecilMethodVisibilityInternal), NBrowse.Reflection.Visibility.Internal)]
        [TestCase("CecilMethodVisibilityPrivate", NBrowse.Reflection.Visibility.Private)]
        [TestCase("CecilMethodVisibilityProtected", NBrowse.Reflection.Visibility.Protected)]
        [TestCase(nameof(TestClass.CecilMethodVisibilityPublic), NBrowse.Reflection.Visibility.Public)]
        public void Visibility(string name, Visibility expected)
        {
            Assert.That(GetMethod(name).Visibility, Is.EqualTo(expected));
        }

        private static Method GetMethod(string name)
        {
            return CecilProjectTest.CreateProject().FindMethod(name);
        }

        private abstract class TestClass
        {
            public abstract void CecilMethodArguments0();
            public abstract void CecilMethodArguments1(int a);
            public abstract void CecilMethodArguments2(int a, int b);

            public abstract void CecilMethodAttributes0();

            [Description("attribute")]
            public abstract void CecilMethodAttributes1();

            [MTAThread]
            [STAThread]
            public abstract void CecilMethodAttributes2();

            public abstract void CecilMethodBindingInstance();

            public static void CecilMethodBindingStatic()
            {
            }

            public abstract void CecilMethodDefinitionAbstract();

            public void CecilMethodDefinitionConcrete()
            {
            }

            public virtual void CecilMethodDefinitionVirtual()
            {
            }

            public abstract void CecilMethodName();

            public abstract void CecilMethodParameters0();
            public abstract void CecilMethodParameters1<TParameter1>();
            public abstract void CecilMethodParameters2<TParameter1, TParameter2>();
            public abstract void CecilMethodParent();
            public abstract int CecilMethodReturnTypeInt32();
            public abstract void CecilMethodReturnTypeVoid();

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