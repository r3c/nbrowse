using System;
using Mono.Cecil;
using NBrowse.Reflection;
using NUnit.Framework;

namespace NBrowse.Test
{
    public class ProjectTest
    {
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
            var assembly = project.FindAssembly("NBrowse.Test");

            Assert.That(assembly.Name, Is.EqualTo("NBrowse.Test"));
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

            Assert.Throws<ArgumentOutOfRangeException>(() => project.FindType("Conflict"));
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

        private static Project CreateProject()
        {
            return new Project(new []
            {
                new Assembly(AssemblyDefinition.ReadAssembly(typeof(ProjectTest).Assembly.Location))
            });
        }
    }
}

namespace NBrowse.Test.Namespace1
{
    public class Conflict {}
    public class Unique {}
}

namespace NBrowse.Test.Namespace2
{
    public class Conflict {}
}