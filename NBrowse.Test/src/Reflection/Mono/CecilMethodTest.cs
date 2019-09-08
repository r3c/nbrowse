using System;
using NBrowse.Reflection.Mono;
using NUnit.Framework;

namespace NBrowse.Test.Reflection.Mono
{
	public class CecilMethodTest
	{
		[Test]
		public void IsUsing()
		{
			var project = CecilMethodTest.CreateProject();

			var caller = project.FindMethod("CecilMethodCaller");
			var useAsArgument = project.FindMethod("CecilMethodCalleeArgument");
			var useAsInvoke = project.FindMethod("CecilMethodCalleeInvoke");
			var noUse = project.FindMethod("CecilMethodDummy");

			Assert.That(caller.IsUsing(useAsArgument), Is.True);
			Assert.That(caller.IsUsing(useAsInvoke), Is.True);
			Assert.That(caller.IsUsing(noUse), Is.False);
		}

		private static CecilProject CreateProject()
		{
			return new CecilProject(new[] {typeof(CecilMethodTest).Assembly.Location});
		}

		public abstract class TestClass
		{
			public void CecilMethodCaller()
			{
				TestClass.CecilMethodCalleeInvoke();

				Action action = TestClass.CecilMethodCalleeArgument;

				if (action == null)
					throw new Exception();
			}

			private static void CecilMethodCalleeArgument()
			{
			}

			private static void CecilMethodCalleeInvoke()
			{
			}

			private static void CecilMethodDummy()
			{
			}
		}
	}
}