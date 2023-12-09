using System;
using System.Linq;
using NBrowse.Reflection;
using NUnit.Framework;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedType.Local
// ReSharper disable UnusedTypeParameter

namespace NBrowse.Test.Reflection.Mono;

public class CecilNParameterTest
{
    [Test]
    [TestCase(nameof(TestClass.ParameterConstraints), 0, "")]
    [TestCase(nameof(TestClass.ParameterConstraints), 1, "IDisposable")]
    [TestCase(nameof(TestClass.ParameterConstraints), 2, "IDisposable,ValueType")]
    public void ConstraintsOfMethodAttribute(string name, int index, string expected)
    {
        var constraints = string.Join(",",
            GetParameterFromMethod(name, index).Constraints.Select(c => c.Name));

        Assert.That(constraints, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(nameof(CecilNParameterTest) + "+IParameterConstraints", 0, "")]
    [TestCase(nameof(CecilNParameterTest) + "+IParameterConstraints", 1, "IDisposable")]
    [TestCase(nameof(CecilNParameterTest) + "+IParameterConstraints", 2, "IDisposable,ValueType")]
    public void ConstraintsOfTypeAttribute(string name, int index, string expected)
    {
        var constraints = string.Join(",",
            GetParameterFromType(name, index).Constraints.Select(c => c.Name));

        Assert.That(constraints, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(nameof(TestClass.ParameterName), 0, nameof(TestClass.ParameterName), 0, true)]
    [TestCase(nameof(TestClass.ParameterName), 0, nameof(TestClass.ParameterName), 1, false)]
    [TestCase(nameof(TestClass.ParameterName), 0, nameof(TestClass.ParameterConstraints), 1, false)]
    public void Equals(string name1, int index1, string name2, int index2, bool expected)
    {
        var parameter1 = GetParameterFromMethod(name1, index1);
        var parameter2 = GetParameterFromMethod(name2, index2);

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
        var parameter = GetParameterFromMethod(
            nameof(TestClass.ParameterHasConstraint), index);

        Assert.That(parameter.HasDefaultConstructorConstraint, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(0, true)]
    [TestCase(1, false)]
    [TestCase(2, true)]
    public void HasDefaultConstructorConstraintOfTypeAttribute(int index, bool expected)
    {
        var parameter = GetParameterFromType(
            nameof(CecilNParameterTest) + "+IParameterHasDefaultConstructor", index);

        Assert.That(parameter.HasDefaultConstructorConstraint, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(0, false)]
    [TestCase(1, true)]
    [TestCase(2, false)]
    public void HasReferenceTypeConstraintOfMethodAttribute(int index, bool expected)
    {
        var parameter = GetParameterFromMethod(
            nameof(TestClass.ParameterHasConstraint), index);

        Assert.That(parameter.HasReferenceTypeConstraint, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(0, false)]
    [TestCase(1, true)]
    [TestCase(2, false)]
    public void HasReferenceTypeConstraintOfTypeAttribute(int index, bool expected)
    {
        var parameter = GetParameterFromType(
            nameof(CecilNParameterTest) + "+IParameterHasDefaultConstructor", index);

        Assert.That(parameter.HasReferenceTypeConstraint, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(0, false)]
    [TestCase(1, false)]
    [TestCase(2, true)]
    public void HasValueTypeConstraintOfMethodAttribute(int index, bool expected)
    {
        var parameter = GetParameterFromMethod(
            nameof(TestClass.ParameterHasConstraint), index);

        Assert.That(parameter.HasValueTypeConstraint, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(0, false)]
    [TestCase(1, false)]
    [TestCase(2, true)]
    public void HasValueTypeConstraintOfTypeAttribute(int index, bool expected)
    {
        var parameter = GetParameterFromType(
            nameof(CecilNParameterTest) + "+IParameterHasDefaultConstructor", index);

        Assert.That(parameter.HasValueTypeConstraint, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(nameof(TestClass.ParameterName), 0, "TParameter1")]
    [TestCase(nameof(TestClass.ParameterName), 1, "TParameter2")]
    public void NameOfMethodAttribute(string name, int index, string expected)
    {
        Assert.That(GetParameterFromMethod(name, index).Name, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(nameof(CecilNParameterTest) + "+IParameterName", 0, "TParameter1")]
    [TestCase(nameof(CecilNParameterTest) + "+IParameterName", 1, "TParameter2")]
    public void NameOfTypeAttribute(string name, int index, string expected)
    {
        Assert.That(GetParameterFromType(name, index).Name, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(nameof(CecilNParameterTest) + "+IParameterVariance", 0, NVariance.Contravariant)]
    [TestCase(nameof(CecilNParameterTest) + "+IParameterVariance", 1, NVariance.Covariant)]
    [TestCase(nameof(CecilNParameterTest) + "+IParameterVariance", 2, NVariance.Invariant)]
    public void Variance(string name, int index, NVariance expected)
    {
        Assert.That(GetParameterFromType(name, index).NVariance, Is.EqualTo(expected));
    }

    private static NParameter GetParameterFromMethod(string name, int index)
    {
        return CecilNProjectTest.CreateProject().FindMethod(name).Parameters.ToArray()[index];
    }

    private static NParameter GetParameterFromType(string name, int index)
    {
        return CecilNProjectTest.CreateProject().FindType(name).Parameters.ToArray()[index];
    }

    private interface IParameterConstraints<TParameter1, TParameter2, TParameter3>
        where TParameter2 : IDisposable where TParameter3 : struct, IDisposable
    {
    }

    private interface IParameterHasDefaultConstructor<TParameter1, TParameter2, TParameter3>
        where TParameter1 : new()
        where TParameter2 : class
        where TParameter3 : struct
    {
    }

    private interface IParameterName<TParameter1, TParameter2>
    {
    }

    private interface IParameterVariance<in TParameter1, out TParameter2, TParameter3>
    {
    }

    private abstract class TestClass
    {
        public abstract void ParameterConstraints<TParameter1, TParameter2, TParameter3>()
            where TParameter2 : IDisposable
            where TParameter3 : struct, IDisposable;

        public abstract void ParameterHasConstraint<TParameter1, TParameter2, TParameter3>()
            where TParameter1 : new()
            where TParameter2 : class
            where TParameter3 : struct;

        public abstract void ParameterName<TParameter1, TParameter2>();
    }
}