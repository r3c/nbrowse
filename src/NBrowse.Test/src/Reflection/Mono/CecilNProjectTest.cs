using System;
using System.Reflection;
using NBrowse.Reflection;
using NBrowse.Reflection.Mono;
using NUnit.Framework;

// ReSharper disable UnusedMember.Global

namespace NBrowse.Test.Reflection.Mono
{
    public class CecilNProjectTest
    {
        private static readonly string AssemblyName = typeof(CecilNProjectTest).Assembly.GetName().Name;

        [Test]
        public void Assemblies()
        {
            var project = CreateProject();

            Assert.That(project.Assemblies, Has.Some.Matches<NAssembly>(a => a.Name == AssemblyName));
        }

        [Test]
        public void FilterAssembly_Exclude()
        {
            var project = CreateProject();

            Assert.That(project.FilterAssemblies(new[] { "Missing" }), Is.Empty);
        }

        [Test]
        public void FilterAssembly_Include()
        {
            var project = CreateProject();

            Assert.That(project.FilterAssemblies(new[] { "Missing1", AssemblyName, "Missing2" }),
                Has.Some.Matches<NAssembly>(a => a.Name == AssemblyName));
        }

        [Test]
        public void FindAssembly_ByName_Missing()
        {
            var project = CreateProject();

            Assert.Throws<ArgumentOutOfRangeException>(() => project.FindAssembly("Missing"));
        }

        [Test]
        public void FindAssembly_ByName_Unique()
        {
            var project = CreateProject();
            var assembly = project.FindAssembly(AssemblyName);

            Assert.That(assembly.Name, Is.EqualTo(AssemblyName));
        }

        [Test]
        public void FindMethod_ByFullName_Conflict()
        {
            var project = CreateProject();

            Assert.Throws<AmbiguousMatchException>(() =>
                project.FindMethod("NBrowse.Test.Namespace1.Conflict.ConflictOnArguments"));
        }

        [Test]
        public void FindMethod_ByFullName_Missing()
        {
            var project = CreateProject();

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                project.FindMethod("NBrowse.Test.Namespace1.Conflict.Missing"));
        }

        [Test]
        public void FindMethod_ByFullName_Unique()
        {
            var project = CreateProject();
            var method = project.FindMethod("NBrowse.Test.Namespace2.Conflict.NoConflict");

            Assert.That(method.Name, Is.EqualTo("NoConflict"));
        }

        [Test]
        public void FindMethod_ByIdentifier_Missing()
        {
            var project = CreateProject();

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                project.FindMethod("NBrowse.Test.Namespace1.Conflict.ConflictOnArguments(bool)"));
        }

        [Test]
        public void FindMethod_ByIdentifier_Unique()
        {
            var project = CreateProject();
            var method = project.FindMethod("NBrowse.Test.Namespace1.Conflict.ConflictOnArguments()");

            Assert.That(method.Name, Is.EqualTo("ConflictOnArguments"));
        }

        [Test]
        public void FindMethod_ByName_Conflict()
        {
            var project = CreateProject();

            Assert.Throws<AmbiguousMatchException>(() => project.FindMethod("ConflictOnName"));
        }

        [Test]
        public void FindMethod_ByName_Missing()
        {
            var project = CreateProject();

            Assert.Throws<ArgumentOutOfRangeException>(() => project.FindMethod("Missing"));
        }

        [Test]
        public void FindMethod_ByName_Unique()
        {
            var project = CreateProject();
            var method = project.FindMethod("NoConflict");

            Assert.That(method.Name, Is.EqualTo("NoConflict"));
        }

        [Test]
        public void FindMethod_ByParent_Conflict()
        {
            var project = CreateProject();

            Assert.Throws<AmbiguousMatchException>(() => project.FindMethod("Conflict.ConflictOnName"));
        }

        [Test]
        public void FindMethod_ByParent_Missing()
        {
            var project = CreateProject();

            Assert.Throws<ArgumentOutOfRangeException>(() => project.FindMethod("Conflict.Missing"));
        }

        [Test]
        public void FindMethod_ByParent_Unique()
        {
            var project = CreateProject();
            var method = project.FindMethod("Unique.ConflictOnName");

            Assert.That(method.Name, Is.EqualTo("ConflictOnName"));
        }

        [Test]
        public void FindType_ByIdentifier_Unique()
        {
            var project = CreateProject();
            var type = project.FindType("NBrowse.Test.Namespace1.Conflict");

            Assert.That(type.Name, Is.EqualTo("Conflict"));
        }

        [Test]
        public void FindType_ByName_Conflict()
        {
            var project = CreateProject();

            Assert.Throws<AmbiguousMatchException>(() => project.FindType("Conflict"));
        }

        [Test]
        public void FindType_ByName_Missing()
        {
            var project = CreateProject();

            Assert.Throws<ArgumentOutOfRangeException>(() => project.FindType("Missing"));
        }

        [Test]
        public void FindType_ByName_Unique()
        {
            var project = CreateProject();
            var type = project.FindType("Unique");

            Assert.That(type.Name, Is.EqualTo("Unique"));
        }

        public static NProject CreateProject()
        {
            return new CecilNProject(new[] { typeof(CecilNProjectTest).Assembly.Location });
        }
    }
}

namespace NBrowse.Test.Namespace1
{
    public abstract class Conflict
    {
        public abstract void ConflictOnArguments(int a);

        public abstract void ConflictOnArguments();

        public abstract void ConflictOnName();
    }

    public abstract class Unique
    {
        public abstract void ConflictOnName();
    }
}

namespace NBrowse.Test.Namespace2
{
    public abstract class Conflict
    {
        public abstract void ConflictOnName();

        public abstract void NoConflict();
    }
}