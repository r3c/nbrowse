using System;
using System.Collections;
using System.Diagnostics;
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
			Assert.AreEqual(expected, await CreateAndQuery<T>(expression));
		}

		[Test]
		public async Task Query_Has_TypeCustomAttribute()
		{
			Assert.IsTrue(await CreateAndQuery<bool>($"project => Has.Attribute<System.Runtime.CompilerServices.CompilerGeneratedAttribute>(project.FindType(\"{nameof(RepositoryTest)}+{nameof(InternalStructure)}\"))"));
		}

		[Test]
		public async Task Query_Is_TypeGenerated()
		{
			Assert.IsTrue(await CreateAndQuery<bool>($"project => Is.Generated(project.FindType(\"{nameof(RepositoryTest)}+{nameof(InternalStructure)}\"))"));
		}

		[Test]
		public async Task Query_Method_IsUsingMethod()
		{
			var caller = await FindMethodByName("PublicMethodWithCallToOthers");
			var directCallee = await FindMethodByName("ProtectedVirtualDynamicMethod");
			var indirectCall = await FindMethodByName("PrivateStaticMethodCalled");
			var noCall = await FindMethodByName("PrivateStaticMethodNotCalled");

			Assert.IsTrue(caller.IsUsing(directCallee));
			Assert.IsTrue(caller.IsUsing(indirectCall));
			Assert.IsFalse(caller.IsUsing(noCall));
		}

		[Test]
		public async Task Query_Method_GenericDefaultConstructorMethod()
		{
			var method = await FindMethodByName("GenericDefaultConstructorMethod");
			var parameters = method.Parameters.ToArray();

			Assert.AreEqual(1, parameters.Length);

			Assert.AreEqual(true, parameters[0].HasDefaultConstructor);
			Assert.AreEqual(false, parameters[0].IsContravariant);
			Assert.AreEqual(false, parameters[0].IsCovariant);
			Assert.AreEqual("U", parameters[0].Name);

			var constraints = parameters[0].Constraints.ToArray();

			Assert.AreEqual(0, constraints.Length);
		}

		[Test]
		public async Task Query_Method_GenericValueTypeMethod()
		{
			var method = await FindMethodByName("GenericValueTypeMethod");
			var parameters = method.Parameters.ToArray();

			Assert.AreEqual(1, parameters.Length);

			Assert.AreEqual(true, parameters[0].HasDefaultConstructor);
			Assert.AreEqual(false, parameters[0].IsContravariant);
			Assert.AreEqual(false, parameters[0].IsCovariant);
			Assert.AreEqual("U", parameters[0].Name);

			var constraints = parameters[0].Constraints.ToArray();

			Assert.AreEqual(1, constraints.Length);

			Assert.AreEqual("ValueType", constraints[0].Name);
		}

		[Test]
		public async Task Query_Project_FilterAssemblies()
		{
			var assemblies = await CreateAndQuery<Reflection.Assembly[]>($"project => project.FilterAssemblies(new [] {{\"Missing1\", \"{typeof(RepositoryTest).Assembly.GetName().Name}\", \"Missing2\"}}).ToArray()");

			Assert.AreEqual(1, assemblies.Length);
			Assert.AreEqual(typeof(RepositoryTest).Assembly.GetName().Name, assemblies[0].Name);
		}

		[Test]
		public async Task Query_Project_FindExistingAssembly()
		{
			var assembly = await CreateAndQuery<Reflection.Assembly>($"project => project.FindAssembly(\"{typeof(RepositoryTest).Assembly.GetName().Name}\")");

			StringAssert.EndsWith("NBrowse.Test.dll", assembly.FileName);
		}

		[Test]
		public void Query_Project_FindMissingAssembly()
		{
			Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await CreateAndQuery<Reflection.Assembly>("project => project.FindAssembly(\"DoesNotExist\")"));
		}

		[Test]
		public async Task Query_Project_FindExistingTypeByIdentifier()
		{
			var type = await CreateAndQuery<Reflection.Type>($"project => project.FindType(\"{typeof(RepositoryTest).FullName}\")");

			Assert.AreEqual(typeof(RepositoryTest).FullName, type.Identifier);
		}

		[Test]
		public async Task Query_Project_FindExistingTypeByName()
		{
			var type = await CreateAndQuery<Reflection.Type>("project => project.FindType(\"RepositoryTest\")");

			Assert.AreEqual(typeof(RepositoryTest).FullName, type.Identifier);
		}

		[Test]
		public void Query_Project_FindMissingType()
		{
			Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await CreateAndQuery<Reflection.Type>("project => project.FindType(\"DoesNotExist\")"));
		}

		[Test]
		public async Task Query_Type_ClassWithInterfaces()
		{
			var names = await CreateAndQuery<string[]>($"project => project.FindType(\"{nameof(RepositoryTest)}+{nameof(ClassWithInterfaces)}\").Interfaces.Select(i => i.Name).ToArray()");

			CollectionAssert.AreEquivalent(new[] { "ICloneable", "IDisposable" }, names);
		}

		[Test]
		public async Task Query_Type_InterfaceWithGenericParameter()
		{
			var candidateType = await FindTypeByName($"{nameof(RepositoryTest)}+{nameof(InterfaceWithGenericParameter<Stream>)}`1");

			var parameters = candidateType.Parameters.ToArray();

			Assert.AreEqual(1, parameters.Length);

			Assert.AreEqual(false, parameters[0].HasDefaultConstructor);
			Assert.AreEqual(true, parameters[0].IsContravariant);
			Assert.AreEqual(false, parameters[0].IsCovariant);
			Assert.AreEqual("T", parameters[0].Name);

			var constraints = parameters[0].Constraints.ToArray();

			Assert.AreEqual(1, constraints.Length);

			Assert.AreEqual("IDisposable", constraints[0].Name);
		}

		[Test]
		public async Task Query_Type_PrivateClassWithFieldsAndNestedType()
		{
			var candidateType = await FindTypeByName($"{nameof(RepositoryTest)}+{nameof(PrivateClassWithFields)}");
			var expectedType = typeof(PrivateClassWithFields);

			Assert.AreEqual(Implementation.Virtual, candidateType.Implementation);
			Assert.AreEqual(Model.Class, candidateType.Model);
			Assert.AreEqual($"{nameof(RepositoryTest)}+{nameof(PrivateClassWithFields)}", candidateType.Name);
			Assert.AreEqual(expectedType.Assembly.GetName().Name, candidateType.Parent.Name);
			Assert.AreEqual(expectedType.Namespace, candidateType.Namespace);
			Assert.AreEqual(Visibility.Private, candidateType.Visibility);

			var candidateFields = candidateType.Fields.ToArray();

			Assert.AreEqual(4, candidateFields.Length);

			Assert.AreEqual(Binding.Instance, candidateFields[0].Binding);
			Assert.AreEqual("A", candidateFields[0].Name);
			Assert.AreEqual("String", candidateFields[0].Type.Name);
			Assert.AreEqual(Visibility.Public, candidateFields[0].Visibility);
			Assert.AreEqual(Binding.Instance, candidateFields[0].Binding);

			Assert.AreEqual("B", candidateFields[1].Name);
			Assert.AreEqual("Int32", candidateFields[1].Type.Name);
			Assert.AreEqual(Visibility.Protected, candidateFields[1].Visibility);
			Assert.AreEqual(Binding.Instance, candidateFields[1].Binding);

			Assert.AreEqual("C", candidateFields[2].Name);
			Assert.AreEqual("Single", candidateFields[2].Type.Name);
			Assert.AreEqual(Visibility.Private, candidateFields[2].Visibility);
			Assert.AreEqual(Binding.Static, candidateFields[2].Binding);

			Assert.AreEqual("D", candidateFields[3].Name);
			Assert.AreEqual("Int64", candidateFields[3].Type.Name);
			Assert.AreEqual(Visibility.Internal, candidateFields[3].Visibility);

			var candidateNestedTypes = candidateType.NestedTypes.ToArray();

			Assert.AreEqual(1, candidateNestedTypes.Length);

			Assert.AreEqual($"{nameof(RepositoryTest)}+{nameof(PrivateClassWithFields)}+{nameof(PrivateClassWithFields.NestedClass)}", candidateNestedTypes[0].Name);
		}

		[Test]
		public async Task Query_Type_PrivateClassWithInheritance()
		{
			var candidateType = await FindTypeByName($"{nameof(RepositoryTest)}+{nameof(InheritFromPrivateClass)}");
			var expectedType = typeof(InheritFromPrivateClass);

			Assert.AreEqual(Implementation.Virtual, candidateType.Implementation);
			Assert.AreEqual(Model.Class, candidateType.Model);
			Assert.AreEqual($"{nameof(RepositoryTest)}+{nameof(InheritFromPrivateClass)}", candidateType.Name);
			Assert.AreEqual(expectedType.Assembly.GetName().Name, candidateType.Parent.Name);
			Assert.AreEqual(expectedType.Namespace, candidateType.Namespace);
			Assert.AreEqual(Visibility.Private, candidateType.Visibility);

			var candidateFields = candidateType.Fields.ToArray();

			Assert.AreEqual(1, candidateFields.Count());

			Assert.AreEqual(Binding.Instance, candidateFields[0].Binding);
			Assert.AreEqual("E", candidateFields[0].Name);
			Assert.AreEqual("Byte", candidateFields[0].Type.Name);
			Assert.AreEqual(Visibility.Public, candidateFields[0].Visibility);
			Assert.AreEqual(Binding.Instance, candidateFields[0].Binding);

			Assert.IsNotNull(candidateType.Base);
			Assert.AreEqual($"{nameof(RepositoryTest)}+{nameof(PrivateClassWithFields)}", candidateType.Base.Value.Name);
		}

		[Test]
		public async Task Query_Type_ProtectedDelegate()
		{
			var candidateType = await FindTypeByName($"{nameof(RepositoryTest)}+{nameof(ProtectedDelegate)}");
			var expectedType = typeof(ProtectedDelegate);

			Assert.AreEqual(Implementation.Final, candidateType.Implementation);
			Assert.AreEqual(Model.Class, candidateType.Model);
			Assert.AreEqual(expectedType.Assembly.GetName().Name, candidateType.Parent.Name);
			Assert.AreEqual(expectedType.Namespace, candidateType.Namespace);
			Assert.AreEqual(Visibility.Protected, candidateType.Visibility);
		}

		[Test]
		public async Task Query_Type_PublicClassWithMethods()
		{
			var candidateType = await FindTypeByName($"{nameof(RepositoryTest)}+{nameof(PublicClassWithMethods)}");
			var expectedType = typeof(PublicClassWithMethods);

			Assert.AreEqual(Implementation.Abstract, candidateType.Implementation);
			Assert.AreEqual(Model.Class, candidateType.Model);
			Assert.AreEqual(expectedType.Assembly.GetName().Name, candidateType.Parent.Name);
			Assert.AreEqual(expectedType.Namespace, candidateType.Namespace);
			Assert.AreEqual(Visibility.Public, candidateType.Visibility);

			var candidateMethods = candidateType.Methods.ToArray();

			Assert.AreEqual(8, candidateMethods.Length);

			Assert.AreEqual(Binding.Constructor, candidateMethods[0].Binding);
			Assert.AreEqual(Implementation.Concrete, candidateMethods[0].Implementation);
			Assert.AreEqual(".ctor", candidateMethods[0].Name);
			Assert.AreEqual("Void", candidateMethods[0].ReturnType.Name);
			Assert.AreEqual(Visibility.Public, candidateMethods[0].Visibility);

			var candidateMethodArguments = candidateMethods[0].Arguments.ToArray();

			Assert.AreEqual(1, candidateMethodArguments.Length);
			Assert.AreEqual("index", candidateMethodArguments[0].Name);
			Assert.AreEqual("Int32", candidateMethodArguments[0].Type.Name);

			Assert.AreEqual(Binding.Instance, candidateMethods[1].Binding);
			Assert.AreEqual(Implementation.Final, candidateMethods[1].Implementation);
			Assert.AreEqual("GetHashCode", candidateMethods[1].Name);
			Assert.AreEqual("Int32", candidateMethods[1].ReturnType.Name);
			Assert.AreEqual(Visibility.Public, candidateMethods[1].Visibility);

			Assert.AreEqual(Binding.Instance, candidateMethods[2].Binding);
			Assert.AreEqual(Implementation.Virtual, candidateMethods[2].Implementation);
			Assert.AreEqual("ToString", candidateMethods[2].Name);
			Assert.AreEqual("String", candidateMethods[2].ReturnType.Name);
			Assert.AreEqual(Visibility.Public, candidateMethods[2].Visibility);

			Assert.AreEqual(Binding.Instance, candidateMethods[3].Binding);
			Assert.AreEqual(Implementation.Concrete, candidateMethods[3].Implementation);
			Assert.AreEqual("PublicMethodWithCallToOthers", candidateMethods[3].Name);
			Assert.AreEqual("Void", candidateMethods[3].ReturnType.Name);
			Assert.AreEqual(Visibility.Public, candidateMethods[3].Visibility);

			Assert.AreEqual(Binding.Instance, candidateMethods[4].Binding);
			Assert.AreEqual(Implementation.Virtual, candidateMethods[4].Implementation);
			Assert.AreEqual("ProtectedVirtualDynamicMethod", candidateMethods[4].Name);
			Assert.AreEqual("TimeSpan", candidateMethods[4].ReturnType.Name);
			Assert.AreEqual(Visibility.Protected, candidateMethods[4].Visibility);

			Assert.AreEqual(Binding.Static, candidateMethods[5].Binding);
			Assert.AreEqual(Implementation.Concrete, candidateMethods[5].Implementation);
			Assert.AreEqual("PrivateStaticMethodCalled", candidateMethods[5].Name);
			Assert.AreEqual("DateTime", candidateMethods[5].ReturnType.Name);
			Assert.AreEqual(Visibility.Private, candidateMethods[5].Visibility);

			Assert.AreEqual(Binding.Static, candidateMethods[6].Binding);
			Assert.AreEqual(Implementation.Concrete, candidateMethods[6].Implementation);
			Assert.AreEqual("PrivateStaticMethodNotCalled", candidateMethods[6].Name);
			Assert.AreEqual("Uri", candidateMethods[6].ReturnType.Name);
			Assert.AreEqual(Visibility.Private, candidateMethods[6].Visibility);

			Assert.AreEqual(Binding.Instance, candidateMethods[7].Binding);
			Assert.AreEqual(Implementation.Abstract, candidateMethods[7].Implementation);
			Assert.AreEqual("InternalAbstractMethod", candidateMethods[7].Name);
			Assert.AreEqual("Guid", candidateMethods[7].ReturnType.Name);
			Assert.AreEqual(Visibility.Internal, candidateMethods[7].Visibility);
		}

		[Test]
		public async Task Query_Type_InternalStructure()
		{
			var candidateType = await FindTypeByName($"{nameof(RepositoryTest)}+{nameof(InternalStructure)}");
			var expectedType = typeof(InternalStructure);

			Assert.AreEqual(Implementation.Final, candidateType.Implementation);
			Assert.AreEqual(Model.Structure, candidateType.Model);
			Assert.AreEqual(expectedType.Assembly.GetName().Name, candidateType.Parent.Name);
			Assert.AreEqual(expectedType.Namespace, candidateType.Namespace);
			Assert.AreEqual(Visibility.Internal, candidateType.Visibility);
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
			var methods = await CreateAndQuery<Method[]>($"project => project.Assemblies.SelectMany(a => a.Types).SelectMany(t => t.Methods).Where(m => m.Name == \"{name}\").ToArray()");

			Assert.AreEqual(1, methods.Length, $"exactly one method must match name {name}");

			return methods[0];
		}

		private static async Task<Reflection.Type> FindTypeByName(string name)
		{
			var types = await CreateAndQuery<Reflection.Type[]>($"project => project.Assemblies.SelectMany(a => a.Types).Where(t => t.Name == \"{name}\").ToArray()");

			Assert.AreEqual(1, types.Length, $"exactly one type must match name {name}");
			Assert.AreEqual(name, types[0].Name, $"inconsistent type name");

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

		interface InterfaceWithGenericParameter<in T> where T : IDisposable
		{
			U GenericDefaultConstructorMethod<U>() where U : new();
			U GenericValueTypeMethod<U>() where U : struct;
		}

		protected delegate int ProtectedDelegate();

		private class PrivateClassWithFields
		{
			public class NestedClass
			{
			}

			public string A = "a";
			protected int B = 1;
			private static float C = 2;
			internal long D = (long)C;
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
				if (ProtectedVirtualDynamicMethod() == null)
					throw new Exception();

				Func<DateTime> func = PrivateStaticMethodCalled;

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