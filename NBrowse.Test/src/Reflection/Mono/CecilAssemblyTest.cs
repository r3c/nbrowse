using System.Linq;
using System.Runtime.Versioning;
using NBrowse.Reflection;
using NUnit.Framework;

namespace NBrowse.Test.Reflection.Mono
{
	public class CecilAssemblyTest
	{
		[Test]
		public void Attributes()
		{
			Assert.That(CecilAssemblyTest.GetAssembly().Attributes,
				Has.Some.Matches<IAttribute>(attribute =>
					attribute.Type.Identifier == typeof(TargetFrameworkAttribute).FullName));
		}

		[Test]
		public void Culture()
		{
			Assert.That(CecilAssemblyTest.GetAssembly().Culture,
				Is.EqualTo(typeof(CecilAssemblyTest).Assembly.GetName().CultureName));
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
				Has.Some.Matches<IType>(t => t.Identifier == typeof(CecilAssemblyTest).FullName));
		}

		[Test]
		public void Version()
		{
			var expected = typeof(CecilAssemblyTest).Assembly.GetName().Version;

			Assert.That(CecilAssemblyTest.GetAssembly().Version, Is.EqualTo(expected));
		}

		private static IAssembly GetAssembly()
		{
			return CecilProjectTest.CreateProject().FindAssembly(typeof(CecilAssemblyTest).Assembly.GetName().Name);
		}
	}
}