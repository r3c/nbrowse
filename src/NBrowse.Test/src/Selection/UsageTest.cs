using System;
using Moq;
using NBrowse.Reflection;
using NBrowse.Selection;
using NBrowse.Test.Reflection.Mono;
using NUnit.Framework;

namespace NBrowse.Test.Selection;

public class UsageTest
{
    private static NImplementation EmptyNImplementation;
    private static NMethod EmptyNMethod;
    private static NType EmptyNType;

    [OneTimeSetUp]
    public void Setup()
    {
        var implementation = new Mock<NImplementation>();

        implementation.Setup(i => i.ReferencedMethods).Returns(Array.Empty<NMethod>());
        implementation.Setup(i => i.ReferencedTypes).Returns(Array.Empty<NType>());

        EmptyNImplementation = implementation.Object;

        var method = new Mock<NMethod>();

        method.Setup(m => m.NImplementation).Returns(implementation.Object);

        EmptyNMethod = method.Object;
        EmptyNType = Mock.Of<NType>();
    }

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
        var source = new Mock<NMethod>();
        var target = new Mock<NMethod>();

        target.Setup(t => t.Equals(target.Object)).Returns(true);

        NMethod resolve;

        if (setupRecursive)
        {
            var indirect = new Mock<NMethod>();
            var indirectImplementation = new Mock<NImplementation>();

            indirectImplementation.Setup(i => i.ReferencedMethods).Returns(new[] { target.Object });
            indirect.Setup(i => i.NImplementation).Returns(indirectImplementation.Object);

            resolve = indirect.Object;
        }
        else
            resolve = target.Object;

        var implementation = new Mock<NImplementation>();

        implementation.Setup(i => i.ReferencedMethods)
            .Returns(matchImplementation ? new[] { resolve } : Array.Empty<NMethod>());
        implementation.Setup(i => i.ReferencedTypes).Returns(Array.Empty<NType>());

        source.Setup(t => t.NImplementation).Returns(implementation.Object);

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
        var source = new Mock<NMethod>();
        var target = new Mock<NType>();

        target.Setup(t => t.Equals(target.Object)).Returns(true);

        NType resolve;

        if (setupRecursive)
        {
            var indirectArgument = new Mock<NArgument>();
            var indirectMethod = new Mock<NMethod>();
            var indirectType = new Mock<NType>();

            indirectArgument.Setup(a => a.NType).Returns(target.Object);
            indirectMethod.Setup(i => i.Arguments).Returns(new[] { indirectArgument.Object });
            indirectMethod.Setup(i => i.ReturnNType).Returns(EmptyNType);
            indirectType.Setup(t => t.Methods).Returns(new[] { indirectMethod.Object });

            resolve = indirectType.Object;
        }
        else
            resolve = target.Object;

        var argument = new Mock<NArgument>();

        argument.Setup(a => a.NType).Returns(matchArguments ? resolve : EmptyNType);
        source.Setup(t => t.Arguments).Returns(new[] { argument.Object });

        var attribute = new Mock<NBrowse.Reflection.NAttribute>();
        var attributeMethod = new Mock<NMethod>();

        attributeMethod.Setup(m => m.NImplementation).Returns(EmptyNImplementation);
        attributeMethod.Setup(m => m.ReturnNType).Returns(EmptyNType);
        attribute.Setup(a => a.Constructor).Returns(attributeMethod.Object);
        attribute.Setup(a => a.NType).Returns(matchAttributes ? resolve : EmptyNType);
        source.Setup(t => t.Attributes).Returns(new[] { attribute.Object });

        var parameter = new Mock<NParameter>();

        parameter.Setup(p => p.Constraints).Returns(new[] { matchParameters ? resolve : EmptyNType });
        source.Setup(t => t.Parameters).Returns(new[] { parameter.Object });

        var implementation = new Mock<NImplementation>();

        implementation.Setup(i => i.ReferencedTypes)
            .Returns(matchImplementation ? new[] { resolve } : Array.Empty<NType>());

