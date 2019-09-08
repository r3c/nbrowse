using System.Linq;
using NUnit.Framework;

namespace NBrowse.Test.Reflection.Mono
{
	public class CecilArgumentTest
	{
		[Test]
		public void HasDefaultValue()
		{
			var project = CecilProjectTest.CreateProject();
			var method = project.FindMethod("CecilArgumentMethod");
			var arguments = method.Arguments.ToArray();

			Assert.That(arguments[0].HasDefaultValue, Is.False);
			Assert.That(arguments[1].HasDefaultValue, Is.True);
		}

		public class TestClass
		{
			public void CecilArgumentMethod(int a, int b = 3)
			{
			}
		}
	}
}