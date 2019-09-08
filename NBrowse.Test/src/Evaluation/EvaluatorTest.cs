using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NBrowse.Evaluation;
using NBrowse.Reflection;
using NUnit.Framework;

namespace NBrowse.Test.Evaluation
{
	public class EvaluatorTest
	{
		[Test]
		[TestCase("project => 42", 42)]
		[TestCase("project => \"Hello, World!\"", "Hello, World!")]
		public async Task Query_Constant_ReturnLiteral<T>(string expression, T expected)
		{
			Assert.That(await EvaluatorTest.CreateAndQuery<T>(expression), Is.EqualTo(expected));
		}

		[Test]
		public async Task Query_Has_TypeCustomAttribute()
		{
			Assert.That(await EvaluatorTest.CreateAndQuery<bool>($"project => Has.Attribute<System.Runtime.CompilerServices.CompilerGeneratedAttribute>(project.FindType(\"{nameof(EvaluatorTest)}+{nameof(InternalStructure)}\"))"), Is.True);
		}

		[Test]
		public async Task Query_Is_TypeGenerated()
		{
			Assert.That(await EvaluatorTest.CreateAndQuery<bool>($"project => Is.Generated(project.FindType(\"{nameof(EvaluatorTest)}+{nameof(InternalStructure)}\"))"), Is.True);
		}

		[Test]
		public async Task Query_Project_FilterAssemblies()
		{
			var assemblies = await EvaluatorTest.CreateAndQuery<IAssembly[]>($"project => project.FilterAssemblies(new [] {{\"Missing1\", \"{typeof(EvaluatorTest).Assembly.GetName().Name}\", \"Missing2\"}}).ToArray()");

			Assert.That(assemblies.Length, Is.EqualTo(1));
			Assert.That(assemblies[0].Name, Is.EqualTo(typeof(EvaluatorTest).Assembly.GetName().Name));
		}

		[Test]
		public async Task Query_Project_FindExistingAssembly()
		{
			var assembly = await EvaluatorTest.CreateAndQuery<IAssembly>($"project => project.FindAssembly(\"{typeof(EvaluatorTest).Assembly.GetName().Name}\")");

			StringAssert.EndsWith("NBrowse.Test.dll", assembly.FileName);
		}

		[Test]
		public void Query_Project_FindMissingAssembly()
		{
			Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await EvaluatorTest.CreateAndQuery<IAssembly>("project => project.FindAssembly(\"DoesNotExist\")"));
		}

		[Test]
		public async Task Query_Project_FindExistingTypeByIdentifier()
		{
			var type = await EvaluatorTest.CreateAndQuery<IType>($"project => project.FindType(\"{typeof(EvaluatorTest).FullName}\")");

			Assert.That(type.Identifier, Is.EqualTo(typeof(EvaluatorTest).FullName));
		}

		[Test]
		public async Task Query_Project_FindExistingTypeByName()
		{
			var type = await EvaluatorTest.CreateAndQuery<IType>("project => project.FindType(\"" + nameof(EvaluatorTest) + "\")");

			Assert.That(type.Identifier, Is.EqualTo(typeof(EvaluatorTest).FullName));
		}

