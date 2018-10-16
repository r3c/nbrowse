using System;
using System.Collections.Generic;
using System.Linq;
using NBrowse.Model;

namespace NBrowse.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var assemblies = new [] { typeof(Query).Assembly };
            var query = new Query(assemblies);

            var models = assemblies.Select(a => new AssemblyModel(a)).ToArray();

            foreach (string result in query.Execute(models, args[0]).Result)
                System.Console.WriteLine(result);
        }
    }
}