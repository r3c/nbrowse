using System;
using System.Linq;
using NBrowse.Reflection;
using NUnit.Framework;

namespace NBrowse.Test.Reflection.Mono
{
	public class CecilParameterTest
	{
		[Test]
		[TestCase("CecilParameterConstraints", 0, "")]
		[TestCase("CecilParameterConstraints", 1, "IDisposable")]
		[TestCase("CecilParameterConstraints", 2, "IDisposable, ValueType")]
		public void Constraints(string name, int index, string expected)
		{
			var constraints = string.Join(", ",
				CecilParameterTest.GetParametersFromMethod(name, index).Constraints.Select(c => c.Name));

			Assert.That(constraints, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilParameterHasDefaultConstructor", 0, false)]
		[TestCase("CecilParameterHasDefaultConstructor", 1, true)]
		public void HasDefaultConstructor(string name, int index, bool expected)
		{
			Assert.That(CecilParameterTest.GetParametersFromMethod(name, index).HasDefaultConstructor, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilParameterName", 0, "T1")]
		[TestCase("CecilParameterName", 1, "T2")]
		public void Name(string name, int index, string expected)
		{
			Assert.That(CecilParameterTest.GetParametersFromMethod(name, index).Name, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilParameterTest+ICecilParameterVariance", 0, NBrowse.Variance.Contravariant)]
		[TestCase("CecilParameterTest+ICecilParameterVariance", 1, NBrowse.Variance.Covariant)]
		[TestCase("CecilParameterTest+ICecilParameterVariance", 2, NBrowse.Variance.Invariant)]
		public void Variance(string name, int index, Variance expected)
		{
			Assert.That(CecilParameterTest.GetParametersFromType(name, index).Variance, Is.EqualTo(expected));
		}

		private static IParameter GetParametersFromMethod(string name, int index)
		{
			return CecilProjectTest.CreateProject().FindMethod(name).Parameters.ToArray()[index];
		}

		private static IParameter GetParametersFromType(string name, int index)
		{
			return CecilProjectTest.CreateProject().FindType(name).Parameters.ToArray()[index];
		}

		private interface ICecilParameterVariance<in T1, out T2, T3>
		{
		}

		private abstract class TestClass
		{
			protected abstract void CecilParameterConstraints<T1, T2, T3>()
				where T2 : IDisposable where T3 : struct, IDisposable;

			protected abstract void CecilParameterHasDefaultConstructor<T1, T2>() where T2 : new();

			protected abstract void CecilParameterName<T1, T2>();

			public abstract void Method1<THasDefaultConstructor>() where THasDefaultConstructor : new();
			public abstract TIsValueType Method2<TIsValueType>() where TIsValueType : struct, IDisposable;
		}
	}
}