        source.Setup(t => t.NImplementation).Returns(implementation.Object);
        source.Setup(t => t.ReturnNType).Returns(matchReturnType ? resolve : EmptyNType);

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
        var source = new Mock<NType>();
        var target = new Mock<NMethod>();

        target.Setup(t => t.Equals(target.Object)).Returns(true);

        NMethod resolve;

        if (setupRecursive)
        {
            var indirect = new Mock<NMethod>();
            var indirectImplementation = new Mock<NImplementation>();

            indirectImplementation.Setup(i => i.ReferencedMethods).Returns(new[] { target.Object });
            indirect.Setup(t => t.NImplementation).Returns(indirectImplementation.Object);

            resolve = indirect.Object;
        }
        else
            resolve = target.Object;

        source.Setup(t => t.Methods).Returns(new[] { matchMethods ? resolve : EmptyNMethod });

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
        var source = new Mock<NType>();
        var target = new Mock<NType>();

        target.Setup(t => t.Equals(target.Object)).Returns(true);

        NType resolve;

        if (setupRecursive)
        {
            var indirect = new Mock<NType>();
            var indirectAttribute = new Mock<NBrowse.Reflection.NAttribute>();
            var indirectAttributeConstructor = new Mock<NMethod>();

            indirectAttributeConstructor.Setup(m => m.NImplementation).Returns(EmptyNImplementation);
            indirectAttributeConstructor.Setup(m => m.ReturnNType).Returns(EmptyNType);
            indirectAttribute.Setup(a => a.Constructor).Returns(indirectAttributeConstructor.Object);
            indirectAttribute.Setup(a => a.NType).Returns(target.Object);
            indirect.Setup(i => i.Attributes).Returns(new[] { indirectAttribute.Object });

            resolve = indirect.Object;
        }
        else
            resolve = target.Object;

        var attribute = new Mock<NBrowse.Reflection.NAttribute>();
        var attributeConstructor = new Mock<NMethod>();

        attributeConstructor.Setup(m => m.NImplementation).Returns(EmptyNImplementation);
        attributeConstructor.Setup(m => m.ReturnNType).Returns(EmptyNType);
        attribute.Setup(a => a.Constructor).Returns(attributeConstructor.Object);
        attribute.Setup(a => a.NType).Returns(matchAttributes ? resolve : EmptyNType);
        source.Setup(t => t.Attributes).Returns(new[] { attribute.Object });
        source.Setup(t => t.BaseOrNull).Returns(matchBaseOrNull ? resolve : null);

        var field = new Mock<NField>();

        field.Setup(a => a.NType).Returns(matchFields ? resolve : EmptyNType);
        source.Setup(t => t.Fields).Returns(new[] { field.Object });
        source.Setup(t => t.Interfaces).Returns(matchInterfaces ? new[] { resolve } : Array.Empty<NType>());
        source.Setup(t => t.NestedTypes).Returns(matchNestedTypes ? new[] { resolve } : Array.Empty<NType>());

        var parameter = new Mock<NParameter>();

        parameter.Setup(p => p.Constraints).Returns(new[] { matchParameters ? resolve : EmptyNType });
        source.Setup(t => t.Parameters).Returns(new[] { parameter.Object });

        var method = new Mock<NMethod>();

        method.Setup(m => m.NImplementation).Returns(EmptyNImplementation);
        method.Setup(m => m.ReturnNType).Returns(EmptyNType);
        source.Setup(t => t.Methods).Returns(new[] { method.Object });

        Assert.That(source.Object.IsUsing(target.Object, usingRecursive), Is.EqualTo(expected));
    }

    [Test]
    [TestCase(128, 1)]
    [TestCase(0, 2)]
    public void IsUsing_Type_Type_WithCache(int cacheSize, int expectedComparisons)
    {
        // Force flush cache by setting size to zero and asking for usage of some type not previously used
        var fake = CecilNProjectTest.CreateProject().FindType(typeof(FakeType).FullName);

        Usage.CacheSize = 0;

        fake.IsUsing(fake);

        Usage.CacheSize = cacheSize;

        // Configure mocks to equal themselves so they pass cache verification
        var source = new Mock<NType>();
        var target = new Mock<NType>();

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