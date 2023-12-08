using System;
using System.Linq;
using NBrowse.Reflection;
using NUnit.Framework;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedType.Local
// ReSharper disable UnusedTypeParameter

namespace NBrowse.Test.Reflection.Mono
{
    public class CecilParameterTest
    {
        [Test]
        [TestCase(nameof(TestClass.CecilParameterConstraints), 0, "")]
        [TestCase(nameof(TestClass.CecilParameterConstraints), 1, "IDisposable")]
        [TestCase(nameof(TestClass.CecilParameterConstraints), 2, "IDisposable,ValueType")]
        public void ConstraintsOfMethodAttribute(string name, int index, string expected)
        {
            var constraints = string.Join(",",
                CecilParameterTest.GetParameterFromMethod(name, index).Constraints.Select(c => c.Name));

            Assert.That(constraints, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(CecilParameterTest) + "+ICecilParameterConstraints", 0, "")]
        [TestCase(nameof(CecilParameterTest) + "+ICecilParameterConstraints", 1, "IDisposable")]
        [TestCase(nameof(CecilParameterTest) + "+ICecilParameterConstraints", 2, "IDisposable,ValueType")]
        public void ConstraintsOfTypeAttribute(string name, int index, string expected)
        {
            var constraints = string.Join(",",
                CecilParameterTest.GetParameterFromType(name, index).Constraints.Select(c => c.Name));

            Assert.That(constraints, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TestClass.CecilParameterName), 0, nameof(TestClass.CecilParameterName), 0, true)]
        [TestCase(nameof(TestClass.CecilParameterName), 0, nameof(TestClass.CecilParameterName), 1, false)]
        [TestCase(nameof(TestClass.CecilParameterName), 0, nameof(TestClass.CecilParameterConstraints), 1, false)]
        public void Equals(string name1, int index1, string name2, int index2, bool expected)
        {
            var parameter1 = CecilParameterTest.GetParameterFromMethod(name1, index1);
            var parameter2 = CecilParameterTest.GetParameterFromMethod(name2, index2);

            Assert.That(parameter1.Equals(parameter2), Is.EqualTo(expected));
            Assert.That(parameter1 == parameter2, Is.EqualTo(expected));
            Assert.That(parameter1 != parameter2, Is.EqualTo(!expected));
        }

        [Test]
        [TestCase(0, true)]
        [TestCase(1, false)]
        [TestCase(2, true)]
        public void HasDefaultConstructorConstraintOfMethodAttribute(int index, bool expected)
        {
            var parameter = CecilParameterTest.GetParameterFromMethod(
                nameof(TestClass.CecilParameterHasConstraint), index);

            Assert.That(parameter.HasDefaultConstructorConstraint, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(0, true)]
        [TestCase(1, false)]
        [TestCase(2, true)]
        public void HasDefaultConstructorConstraintOfTypeAttribute(int index, bool expected)
        {
            var parameter = CecilParameterTest.GetParameterFromType(
                nameof(CecilParameterTest) + "+ICecilParameterHasDefaultConstructor", index);

            Assert.That(parameter.HasDefaultConstructorConstraint, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(0, false)]
        [TestCase(1, true)]
        [TestCase(2, false)]
        public void HasReferenceTypeConstraintOfMethodAttribute(int index, bool expected)
        {
            var parameter = CecilParameterTest.GetParameterFromMethod(
                nameof(TestClass.CecilParameterHasConstraint), index);

            Assert.That(parameter.HasReferenceTypeConstraint, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(0, false)]
        [TestCase(1, true)]
        [TestCase(2, false)]
        public void HasReferenceTypeConstraintOfTypeAttribute(int index, bool expected)
        {
            var parameter = CecilParameterTest.GetParameterFromType(
                nameof(CecilParameterTest) + "+ICecilParameterHasDefaultConstructor", index);

            Assert.That(parameter.HasReferenceTypeConstraint, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(0, false)]
        [TestCase(1, false)]
        [TestCase(2, true)]
        public void HasValueTypeConstraintOfMethodAttribute(int index, bool expected)
        {
            var parameter = CecilParameterTest.GetParameterFromMethod(
                nameof(TestClass.CecilParameterHasConstraint), index);

            Assert.That(parameter.HasValueTypeConstraint, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(0, false)]
        [TestCase(1, false)]
        [TestCase(2, true)]
        public void HasValueTypeConstraintOfTypeAttribute(int index, bool expected)
        {
            var parameter = CecilParameterTest.GetParameterFromType(
                nameof(CecilParameterTest) + "+ICecilParameterHasDefaultConstructor", index);

            Assert.That(parameter.HasValueTypeConstraint, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(TestClass.CecilParameterName), 0, "TParameter1")]
        [TestCase(nameof(TestClass.CecilParameterName), 1, "TParameter2")]
        public void NameOfMethodAttribute(string name, int index, string expected)
        {
            Assert.That(CecilParameterTest.GetParameterFromMethod(name, index).Name, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(CecilParameterTest) + "+ICecilParameterName", 0, "TParameter1")]
        [TestCase(nameof(CecilParameterTest) + "+ICecilParameterName", 1, "TParameter2")]
        public void NameOfTypeAttribute(string name, int index, string expected)
        {
            Assert.That(CecilParameterTest.GetParameterFromType(name, index).Name, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(nameof(CecilParameterTest) + "+ICecilParameterVariance", 0,
            NBrowse.Reflection.Variance.Contravariant)]
        [TestCase(nameof(CecilParameterTest) + "+ICecilParameterVariance", 1, NBrowse.Reflection.Variance.Covariant)]
        [TestCase(nameof(CecilParameterTest) + "+ICecilParameterVariance", 2, NBrowse.Reflection.Variance.Invariant)]
        public void Variance(string name, int index, Variance expected)
        {
            Assert.That(CecilParameterTest.GetParameterFromType(name, index).Variance, Is.EqualTo(expected));
        }

        private static Parameter GetParameterFromMethod(string name, int index)
        {
            return CecilProjectTest.CreateProject().FindMethod(name).Parameters.ToArray()[index];
        }

        private static Parameter GetParameterFromType(string name, int index)
        {
            return CecilProjectTest.CreateProject().FindType(name).Parameters.ToArray()[index];
        }

        private interface ICecilParameterConstraints<TParameter1, TParameter2, TParameter3>
            where TParameter2 : IDisposable where TParameter3 : struct, IDisposable
        {
        }

        private interface ICecilParameterHasDefaultConstructor<TParameter1, TParameter2, TParameter3>
            where TParameter1 : new()
            where TParameter2 : class
            where TParameter3 : struct
        {
        }

        private interface ICecilParameterName<TParameter1, TParameter2>
        {
        }

        private interface ICecilParameterVariance<in TParameter1, out TParameter2, TParameter3>
        {
        }

        private abstract class TestClass
        {
            public abstract void CecilParameterConstraints<TParameter1, TParameter2, TParameter3>()
                where TParameter2 : IDisposable
                where TParameter3 : struct, IDisposable;

            public abstract void CecilParameterHasConstraint<TParameter1, TParameter2, TParameter3>()
                where TParameter1 : new()
                where TParameter2 : class
                where TParameter3 : struct;

            public abstract void CecilParameterName<TParameter1, TParameter2>();

            public abstract void Method1<THasDefaultConstructor>() where THasDefaultConstructor : new();
            public abstract TIsValueType Method2<TIsValueType>() where TIsValueType : struct, IDisposable;
        }
    }
}