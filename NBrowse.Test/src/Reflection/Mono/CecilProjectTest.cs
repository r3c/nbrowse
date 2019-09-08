using System;
using NBrowse.Reflection;
using NBrowse.Reflection.Mono;
using NUnit.Framework;

namespace NBrowse.Test.Reflection.Mono
{
	public class CecilProjectTest
	{
		[Test]
		public void FindAssembly_ByName_Missing()
		{
			var project = CecilProjectTest.CreateProject();

			Assert.Throws<ArgumentOutOfRangeException>(() => project.FindAssembly("Missing"));
		}

		[Test]
		public void FindAssembly_ByName_Unique()
		{
			var project = CecilProjectTest.CreateProject();
			var assembly = project.FindAssembly("NBrowse.Test");

			Assert.That(assembly.Name, Is.EqualTo("NBrowse.Test"));
		}

		[Test]
		public void FindMethod_ByIdentifier_Unique()
		{
			var project = CecilProjectTest.CreateProject();
			var method = project.FindMethod("NBrowse.Test.Namespace2.Conflict.UniqueMethod()");

			Assert.That(method.Name, Is.EqualTo("UniqueMethod"));
		}

		[Test]
		public void FindMethod_ByName_Conflict()
		{
			var project = CecilProjectTest.CreateProject();

			Assert.Throws<ArgumentOutOfRangeException>(() => project.FindMethod("Conflict()"));
		}

		[Test]
		public void FindMethod_ByName_Missing()
		{
			var project = CecilProjectTest.CreateProject();

			Assert.Throws<ArgumentOutOfRangeException>(() => project.FindMethod("Missing()"));
		}

		[Test]
		public void FindMethod_ByName_Unique()
		{
			var project = CecilProjectTest.CreateProject();
			var method = project.FindMethod("UniqueMethod");

			Assert.That(method.Name, Is.EqualTo("UniqueMethod"));
		}

		[Test]
		public void FindType_ByIdentifier_Unique()
		{
			var project = CecilProjectTest.CreateProject();
			var type = project.FindType("NBrowse.Test.Namespace1.Conflict");

			Assert.That(type.Name, Is.EqualTo("Conflict"));
		}

		[Test]
		public void FindType_ByName_Conflict()
		{
			var project = CecilProjectTest.CreateProject();

			Assert.Throws<ArgumentOutOfRangeException>(() => project.FindType("Conflict"));
		}

		[Test]
		public void FindType_ByName_Missing()
		{
			var project = CecilProjectTest.CreateProject();

			Assert.Throws<ArgumentOutOfRangeException>(() => project.FindType("Missing"));
		}

		[Test]
		public void FindType_ByName_Unique()
		{
			var project = CecilProjectTest.CreateProject();
			var type = project.FindType("Unique");

			Assert.That(type.Name, Is.EqualTo("Unique"));
		}

		public static IProject CreateProject()
		{
			return new CecilProject(new[] {typeof(CecilProjectTest).Assembly.Location});
		}
	}
}

namespace NBrowse.Test.Namespace1
{
	public abstract class Conflict
	{
		public abstract void ConflictMethod();
	}

	public abstract class Unique
	{
		public abstract void ConflictMethod();
	}
}

namespace NBrowse.Test.Namespace2
{
	public abstract class Conflict
	{
		public abstract void UniqueMethod();
	}
}