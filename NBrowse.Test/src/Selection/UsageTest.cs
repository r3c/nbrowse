using System;
using Moq;
using NBrowse.Reflection;
using NBrowse.Selection;
using NBrowse.Test.Reflection.Mono;
using NUnit.Framework;

namespace NBrowse.Test.Selection
{
	public class UsageTest
	{
		[Test]
		[TestCase(false, false)]
		[TestCase(true, true)]
		public void IsUsing_Method_Method(bool matchBody, bool expected)
		{
			var source = new Mock<IMethod>();
			var target = new Mock<IMethod>();

			target.Setup(t => t.Equals(target.Object)).Returns(true);

			var body = new Mock<IImplementation>();

			body.Setup(b => b.ReferencedMethods).Returns(new[] {target.Object});
			source.Setup(t => t.ImplementationOrNull).Returns(matchBody ? body.Object : null);

			Assert.That(source.Object.IsUsing(target.Object), Is.EqualTo(expected));
		}

		[Test]
		[TestCase(false, false, false, false, false, false)]
		[TestCase(true, false, false, false, false, true)]
		[TestCase(false, true, false, false, false, true)]
		[TestCase(false, false, true, false, false, true)]
		[TestCase(false, false, false, true, false, true)]
		[TestCase(false, false, false, false, true, true)]
		public void IsUsing_Method_Type(bool matchArguments, bool matchAttributes, bool matchParameters, bool matchBody,
			bool matchReturnType, bool expected)
		{
			var source = new Mock<IMethod>();
			var target = new Mock<IType>();

			target.Setup(t => t.Equals(target.Object)).Returns(true);

			var argument = new Mock<IArgument>();

			argument.Setup(a => a.Type).Returns(matchArguments ? target.Object : Mock.Of<IType>());
			source.Setup(t => t.Arguments).Returns(new[] {argument.Object});

			var attribute = new Mock<IAttribute>();

			attribute.Setup(a => a.Type).Returns(matchAttributes ? target.Object : Mock.Of<IType>());
			source.Setup(t => t.Attributes).Returns(new[] {attribute.Object});

			var parameter = new Mock<IParameter>();

			parameter.Setup(p => p.Constraints).Returns(new[] {matchParameters ? target.Object : Mock.Of<IType>()});
			source.Setup(t => t.Parameters).Returns(new[] {parameter.Object});

			var body = new Mock<IImplementation>();

			body.Setup(b => b.ReferencedTypes).Returns(new[] {target.Object});
			source.Setup(t => t.ImplementationOrNull).Returns(matchBody ? body.Object : null);

			source.Setup(t => t.ReturnType).Returns(matchReturnType ? target.Object : Mock.Of<IType>());

			Assert.That(source.Object.IsUsing(target.Object), Is.EqualTo(expected));
		}

		[Test]
		[TestCase(false, false)]
		[TestCase(true, true)]
		public void IsUsing_Type_Method(bool matchMethods, bool expected)
		{
			var source = new Mock<IType>();
			var target = new Mock<IMethod>();

			target.Setup(t => t.Equals(target.Object)).Returns(true);

			var body = new Mock<IImplementation>();

			body.Setup(b => b.ReferencedMethods).Returns(new[] {target.Object});

			var method = new Mock<IMethod>();

			method.Setup(t => t.ImplementationOrNull).Returns(matchMethods ? body.Object : null);
			source.Setup(t => t.Methods).Returns(new[] {method.Object});

			Assert.That(source.Object.IsUsing(target.Object), Is.EqualTo(expected));
		}

		[Test]
		[TestCase(false, false, false, false, false, false, false, false)]
		[TestCase(true, false, false, false, false, false, false, true)]
		[TestCase(false, true, false, false, false, false, false, true)]
		[TestCase(false, false, true, false, false, false, false, true)]
		[TestCase(false, false, false, true, false, false, false, true)]
		[TestCase(false, false, false, false, true, false, false, true)]
		[TestCase(false, false, false, false, false, true, false, true)]
		[TestCase(false, false, false, false, false, false, true, true)]
		public void IsUsing_Type_Type(bool matchAttributes, bool matchBaseOrNull, bool matchFields,
			bool matchInterfaces, bool matchNestedTypes, bool matchParameters, bool matchMethods,
			bool expected)
		{
			var source = new Mock<IType>();
			var target = new Mock<IType>();

			target.Setup(t => t.Equals(target.Object)).Returns(true);

			var attribute = new Mock<IAttribute>();

			attribute.Setup(a => a.Type).Returns(matchAttributes ? target.Object : Mock.Of<IType>());

			source.Setup(t => t.Attributes).Returns(new[] {attribute.Object});

			source.Setup(t => t.BaseOrNull).Returns(matchBaseOrNull ? target.Object : null);

			var field = new Mock<IField>();

			field.Setup(a => a.Type).Returns(matchFields ? target.Object : Mock.Of<IType>());
			source.Setup(t => t.Fields).Returns(new[] {field.Object});

			source.Setup(t => t.Interfaces).Returns(matchInterfaces ? new[] {target.Object} : Array.Empty<IType>());

			source.Setup(t => t.NestedTypes).Returns(matchNestedTypes ? new[] {target.Object} : Array.Empty<IType>());

			var parameter = new Mock<IParameter>();

			parameter.Setup(p => p.Constraints).Returns(new[] {matchParameters ? target.Object : Mock.Of<IType>()});
			source.Setup(t => t.Parameters).Returns(new[] {parameter.Object});

			var method = new Mock<IMethod>();

			method.Setup(m => m.ReturnType).Returns(matchMethods ? target.Object : Mock.Of<IType>());
			source.Setup(t => t.Methods).Returns(new[] {method.Object});

			Assert.That(source.Object.IsUsing(target.Object), Is.EqualTo(expected));
		}

		[Test]
		[TestCase(128, 1)]
		[TestCase(0, 2)]
		public void IsUsing_Type_Type_WithCache(int cacheSize, int expectedComparisons)
		{
			// Force flush cache by setting size to zero and asking for usage of some type not previously used
			var fake = CecilProjectTest.CreateProject().FindType(typeof(FakeType).FullName);

			Usage.CacheSize = 0;

			fake.IsUsing(fake);

			Usage.CacheSize = cacheSize;

			// Configure mocks to equal themselves so they pass cache verification
			var source = new Mock<IType>();
			var target = new Mock<IType>();

			source.Setup(type => type.Equals(source.Object)).Returns(true);
			target.Setup(type => type.Equals(target.Object)).Returns(true);

			// Call "IsUsing" a first time and verify type equality was used exactly once
			Assert.That(source.Object.IsUsing(target.Object), Is.False);

			source.Verify(type => type.Equals(target.Object), Times.Once());

			// Call "IsUsing" a second time and verify type equality was used depending on cache status
			Assert.That(source.Object.IsUsing(target.Object), Is.False);

			source.Verify(type => type.Equals(target.Object), Times.Exactly(expectedComparisons));
		}

		private struct FakeType
		{
		}
	}
}