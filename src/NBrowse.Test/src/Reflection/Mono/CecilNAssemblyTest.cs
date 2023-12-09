using System.Linq;
using System.Runtime.Versioning;
using NBrowse.Reflection;
using NBrowse.Reflection.Mono;
using NUnit.Framework;

namespace NBrowse.Test.Reflection.Mono;

public class CecilNAssemblyTest
{
    [Test]
    public void Attributes()
    {
        Assert.That(GetAssembly().Attributes,
            Has.Some.Matches<NAttribute>(attribute =>
                attribute.NType.Identifier == typeof(TargetFrameworkAttribute).FullName));
    }

    [Test]
    public void Culture()
    {
        Assert.That(GetAssembly().Culture,
            Is.EqualTo(typeof(CecilNAssemblyTest).Assembly.GetName().CultureName));
    }

    [Test]
    [TestCase(false)]
    [TestCase(true)]
    public void Equals(bool same)
    {
        var assembly1 = GetAssembly();
        var assembly2 = same ? GetAssembly() : GetOtherAssembly();

        Assert.That(assembly1.Equals(assembly2), Is.EqualTo(same));
        Assert.That(assembly1 == assembly2, Is.EqualTo(same));
        Assert.That(assembly1 != assembly2, Is.EqualTo(!same));
    }

    [Test]
    public void FileName()
    {
        Assert.That(GetAssembly().FileName,
            Is.EqualTo(typeof(CecilNAssemblyTest).Assembly.Location));
    }

    [Test]
    public void Name()
    {
        Assert.That(GetAssembly().Name,
            Is.EqualTo(typeof(CecilNAssemblyTest).Assembly.GetName().Name));
    }

    [Test]
    public void References()
    {
        var references = typeof(CecilNAssemblyTest).Assembly.GetReferencedAssemblies().Select(a => a.FullName);

        Assert.That(GetAssembly().References.Select(r => r.Identifier),
            Is.EquivalentTo(references));
    }

    [Test]
    public void Types()
    {
        Assert.That(GetAssembly().Types,
            Has.Some.Matches<NType>(t => t.Identifier == typeof(CecilNAssemblyTest).FullName));
    }

    [Test]
    public void Version()
    {
        var expected = typeof(CecilNAssemblyTest).Assembly.GetName().Version;

        Assert.That(GetAssembly().Version, Is.EqualTo(expected));
    }

    private static NAssembly GetAssembly()
    {
        return CecilNProjectTest.CreateProject().FindAssembly(typeof(CecilNAssemblyTest).Assembly.GetName().Name);
    }

    private static NAssembly GetOtherAssembly()
    {
        var assembly = typeof(CecilNProject).Assembly;
        var project = new CecilNProject(new[] { assembly.Location });

        return project.FindAssembly(assembly.GetName().Name);
    }
}