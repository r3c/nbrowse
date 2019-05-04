using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NBrowse.Reflection;
using NUnit.Framework;

namespace NBrowse.Test
{
	public class RepositoryTest
	{
		[Test]
		[TestCase("project => 42", 42)]
		[TestCase("project => \"Hello, World!\"", "Hello, World!")]
		public async Task Query_Constant_ReturnLiteral<T>(string expression, T expected)
		{
			Assert.That(await RepositoryTest.CreateAndQuery<T>(expression), Is.EqualTo(expected));
		}

		[Test]
		public async Task Query_Has_TypeCustomAttribute()
		{
			Assert.That(await RepositoryTest.CreateAndQuery<bool>($"project => Has.Attribute<System.Runtime.CompilerServices.CompilerGeneratedAttribute>(project.FindType(\"{nameof(RepositoryTest)}+{nameof(InternalStructure)}\"))"), Is.True);
		}

		[Test]
		public async Task Query_Is_TypeGenerated()
		{
			Assert.That(await RepositoryTest.CreateAndQuery<bool>($"project => Is.Generated(project.FindType(\"{nameof(RepositoryTest)}+{nameof(InternalStructure)}\"))"), Is.True);
		}

		[Test]
		public async Task Query_Method_IsUsingMethod()
		{
			var caller = await RepositoryTest.FindMethodByName("PublicMethodWithCallToOthers");
			var directCallee = await RepositoryTest.FindMethodByName("ProtectedVirtualDynamicMethod");
			var indirectCall = await RepositoryTest.FindMethodByName("PrivateStaticMethodCalled");
			var noCall = await RepositoryTest.FindMethodByName("PrivateStaticMethodNotCalled");

			Assert.That(caller.IsUsing(directCallee), Is.True);
			Assert.That(caller.IsUsing(indirectCall), Is.True);
			Assert.That(caller.IsUsing(noCall), Is.False);
		}

		[Test]
		public async Task Query_Method_GenericDefaultConstructorMethod()
		{
			var method = await RepositoryTest.FindMethodByName("GenericDefaultConstructorMethod");
			var parameters = method.Parameters.ToArray();

			Assert.That(parameters.Length, Is.EqualTo(1));

			Assert.That(parameters[0].HasDefaultConstructor, Is.EqualTo(true));
			Assert.That(parameters[0].IsContravariant, Is.EqualTo(false));
			Assert.That(parameters[0].IsCovariant, Is.EqualTo(false));
			Assert.That(parameters[0].Name, Is.EqualTo("TU"));

			var constraints = parameters[0].Constraints.ToArray();

			Assert.That(constraints.Length, Is.EqualTo(0));
		}

		[Test]
		public async Task Query_Method_GenericValueTypeMethod()
		{
			var method = await RepositoryTest.FindMethodByName("GenericValueTypeMethod");
			var parameters = method.Parameters.ToArray();

			Assert.That(parameters.Length, Is.EqualTo(1));

			Assert.That(parameters[0].HasDefaultConstructor, Is.EqualTo(true));
			Assert.That(parameters[0].IsContravariant, Is.EqualTo(false));
			Assert.That(parameters[0].IsCovariant, Is.EqualTo(false));
			Assert.That(parameters[0].Name, Is.EqualTo("TU"));

			var constraints = parameters[0].Constraints.ToArray();

			Assert.That(constraints.Length, Is.EqualTo(1));

			Assert.That(constraints[0].Name, Is.EqualTo("ValueType"));
		}

		[Test]
		public async Task Query_Project_FilterAssemblies()
		{
			var assemblies = await RepositoryTest.CreateAndQuery<Assembly[]>($"project => project.FilterAssemblies(new [] {{\"Missing1\", \"{typeof(RepositoryTest).Assembly.GetName().Name}\", \"Missing2\"}}).ToArray()");

			Assert.That(assemblies.Length, Is.EqualTo(1));
			Assert.That(assemblies[0].Name, Is.EqualTo(typeof(RepositoryTest).Assembly.GetName().Name));
		}

		[Test]
		public async Task Query_Project_FindExistingAssembly()
		{
			var assembly = await RepositoryTest.CreateAndQuery<Assembly>($"project => project.FindAssembly(\"{typeof(RepositoryTest).Assembly.GetName().Name}\")");

			StringAssert.EndsWith("NBrowse.Test.dll", assembly.FileName);
		}

		[Test]
		public void Query_Project_FindMissingAssembly()
		{
			Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await RepositoryTest.CreateAndQuery<Assembly>("project => project.FindAssembly(\"DoesNotExist\")"));
		}

		[Test]
		public async Task Query_Project_FindExistingTypeByIdentifier()
		{
			var type = await RepositoryTest.CreateAndQuery<Reflection.Type>($"project => project.FindType(\"{typeof(RepositoryTest).FullName}\")");

			Assert.That(type.Identifier, Is.EqualTo(typeof(RepositoryTest).FullName));
		}

		[Test]
		public async Task Query_Project_FindExistingTypeByName()
		{
			var type = await RepositoryTest.CreateAndQuery<Reflection.Type>("project => project.FindType(\"RepositoryTest\")");

			Assert.That(type.Identifier, Is.EqualTo(typeof(RepositoryTest).FullName));
		}

