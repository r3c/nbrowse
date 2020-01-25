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
		[TestCase(false, false, false, false)]
		[TestCase(false, true, false, false)]
		[TestCase(false, true, true, false)]
		[TestCase(true, false, false, true)]
		[TestCase(true, true, false, false)]
		[TestCase(true, true, true, true)]
		public void IsUsing_Method_Method(bool matchImplementation, bool setupRecursive, bool usingRecursive,
			bool expected)
		{
			var source = new Mock<IMethod>();
			var target = new Mock<IMethod>();

			target.Setup(t => t.Equals(target.Object)).Returns(true);

			IMethod resolve;

			if (setupRecursive)
			{
				var indirect = new Mock<IMethod>();
				var indirectImplementation = new Mock<IImplementation>();

				indirectImplementation.Setup(i => i.ReferencedMethods).Returns(new[] {target.Object});
				indirect.Setup(i => i.ImplementationOrNull).Returns(indirectImplementation.Object);

				resolve = indirect.Object;
			}
			else
				resolve = target.Object;

			var implementation = new Mock<IImplementation>();

			implementation.Setup(b => b.ReferencedMethods).Returns(new[] {resolve});
			source.Setup(t => t.ImplementationOrNull).Returns(matchImplementation ? implementation.Object : null);

			Assert.That(source.Object.IsUsing(target.Object, usingRecursive), Is.EqualTo(expected));
		}

		[Test]
		[TestCase(false, false, false, false, false, false, false, false)]
		[TestCase(false, false, false, false, false, true, false, false)]
		[TestCase(false, false, false, false, false, true, true, false)]
		[TestCase(false, false, false, false, true, false, false, true)]
		[TestCase(false, false, false, false, true, true, false, false)]
		[TestCase(false, false, false, false, true, true, true, true)]
		[TestCase(false, false, false, true, false, false, false, true)]
		[TestCase(false, false, false, true, false, true, false, false)]
		[TestCase(false, false, false, true, false, true, true, true)]
		[TestCase(false, false, true, false, false, false, false, true)]
		[TestCase(false, false, true, false, false, true, false, false)]
		[TestCase(false, false, true, false, false, true, true, true)]
		[TestCase(false, true, false, false, false, false, false, true)]
		[TestCase(false, true, false, false, false, true, false, false)]
		[TestCase(false, true, false, false, false, true, true, true)]
		[TestCase(true, false, false, false, false, false, false, true)]
		[TestCase(true, false, false, false, false, true, false, false)]
		[TestCase(true, false, false, false, false, true, true, true)]
		public void IsUsing_Method_Type(bool matchArguments, bool matchAttributes, bool matchParameters,
			bool matchImplementation, bool matchReturnType, bool setupRecursive, bool usingRecursive, bool expected)
		{
			var source = new Mock<IMethod>();
			var target = new Mock<IType>();

			target.Setup(t => t.Equals(target.Object)).Returns(true);

			IType resolve;

			if (setupRecursive)
			{
				var indirectArgument = new Mock<IArgument>();
				var indirectMethod = new Mock<IMethod>();
				var indirectType = new Mock<IType>();

				indirectArgument.Setup(a => a.Type).Returns(target.Object);
				indirectMethod.Setup(i => i.Arguments).Returns(new[] {indirectArgument.Object});
				indirectMethod.Setup(i => i.ReturnType).Returns(Mock.Of<IType>());
				indirectType.Setup(t => t.Methods).Returns(new[] {indirectMethod.Object});

				resolve = indirectType.Object;
			}
			else
				resolve = target.Object;

			var argument = new Mock<IArgument>();

			argument.Setup(a => a.Type).Returns(matchArguments ? resolve : Mock.Of<IType>());
			source.Setup(t => t.Arguments).Returns(new[] {argument.Object});

			var attribute = new Mock<IAttribute>();
			var attributeMethod = new Mock<IMethod>();

			attributeMethod.Setup(m => m.ReturnType).Returns(Mock.Of<IType>());
			attribute.Setup(a => a.Constructor).Returns(attributeMethod.Object);
			attribute.Setup(a => a.Type).Returns(matchAttributes ? resolve : Mock.Of<IType>());
			source.Setup(t => t.Attributes).Returns(new[] {attribute.Object});

			var parameter = new Mock<IParameter>();

			parameter.Setup(p => p.Constraints).Returns(new[] {matchParameters ? resolve : Mock.Of<IType>()});
			source.Setup(t => t.Parameters).Returns(new[] {parameter.Object});

			var implementation = new Mock<IImplementation>();

			implementation.Setup(b => b.ReferencedTypes).Returns(new[] {resolve});
			source.Setup(t => t.ImplementationOrNull).Returns(matchImplementation ? implementation.Object : null);
			source.Setup(t => t.ReturnType).Returns(matchReturnType ? resolve : Mock.Of<IType>());

			Assert.That(source.Object.IsUsing(target.Object, usingRecursive), Is.EqualTo(expected));
		}

		[Test]
		[TestCase(false, false, false, false)]
		[TestCase(false, true, false, false)]
		[TestCase(false, true, true, false)]
		[TestCase(true, false, false, true)]
		[TestCase(true, true, false, false)]
		[TestCase(true, true, true, true)]
		public void IsUsing_Type_Method(bool matchMethods, bool setupRecursive, bool usingRecursive, bool expected)
		{
			var source = new Mock<IType>();
			var target = new Mock<IMethod>();

			target.Setup(t => t.Equals(target.Object)).Returns(true);

			IMethod resolve;

			if (setupRecursive)
			{
				var indirect = new Mock<IMethod>();
				var indirectImplementation = new Mock<IImplementation>();

				indirectImplementation.Setup(b => b.ReferencedMethods).Returns(new[] {target.Object});
				indirect.Setup(t => t.ImplementationOrNull).Returns(indirectImplementation.Object);

				resolve = indirect.Object;
			}
			else
				resolve = target.Object;
			
			source.Setup(t => t.Methods).Returns(new[] {matchMethods ? resolve : Mock.Of<IMethod>()});

			Assert.That(source.Object.IsUsing(target.Object, usingRecursive), Is.EqualTo(expected));
		}

		[Test]
		[TestCase(false, false, false, false, false, false, false, false, false)]
		[TestCase(false, false, false, false, false, false, false, false, false)]
		[TestCase(false, false, false, false, false, false, true, false, false)]
		[TestCase(false, false, false, false, false, false, true, false, false)]
		[TestCase(false, false, false, false, false, false, true, true, false)]
		[TestCase(false, false, false, false, false, false, true, true, false)]
		[TestCase(false, false, false, false, false, true, false, false, true)]
		[TestCase(false, false, false, false, false, true, true, false, false)]
		[TestCase(false, false, false, false, false, true, true, true, true)]
		[TestCase(false, false, false, false, true, false, false, false, true)]
		[TestCase(false, false, false, false, true, false, true, false, false)]
		[TestCase(false, false, false, false, true, false, true, true, true)]
		[TestCase(false, false, false, true, false, false, false, false, true)]
		[TestCase(false, false, false, true, false, false, true, false, false)]
		[TestCase(false, false, false, true, false, false, true, true, true)]
		[TestCase(false, false, true, false, false, false, false, false, true)]
		[TestCase(false, false, true, false, false, false, true, false, false)]
		[TestCase(false, false, true, false, false, false, true, true, true)]
		[TestCase(false, true, false, false, false, false, false, false, true)]
		[TestCase(false, true, false, false, false, false, true, false, false)]
		[TestCase(false, true, false, false, false, false, true, true, true)]
		[TestCase(true, false, false, false, false, false, false, false, true)]
		[TestCase(true, false, false, false, false, false, true, false, false)]
		[TestCase(true, false, false, false, false, false, true, true, true)]
		public void IsUsing_Type_Type(bool matchAttributes, bool matchBaseOrNull, bool matchFields,
			bool matchInterfaces, bool matchNestedTypes, bool matchParameters, bool setupRecursive, bool usingRecursive,
			bool expected)
		{
			var source = new Mock<IType>();
			var target = new Mock<IType>();

			target.Setup(t => t.Equals(target.Object)).Returns(true);

			IType resolve;

			if (setupRecursive)
			{
				var indirect = new Mock<IType>();
				var indirectAttribute = new Mock<IAttribute>();
				var indirectAttributeConstructor = new Mock<IMethod>();

				indirectAttributeConstructor.Setup(m => m.ReturnType).Returns(Mock.Of<IType>());
				indirectAttribute.Setup(a => a.Constructor).Returns(indirectAttributeConstructor.Object);
				indirectAttribute.Setup(a => a.Type).Returns(target.Object);
				indirect.Setup(i => i.Attributes).Returns(new[] {indirectAttribute.Object});

				resolve = indirect.Object;
			}
			else
				resolve = target.Object;

			var attribute = new Mock<IAttribute>();
			var attributeConstructor = new Mock<IMethod>();

			attributeConstructor.Setup(m => m.ReturnType).Returns(Mock.Of<IType>());
			attribute.Setup(a => a.Constructor).Returns(attributeConstructor.Object);
			attribute.Setup(a => a.Type).Returns(matchAttributes ? resolve : Mock.Of<IType>());
			source.Setup(t => t.Attributes).Returns(new[] {attribute.Object});
			source.Setup(t => t.BaseOrNull).Returns(matchBaseOrNull ? resolve : null);

			var field = new Mock<IField>();

			field.Setup(a => a.Type).Returns(matchFields ? resolve : Mock.Of<IType>());
			source.Setup(t => t.Fields).Returns(new[] {field.Object});
			source.Setup(t => t.Interfaces).Returns(matchInterfaces ? new[] {resolve} : Array.Empty<IType>());
			source.Setup(t => t.NestedTypes).Returns(matchNestedTypes ? new[] {resolve} : Array.Empty<IType>());

			var parameter = new Mock<IParameter>();

			parameter.Setup(p => p.Constraints).Returns(new[] {matchParameters ? resolve : Mock.Of<IType>()});
			source.Setup(t => t.Parameters).Returns(new[] {parameter.Object});

			var method = new Mock<IMethod>();

			method.Setup(m => m.ReturnType).Returns(Mock.Of<IType>());
			source.Setup(t => t.Methods).Returns(new[] {method.Object});

			Assert.That(source.Object.IsUsing(target.Object, usingRecursive), Is.EqualTo(expected));
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