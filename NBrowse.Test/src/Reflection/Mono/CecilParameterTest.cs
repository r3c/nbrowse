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
		public void ConstraintsFromMethod(string name, int index, string expected)
		{
			var constraints = string.Join(", ",
				CecilParameterTest.GetParameterFromMethod(name, index).Constraints.Select(c => c.Name));

			Assert.That(constraints, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilParameterTest+ICecilParameterConstraints", 0, "")]
		[TestCase("CecilParameterTest+ICecilParameterConstraints", 1, "IDisposable")]
		[TestCase("CecilParameterTest+ICecilParameterConstraints", 2, "IDisposable, ValueType")]
		public void ConstraintsFromType(string name, int index, string expected)
		{
			var constraints = string.Join(", ",
				CecilParameterTest.GetParameterFromType(name, index).Constraints.Select(c => c.Name));

			Assert.That(constraints, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilParameterHasDefaultConstructor", 0, false)]
		[TestCase("CecilParameterHasDefaultConstructor", 1, true)]
		public void HasDefaultConstructorFromMethod(string name, int index, bool expected)
		{
			Assert.That(CecilParameterTest.GetParameterFromMethod(name, index).HasDefaultConstructor, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilParameterTest+ICecilParameterHasDefaultConstructor", 0, false)]
		[TestCase("CecilParameterTest+ICecilParameterHasDefaultConstructor", 1, true)]
		public void HasDefaultConstructorFromType(string name, int index, bool expected)
		{
			Assert.That(CecilParameterTest.GetParameterFromType(name, index).HasDefaultConstructor, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilParameterName", 0, "T1")]
		[TestCase("CecilParameterName", 1, "T2")]
		public void NameFromMethod(string name, int index, string expected)
		{
			Assert.That(CecilParameterTest.GetParameterFromMethod(name, index).Name, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilParameterTest+ICecilParameterName", 0, "T1")]
		[TestCase("CecilParameterTest+ICecilParameterName", 1, "T2")]
		public void NameFromType(string name, int index, string expected)
		{
			Assert.That(CecilParameterTest.GetParameterFromType(name, index).Name, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilParameterTest+ICecilParameterVariance", 0, NBrowse.Variance.Contravariant)]
		[TestCase("CecilParameterTest+ICecilParameterVariance", 1, NBrowse.Variance.Covariant)]
		[TestCase("CecilParameterTest+ICecilParameterVariance", 2, NBrowse.Variance.Invariant)]
		public void Variance(string name, int index, Variance expected)
		{
			Assert.That(CecilParameterTest.GetParameterFromType(name, index).Variance, Is.EqualTo(expected));
		}

		private static IParameter GetParameterFromMethod(string name, int index)
		{
			return CecilProjectTest.CreateProject().FindMethod(name).Parameters.ToArray()[index];
		}

		private static IParameter GetParameterFromType(string name, int index)
		{
			return CecilProjectTest.CreateProject().FindType(name).Parameters.ToArray()[index];
		}

		private interface ICecilParameterConstraints<T1, T2, T3>
			where T2 : IDisposable where T3 : struct, IDisposable
		{
		}

		private interface ICecilParameterHasDefaultConstructor<T1, T2> where T2 : new()
		{
		}

		private interface ICecilParameterName<T1, T2>
		{
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