		[Test]
		public void Query_Project_FindMissingType()
		{
			Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await RepositoryTest.CreateAndQuery<Reflection.Type>("project => project.FindType(\"DoesNotExist\")"));
		}

		[Test]
		public async Task Query_Type_ClassWithInterfaces()
		{
			var names = await RepositoryTest.CreateAndQuery<string[]>($"project => project.FindType(\"{nameof(RepositoryTest)}+{nameof(ClassWithInterfaces)}\").Interfaces.Select(i => i.Name).ToArray()");

			CollectionAssert.AreEquivalent(new[] { "ICloneable", "IDisposable" }, names);
		}

		[Test]
		public async Task Query_Type_InterfaceWithGenericParameter()
		{
			var candidateType = await RepositoryTest.FindTypeByName($"{nameof(RepositoryTest)}+{nameof(INterfaceWithGenericParameter<Stream>)}`1");

			var parameters = candidateType.Parameters.ToArray();

			Assert.That(parameters.Length, Is.EqualTo(1));

			Assert.That(parameters[0].HasDefaultConstructor, Is.EqualTo(false));
			Assert.That(parameters[0].IsContravariant, Is.EqualTo(true));
			Assert.That(parameters[0].IsCovariant, Is.EqualTo(false));
			Assert.That(parameters[0].Name, Is.EqualTo("T"));

			var constraints = parameters[0].Constraints.ToArray();

			Assert.That(constraints.Length, Is.EqualTo(1));

			Assert.That(constraints[0].Name, Is.EqualTo("IDisposable"));
		}

		[Test]
		public async Task Query_Type_PrivateClassWithFieldsAndNestedType()
		{
			var candidateType = await RepositoryTest.FindTypeByName($"{nameof(RepositoryTest)}+{nameof(PrivateClassWithFields)}");
			var expectedType = typeof(PrivateClassWithFields);

			Assert.That(candidateType.Implementation, Is.EqualTo(Implementation.Virtual));
			Assert.That(candidateType.Model, Is.EqualTo(Model.Class));
			Assert.That(candidateType.Name, Is.EqualTo($"{nameof(RepositoryTest)}+{nameof(PrivateClassWithFields)}"));
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

			Assert.That(candidateNestedTypes[0].Name, Is.EqualTo($"{nameof(RepositoryTest)}+{nameof(PrivateClassWithFields)}+{nameof(PrivateClassWithFields.NestedClass)}"));
		}

		[Test]
		public async Task Query_Type_PrivateClassWithInheritance()
		{
			var candidateType = await RepositoryTest.FindTypeByName($"{nameof(RepositoryTest)}+{nameof(InheritFromPrivateClass)}");
			var expectedType = typeof(InheritFromPrivateClass);

			Assert.That(candidateType.Implementation, Is.EqualTo(Implementation.Virtual));
			Assert.That(candidateType.Model, Is.EqualTo(Model.Class));
			Assert.That(candidateType.Name, Is.EqualTo($"{nameof(RepositoryTest)}+{nameof(InheritFromPrivateClass)}"));
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

			Assert.That(candidateType.Base, Is.Not.Null);
			Assert.That(candidateType.Base.Value.Name, Is.EqualTo($"{nameof(RepositoryTest)}+{nameof(PrivateClassWithFields)}"));
		}

		[Test]
		public async Task Query_Type_ProtectedDelegate()
		{
			var candidateType = await RepositoryTest.FindTypeByName($"{nameof(RepositoryTest)}+{nameof(ProtectedDelegate)}");
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
			var candidateType = await RepositoryTest.FindTypeByName($"{nameof(RepositoryTest)}+{nameof(PublicClassWithMethods)}");
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
			var candidateType = await RepositoryTest.FindTypeByName($"{nameof(RepositoryTest)}+{nameof(InternalStructure)}");
			var expectedType = typeof(InternalStructure);

			Assert.That(candidateType.Implementation, Is.EqualTo(Implementation.Final));
			Assert.That(candidateType.Model, Is.EqualTo(Model.Structure));
			Assert.That(candidateType.Parent.Name, Is.EqualTo(expectedType.Assembly.GetName().Name));
			Assert.That(candidateType.Namespace, Is.EqualTo(expectedType.Namespace));
			Assert.That(candidateType.Visibility, Is.EqualTo(Visibility.Internal));
		}

		private static async Task<T> CreateAndQuery<T>(string expression)
		{
			var repository = new Repository(new[] { typeof(RepositoryTest).Assembly.Location });
			var untyped = await repository.Query(expression);

			if (untyped is T typed)
				return typed;

			throw new InvalidOperationException("invalid return type");
		}

		private static async Task<Method> FindMethodByName(string name)
		{
			var methods = await RepositoryTest.CreateAndQuery<Method[]>($"project => project.Assemblies.SelectMany(a => a.Types).SelectMany(t => t.Methods).Where(m => m.Name == \"{name}\").ToArray()");

			Assert.That(methods.Length, Is.EqualTo(1), $"exactly one method must match name {name}");

			return methods[0];
		}

		private static async Task<Reflection.Type> FindTypeByName(string name)
		{
			var types = await RepositoryTest.CreateAndQuery<Reflection.Type[]>($"project => project.Assemblies.SelectMany(a => a.Types).Where(t => t.Name == \"{name}\").ToArray()");

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