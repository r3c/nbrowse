using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NBrowse.Reflection;

namespace NBrowse.CLI
{
    internal static class Help
    {
        public static void Write(TextWriter writer)
        {
            writer.WriteLine();
            writer.WriteLine("Entities available in queries are:");
            writer.WriteLine();

            var entities = new Queue<System.Type>();

            entities.Enqueue(typeof(Project));

            var uniques = new HashSet<System.Type>(entities);

            while (entities.Count > 0)
            {
                var entity = entities.Dequeue();

                writer.WriteLine($"  {entity.Name}");

                if (entity.IsEnum)
                    writer.WriteLine($"     {string.Join(" | ", Enum.GetNames(entity))}");
                else if (entity.IsClass)
                {
                    foreach (var property in entity.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {
                        var propertyDescription = property.GetCustomAttribute<DescriptionAttribute>();
                        var propertyType = property.PropertyType;
                        var targetType = GetFirstNonGenericType(property.PropertyType);

                        writer.WriteLine(
                            $"    .{property.Name}: {GetTypeName(propertyType)}{(propertyDescription != null ? " // " + propertyDescription.Description : string.Empty)}");

                        if (targetType.Namespace == entity.Namespace && uniques.Add(targetType))
                            entities.Enqueue(targetType);
                    }

                    foreach (var method in entity.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public))
                    {
                        if (method.IsSpecialName)
                            continue;

                        var methodDescription = method.GetCustomAttribute<DescriptionAttribute>();
                        var methodParameters = method.GetParameters()
                            .Select(p => $"{GetTypeName(p.ParameterType)} {p.Name}");

                        writer.WriteLine(
                            $"    .{method.Name}({string.Join(", ", methodParameters)}): {GetTypeName(method.ReturnType)}{(methodDescription != null ? " // " + methodDescription.Description : string.Empty)}");
                    }
                }
            }
        }

        private static System.Type GetFirstNonGenericType(System.Type type)
        {
            while (type.IsGenericType)
                type = type.GetGenericArguments()[0];

            return type;
        }

        private static string GetTypeName(System.Type type)
        {
            if (!type.IsGenericType)
                return type.Name;

            var typeName = Regex.Replace(type.Name, "`[0-9]+$", string.Empty);
            var argumentNames = string.Join(", ", type.GetGenericArguments().Select(GetTypeName));

            return $"{typeName}<{argumentNames}>";
        }
    }
}