		[Test]
		public void Query_Project_FindMissingType()
		{
			Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await EvaluatorTest.CreateAndQuery<IType>("project => project.FindType(\"DoesNotExist\")"));
		}

		[Test]
		public async Task Query_Type_ClassWithInterfaces()
		{
			var names = await EvaluatorTest.CreateAndQuery<string[]>($"project => project.FindType(\"{nameof(EvaluatorTest)}+{nameof(ClassWithInterfaces)}\").Interfaces.Select(i => i.Name).ToArray()");

			CollectionAssert.AreEquivalent(new[] { "ICloneable", "IDisposable" }, names);
		}

		[Test]
		public async Task Query_Type_InterfaceWithGenericParameter()
		{
			var candidateType = await EvaluatorTest.FindTypeByName($"{nameof(EvaluatorTest)}+{nameof(INterfaceWithGenericParameter<Stream>)}`1");

			var parameters = candidateType.Parameters.ToArray();

			Assert.That(parameters.Length, Is.EqualTo(1));

			Assert.That(parameters[0].HasDefaultConstructor, Is.EqualTo(false));
			Assert.That(parameters[0].Name, Is.EqualTo("T"));
			Assert.That(parameters[0].Variance, Is.EqualTo(Variance.Contravariant));

			var constraints = parameters[0].Constraints.ToArray();

			Assert.That(constraints.Length, Is.EqualTo(1));

			Assert.That(constraints[0].Name, Is.EqualTo("IDisposable"));
		}

		[Test]
		public async Task Query_Type_PrivateClassWithFieldsAndNestedType()
		{
			var candidateType = await EvaluatorTest.FindTypeByName($"{nameof(EvaluatorTest)}+{nameof(PrivateClassWithFields)}");
			var expectedType = typeof(PrivateClassWithFields);

			Assert.That(candidateType.Implementation, Is.EqualTo(Implementation.Virtual));
			Assert.That(candidateType.Model, Is.EqualTo(Model.Class));
			Assert.That(candidateType.Name, Is.EqualTo($"{nameof(EvaluatorTest)}+{nameof(PrivateClassWithFields)}"));
			Assert.That(candidateType.Parent.Name, Is.EqualTo(expectedType.Assembly.GetName().Name));
			Assert.That(candidateType.Namespace, Is.EqualTo(expectedType.Namespace));
			Assert.That(candidateType.Visibility, Is.EqualTo(Visibility.Private));

			var candidateFields = candidateType.Fields.ToArray();

			Assert.That(candidateFields.Length, Is.EqualTo(4));

			Assert.That(candidateFields[0].Binding, Is.EqualTo(Binding.Instance));
			Assert.That(candidateFields[0].Name, Is.EqualTo("A"));
			Assert.That(candidateFields[0].Type.Name, Is.EqualTo("String"));
			Assert.That(candidateFields[0].Visibility, Is.EqualTo(Visibility.Public));
			Assert.That(candidateFields[0].Binding, Is.EqualTo(Binding.Instance));

			Assert.That(candidateFields[1].Name, Is.EqualTo("B"));
			Assert.That(candidateFields[1].Type.Name, Is.EqualTo("Int32"));
			Assert.That(candidateFields[1].Visibility, Is.EqualTo(Visibility.Protected));
			Assert.That(candidateFields[1].Binding, Is.EqualTo(Binding.Instance));

			Assert.That(candidateFields[2].Name, Is.EqualTo("c"));
			Assert.That(candidateFields[2].Type.Name, Is.EqualTo("Single"));
			Assert.That(candidateFields[2].Visibility, Is.EqualTo(Visibility.Private));
			Assert.That(candidateFields[2].Binding, Is.EqualTo(Binding.Static));

			Assert.That(candidateFields[3].Name, Is.EqualTo("D"));
			Assert.That(candidateFields[3].Type.Name, Is.EqualTo("Int64"));
			Assert.That(candidateFields[3].Visibility, Is.EqualTo(Visibility.Internal));

			var candidateNestedTypes = candidateType.NestedTypes.ToArray();

			Assert.That(candidateNestedTypes.Length, Is.EqualTo(1));

			Assert.That(candidateNestedTypes[0].Name, Is.EqualTo($"{nameof(EvaluatorTest)}+{nameof(PrivateClassWithFields)}+{nameof(PrivateClassWithFields.NestedClass)}"));
		}

		[Test]
		public async Task Query_Type_PrivateClassWithInheritance()
		{
			var candidateType = await EvaluatorTest.FindTypeByName($"{nameof(EvaluatorTest)}+{nameof(InheritFromPrivateClass)}");
			var expectedType = typeof(InheritFromPrivateClass);

			Assert.That(candidateType.Implementation, Is.EqualTo(Implementation.Virtual));
			Assert.That(candidateType.Model, Is.EqualTo(Model.Class));
			Assert.That(candidateType.Name, Is.EqualTo($"{nameof(EvaluatorTest)}+{nameof(InheritFromPrivateClass)}"));
			Assert.That(candidateType.Parent.Name, Is.EqualTo(expectedType.Assembly.GetName().Name));
			Assert.That(candidateType.Namespace, Is.EqualTo(expectedType.Namespace));
			Assert.That(candidateType.Visibility, Is.EqualTo(Visibility.Private));

			var candidateFields = candidateType.Fields.ToArray();

			Assert.That(candidateFields.Count(), Is.EqualTo(1));

			Assert.That(candidateFields[0].Binding, Is.EqualTo(Binding.Instance));
			Assert.That(candidateFields[0].Name, Is.EqualTo("E"));
			Assert.That(candidateFields[0].Type.Name, Is.EqualTo("Byte"));
			Assert.That(candidateFields[0].Visibility, Is.EqualTo(Visibility.Public));
			Assert.That(candidateFields[0].Binding, Is.EqualTo(Binding.Instance));

			Assert.That(candidateType.BaseOrNull, Is.Not.Null);
			Assert.That(candidateType.BaseOrNull.Name, Is.EqualTo($"{nameof(EvaluatorTest)}+{nameof(PrivateClassWithFields)}"));
		}

		[Test]
		public async Task Query_Type_ProtectedDelegate()
		{
			var candidateType = await EvaluatorTest.FindTypeByName($"{nameof(EvaluatorTest)}+{nameof(ProtectedDelegate)}");
			var expectedType = typeof(ProtectedDelegate);

			Assert.That(candidateType.Implementation, Is.EqualTo(Implementation.Final));
			Assert.That(candidateType.Model, Is.EqualTo(Model.Class));
			Assert.That(candidateType.Parent.Name, Is.EqualTo(expectedType.Assembly.GetName().Name));
			Assert.That(candidateType.Namespace, Is.EqualTo(expectedType.Namespace));
			Assert.That(candidateType.Visibility, Is.EqualTo(Visibility.Protected));
		}

		[Test]
		public async Task Query_Type_PublicClassWithMethods()
		{
			var candidateType = await EvaluatorTest.FindTypeByName($"{nameof(EvaluatorTest)}+{nameof(PublicClassWithMethods)}");
			var expectedType = typeof(PublicClassWithMethods);

			Assert.That(candidateType.Implementation, Is.EqualTo(Implementation.Abstract));
			Assert.That(candidateType.Model, Is.EqualTo(Model.Class));
			Assert.That(candidateType.Parent.Name, Is.EqualTo(expectedType.Assembly.GetName().Name));
			Assert.That(candidateType.Namespace, Is.EqualTo(expectedType.Namespace));
			Assert.That(candidateType.Visibility, Is.EqualTo(Visibility.Public));

			var candidateMethods = candidateType.Methods.ToArray();

			Assert.That(candidateMethods.Length, Is.EqualTo(8));

			Assert.That(candidateMethods[0].Binding, Is.EqualTo(Binding.Constructor));
			Assert.That(candidateMethods[0].Implementation, Is.EqualTo(Implementation.Concrete));
			Assert.That(candidateMethods[0].Name, Is.EqualTo(".ctor"));
			Assert.That(candidateMethods[0].ReturnType.Name, Is.EqualTo("Void"));
			Assert.That(candidateMethods[0].Visibility, Is.EqualTo(Visibility.Public));

			var candidateMethodArguments = candidateMethods[0].Arguments.ToArray();

			Assert.That(candidateMethodArguments.Length, Is.EqualTo(1));
			Assert.That(candidateMethodArguments[0].Name, Is.EqualTo("index"));
			Assert.That(candidateMethodArguments[0].Type.Name, Is.EqualTo("Int32"));

			Assert.That(candidateMethods[1].Binding, Is.EqualTo(Binding.Instance));
			Assert.That(candidateMethods[1].Implementation, Is.EqualTo(Implementation.Final));
			Assert.That(candidateMethods[1].Name, Is.EqualTo("GetHashCode"));
			Assert.That(candidateMethods[1].ReturnType.Name, Is.EqualTo("Int32"));
			Assert.That(candidateMethods[1].Visibility, Is.EqualTo(Visibility.Public));

			Assert.That(candidateMethods[2].Binding, Is.EqualTo(Binding.Instance));
			Assert.That(candidateMethods[2].Implementation, Is.EqualTo(Implementation.Virtual));
			Assert.That(candidateMethods[2].Name, Is.EqualTo("ToString"));
			Assert.That(candidateMethods[2].ReturnType.Name, Is.EqualTo("String"));
			Assert.That(candidateMethods[2].Visibility, Is.EqualTo(Visibility.Public));

			Assert.That(candidateMethods[3].Binding, Is.EqualTo(Binding.Instance));
			Assert.That(candidateMethods[3].Implementation, Is.EqualTo(Implementation.Concrete));
			Assert.That(candidateMethods[3].Name, Is.EqualTo("PublicMethodWithCallToOthers"));
			Assert.That(candidateMethods[3].ReturnType.Name, Is.EqualTo("Void"));
			Assert.That(candidateMethods[3].Visibility, Is.EqualTo(Visibility.Public));

			Assert.That(candidateMethods[4].Binding, Is.EqualTo(Binding.Instance));
			Assert.That(candidateMethods[4].Implementation, Is.EqualTo(Implementation.Virtual));
			Assert.That(candidateMethods[4].Name, Is.EqualTo("ProtectedVirtualDynamicMethod"));
			Assert.That(candidateMethods[4].ReturnType.Name, Is.EqualTo("TimeSpan"));
			Assert.That(candidateMethods[4].Visibility, Is.EqualTo(Visibility.Protected));

			Assert.That(candidateMethods[5].Binding, Is.EqualTo(Binding.Static));
			Assert.That(candidateMethods[5].Implementation, Is.EqualTo(Implementation.Concrete));
			Assert.That(candidateMethods[5].Name, Is.EqualTo("PrivateStaticMethodCalled"));
			Assert.That(candidateMethods[5].ReturnType.Name, Is.EqualTo("DateTime"));
			Assert.That(candidateMethods[5].Visibility, Is.EqualTo(Visibility.Private));

			Assert.That(candidateMethods[6].Binding, Is.EqualTo(Binding.Static));
			Assert.That(candidateMethods[6].Implementation, Is.EqualTo(Implementation.Concrete));
			Assert.That(candidateMethods[6].Name, Is.EqualTo("PrivateStaticMethodNotCalled"));
			Assert.That(candidateMethods[6].ReturnType.Name, Is.EqualTo("Uri"));
			Assert.That(candidateMethods[6].Visibility, Is.EqualTo(Visibility.Private));

			Assert.That(candidateMethods[7].Binding, Is.EqualTo(Binding.Instance));
			Assert.That(candidateMethods[7].Implementation, Is.EqualTo(Implementation.Abstract));
			Assert.That(candidateMethods[7].Name, Is.EqualTo("InternalAbstractMethod"));
			Assert.That(candidateMethods[7].ReturnType.Name, Is.EqualTo("Guid"));
			Assert.That(candidateMethods[7].Visibility, Is.EqualTo(Visibility.Internal));
		}

		[Test]
		public async Task Query_Type_InternalStructure()
		{
			var candidateType = await EvaluatorTest.FindTypeByName($"{nameof(EvaluatorTest)}+{nameof(InternalStructure)}");
			var expectedType = typeof(InternalStructure);

			Assert.That(candidateType.Implementation, Is.EqualTo(Implementation.Final));
			Assert.That(candidateType.Model, Is.EqualTo(Model.Structure));
			Assert.That(candidateType.Parent.Name, Is.EqualTo(expectedType.Assembly.GetName().Name));
			Assert.That(candidateType.Namespace, Is.EqualTo(expectedType.Namespace));
			Assert.That(candidateType.Visibility, Is.EqualTo(Visibility.Internal));
		}

		private static async Task<T> CreateAndQuery<T>(string expression)
		{
			var untyped = await Evaluator.LoadAndEvaluate(new[] { typeof(EvaluatorTest).Assembly.Location }, expression);

			if (untyped is T typed)
				return typed;

			throw new InvalidOperationException("invalid return type");
		}

		private static async Task<IMethod> FindMethodByName(string name)
		{
			var methods = await EvaluatorTest.CreateAndQuery<IMethod[]>($"project => project.Assemblies.SelectMany(a => a.Types).SelectMany(t => t.Methods).Where(m => m.Name == \"{name}\").ToArray()");

			Assert.That(methods.Length, Is.EqualTo(1), $"exactly one method must match name {name}");

			return methods[0];
		}

		private static async Task<IType> FindTypeByName(string name)
		{
			var types = await EvaluatorTest.CreateAndQuery<IType[]>($"project => project.Assemblies.SelectMany(a => a.Types).Where(t => t.Name == \"{name}\").ToArray()");

			Assert.That(types.Length, Is.EqualTo(1), $"exactly one type must match name {name}");
			Assert.That(types[0].Name, Is.EqualTo(name), "inconsistent type name");

			return types[0];
		}

		public class ClassWithInterfaces : ICloneable, IDisposable
		{
			public object Clone()
			{
				throw new NotImplementedException();
			}

			public void Dispose()
			{
				throw new NotImplementedException();
			}
		}

		private interface INterfaceWithGenericParameter<in T> where T : IDisposable
		{
			TU GenericDefaultConstructorMethod<TU>() where TU : new();
			TU GenericValueTypeMethod<TU>() where TU : struct;
		}

		protected delegate int ProtectedDelegate();

		private class PrivateClassWithFields
		{
			public class NestedClass
			{
			}

			public string A = "a";
			protected int B = 1;
			private static float c = 2;
			internal long D = (long)PrivateClassWithFields.c;
		}

		private class InheritFromPrivateClass : PrivateClassWithFields
		{
			public byte E = 4;
		}

		public abstract class PublicClassWithMethods
		{
			public PublicClassWithMethods(int index) { }

			public sealed override int GetHashCode()
			{
				return default(int);
			}

			public override string ToString()
			{
				return default(string);
			}

			public void PublicMethodWithCallToOthers()
			{
				if (this.ProtectedVirtualDynamicMethod() == null)
					throw new Exception();

				Func<DateTime> func = PublicClassWithMethods.PrivateStaticMethodCalled;

				if (func == null)
					throw new Exception();
			}

			protected virtual TimeSpan ProtectedVirtualDynamicMethod()
			{
				return default(TimeSpan);
			}

			private static DateTime PrivateStaticMethodCalled()
			{
				return default(DateTime);
			}

			private static Uri PrivateStaticMethodNotCalled()
			{
				return default(Uri);
			}

			internal abstract Guid InternalAbstractMethod();
		}

		[CompilerGenerated]
		internal struct InternalStructure { }
	}
}