using System;
using System.Linq;
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
        public async Task Query_SingleType_CheckClass()
        {
            var candidateType = await FindTypeByName("PrivateClass");
            var expectedType = typeof(PrivateClass);

            Assert.AreEqual(expectedType.Name, candidateType.Name);
            Assert.AreEqual(Model.Class, candidateType.Model);
            Assert.AreEqual(expectedType.Assembly.GetName().Name, candidateType.Parent.Name);
            Assert.AreEqual(expectedType.Namespace, candidateType.Namespace);
            Assert.AreEqual(Visibility.Private, candidateType.Visibility);

            var candidateFields = candidateType.Fields.ToArray();

            Assert.AreEqual(3, candidateFields.Length);
            Assert.AreEqual("A", candidateFields[0].Name);
            Assert.AreEqual("String", candidateFields[0].Type.Name);
            Assert.AreEqual("B", candidateFields[1].Name);
            Assert.AreEqual("Int32", candidateFields[1].Type.Name);
            Assert.AreEqual("C", candidateFields[2].Name);
            Assert.AreEqual("Single", candidateFields[2].Type.Name);
        }

        [Test]
        public async Task Query_SingleType_CheckStructure()
        {
            var candidateType = await FindTypeByName("PublicStructure");
            var expectedType = typeof(PublicStructure);

            Assert.AreEqual(expectedType.Name, candidateType.Name);
            Assert.AreEqual(Model.Structure, candidateType.Model);
            Assert.AreEqual(expectedType.Assembly.GetName().Name, candidateType.Parent.Name);
            Assert.AreEqual(expectedType.Namespace, candidateType.Namespace);
            Assert.AreEqual(Visibility.Public, candidateType.Visibility);

            var candidateMethods = candidateType.Methods.ToArray();

            Assert.AreEqual(1, candidateMethods.Length);
            Assert.AreEqual("Main", candidateMethods[0].Name);
            Assert.AreEqual("Void", candidateMethods[0].ReturnType.Name);
            Assert.AreEqual(Visibility.Public, candidateMethods[0].Visibility);

            var candidateMethod0Arguments = candidateMethods[0].Arguments.ToArray();

            Assert.AreEqual(1, candidateMethod0Arguments.Length);
            Assert.AreEqual("index", candidateMethod0Arguments[0].Name);
            Assert.AreEqual("Int32", candidateMethod0Arguments[0].Type.Name);
        }

        private static async Task<T> CreateAndQuery<T>(string expression)
        {
            var repository = new Repository(new [] { typeof(RepositoryTest).Assembly.Location });
            var untyped = await repository.Query(expression);

            if (untyped is T typed)
                return typed;

            throw new InvalidOperationException("invalid return type");
        }

        private static Task<Reflection.Type> FindTypeByName(string name)
        {
            return CreateAndQuery<Reflection.Type>($"project => project.Assemblies.SelectMany(a => a.Types).Where(t => t.Name == \"{name}\").First()");
        }

        private class PrivateClass
        {
            public string A;
            protected int B;
            private float C;
        }

        public struct PublicStructure
        {
            public void Main(int index) {}
        }
    }
}