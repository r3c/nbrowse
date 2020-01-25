using System.Linq;
using System.Runtime.Versioning;
using NBrowse.Reflection;
using NBrowse.Reflection.Mono;
using NUnit.Framework;

namespace NBrowse.Test.Reflection.Mono
{
	public class CecilAssemblyTest
	{
		[Test]
		public void Attributes()
		{
			Assert.That(CecilAssemblyTest.GetAssembly().Attributes,
				Has.Some.Matches<Attribute>(attribute =>
					attribute.Type.Identifier == typeof(TargetFrameworkAttribute).FullName));
		}

		[Test]
		public void Culture()
		{
			Assert.That(CecilAssemblyTest.GetAssembly().Culture,
				Is.EqualTo(typeof(CecilAssemblyTest).Assembly.GetName().CultureName));
		}

		[Test]
		[TestCase(false)]
		[TestCase(true)]
		public void Equals(bool same)
		{
			var assembly1 = CecilAssemblyTest.GetAssembly();
			var assembly2 = same ? CecilAssemblyTest.GetAssembly() : CecilAssemblyTest.GetOtherAssembly();

			Assert.That(assembly1.Equals(assembly2), Is.EqualTo(same));
			Assert.That(assembly1 == assembly2, Is.EqualTo(same));
			Assert.That(assembly1 != assembly2, Is.EqualTo(!same));
		}

		[Test]
		public void FileName()
		{
			Assert.That(CecilAssemblyTest.GetAssembly().FileName,
				Is.EqualTo(typeof(CecilAssemblyTest).Assembly.Location));
		}

		[Test]
		public void Name()
		{
			Assert.That(CecilAssemblyTest.GetAssembly().Name,
				Is.EqualTo(typeof(CecilAssemblyTest).Assembly.GetName().Name));
		}

		[Test]
		public void References()
		{
			var references = typeof(CecilAssemblyTest).Assembly.GetReferencedAssemblies().Select(a => a.FullName);

			Assert.That(CecilAssemblyTest.GetAssembly().References.Select(r => r.Identifier),
				Is.EquivalentTo(references));
		}

		[Test]
		public void Types()
		{
			Assert.That(CecilAssemblyTest.GetAssembly().Types,
				Has.Some.Matches<Type>(t => t.Identifier == typeof(CecilAssemblyTest).FullName));
		}

		[Test]
		public void Version()
		{
			var expected = typeof(CecilAssemblyTest).Assembly.GetName().Version;

			Assert.That(CecilAssemblyTest.GetAssembly().Version, Is.EqualTo(expected));
		}

		private static Assembly GetAssembly()
		{
			return CecilProjectTest.CreateProject().FindAssembly(typeof(CecilAssemblyTest).Assembly.GetName().Name);
		}

		private static Assembly GetOtherAssembly()
		{
			var assembly = typeof(CecilProject).Assembly;
			var project = new CecilProject(new[] {assembly.Location});

			return project.FindAssembly(assembly.GetName().Name);
		}
	}
}