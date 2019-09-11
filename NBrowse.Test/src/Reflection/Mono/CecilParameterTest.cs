using System;
using System.Linq;
using NBrowse.Reflection;
using NUnit.Framework;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedTypeParameter

namespace NBrowse.Test.Reflection.Mono
{
	public class CecilParameterTest
	{
		[Test]
		[TestCase("CecilParameterConstraints", 0, "")]
		[TestCase("CecilParameterConstraints", 1, "IDisposable")]
		[TestCase("CecilParameterConstraints", 2, "IDisposable,ValueType")]
		public void ConstraintsOfMethodAttribute(string name, int index, string expected)
		{
			var constraints = string.Join(",",
				CecilParameterTest.GetParameterFromMethod(name, index).Constraints.Select(c => c.Name));

			Assert.That(constraints, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilParameterTest+ICecilParameterConstraints", 0, "")]
		[TestCase("CecilParameterTest+ICecilParameterConstraints", 1, "IDisposable")]
		[TestCase("CecilParameterTest+ICecilParameterConstraints", 2, "IDisposable,ValueType")]
		public void ConstraintsOfTypeAttribute(string name, int index, string expected)
		{
			var constraints = string.Join(",",
				CecilParameterTest.GetParameterFromType(name, index).Constraints.Select(c => c.Name));

			Assert.That(constraints, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilParameterHasDefaultConstructor", 0, false)]
		[TestCase("CecilParameterHasDefaultConstructor", 1, true)]
		public void HasDefaultConstructorOfMethodAttribute(string name, int index, bool expected)
		{
			Assert.That(CecilParameterTest.GetParameterFromMethod(name, index).HasDefaultConstructor,
				Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilParameterTest+ICecilParameterHasDefaultConstructor", 0, false)]
		[TestCase("CecilParameterTest+ICecilParameterHasDefaultConstructor", 1, true)]
		public void HasDefaultConstructorOfTypeAttribute(string name, int index, bool expected)
		{
			Assert.That(CecilParameterTest.GetParameterFromType(name, index).HasDefaultConstructor,
				Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilParameterName", 0, "TParameter1")]
		[TestCase("CecilParameterName", 1, "TParameter2")]
		public void NameOfMethodAttribute(string name, int index, string expected)
		{
			Assert.That(CecilParameterTest.GetParameterFromMethod(name, index).Name, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilParameterTest+ICecilParameterName", 0, "TParameter1")]
		[TestCase("CecilParameterTest+ICecilParameterName", 1, "TParameter2")]
		public void NameOfTypeAttribute(string name, int index, string expected)
		{
			Assert.That(CecilParameterTest.GetParameterFromType(name, index).Name, Is.EqualTo(expected));
		}

		[Test]
		[TestCase("CecilParameterTest+ICecilParameterVariance", 0, NBrowse.Reflection.Variance.Contravariant)]
		[TestCase("CecilParameterTest+ICecilParameterVariance", 1, NBrowse.Reflection.Variance.Covariant)]
		[TestCase("CecilParameterTest+ICecilParameterVariance", 2, NBrowse.Reflection.Variance.Invariant)]
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

		private interface ICecilParameterConstraints<TParameter1, TParameter2, TParameter3>
			where TParameter2 : IDisposable where TParameter3 : struct, IDisposable
		{
		}

		private interface ICecilParameterHasDefaultConstructor<TParameter1, TParameter2> where TParameter2 : new()
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
			protected abstract void CecilParameterConstraints<TParameter1, TParameter2, TParameter3>()
				where TParameter2 : IDisposable where TParameter3 : struct, IDisposable;

			protected abstract void CecilParameterHasDefaultConstructor<TParameter1, TParameter2>()
				where TParameter2 : new();

			protected abstract void CecilParameterName<TParameter1, TParameter2>();

			public abstract void Method1<THasDefaultConstructor>() where THasDefaultConstructor : new();
			public abstract TIsValueType Method2<TIsValueType>() where TIsValueType : struct, IDisposable;
		}
	}
}