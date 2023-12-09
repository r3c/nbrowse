using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using NBrowse.Reflection;

namespace NBrowse.CLI;

internal static class Help
{
    private const BindingFlags Bindings = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public;

    public static void Write(TextWriter writer)
    {
        writer.WriteLine();
        writer.WriteLine("Entities available in queries are:");
        writer.WriteLine();

        var entities = new Queue<Type>();

        entities.Enqueue(typeof(NProject));

        for (var uniques = new HashSet<Type>(entities); entities.Count > 0;)
        {
            var entity = entities.Dequeue();

            writer.WriteLine($"  {entity.Name}");

            if (entity.IsEnum)
                writer.WriteLine($"     {string.Join(" | ", Enum.GetNames(entity))}");
            else if (entity.IsClass)
            {
                foreach (var property in entity.GetProperties(Bindings))
                {
                    var targetType = GetFirstNonGenericType(property.PropertyType);

                    if (targetType.Namespace == entity.Namespace && uniques.Add(targetType))
                        entities.Enqueue(targetType);

                    writer.WriteLine($"    {FormatProperty(property)}");
                }

                foreach (var method in entity.GetMethods(Bindings))
                {
                    if (method.IsSpecialName)
                        continue;

                    writer.WriteLine($"    {FormatMethod(method)}");
                }
            }
        }
    }

    private static string FormatArgument(ParameterInfo parameterInfo)
    {
        return $"{FormatType(parameterInfo.ParameterType)} {parameterInfo.Name}";
    }

    private static string FormatDescription(DescriptionAttribute descriptionAttribute)
    {
        return descriptionAttribute != null ? " // " + descriptionAttribute.Description : string.Empty;
    }

    private static string FormatMember(MemberInfo memberInfo)
    {
        return $".{memberInfo.Name}";
    }

    private static string FormatMethod(MethodInfo methodInfo)
    {
        var description = methodInfo.GetCustomAttribute<DescriptionAttribute>();
        var parameters = string.Join(", ", methodInfo.GetParameters().Select(FormatArgument));
        var returnType = methodInfo.ReturnType;

        return $"{FormatMember(methodInfo)}({parameters}): {FormatType(returnType)}{FormatDescription(description)}";
    }

    private static string FormatProperty(PropertyInfo propertyInfo)
    {
        var propertyDescription = propertyInfo.GetCustomAttribute<DescriptionAttribute>();
        var propertyType = propertyInfo.PropertyType;

        return $"{FormatMember(propertyInfo)}: {FormatType(propertyType)}{FormatDescription(propertyDescription)}";
    }

    private static string FormatType(Type type)
    {
        if (!type.IsGenericType)
            return type.Name;

        var argumentNames = string.Join(", ", type.GetGenericArguments().Select(FormatType));
        var typeName = Regex.Replace(type.Name, "`[0-9]+$", string.Empty);

        return $"{typeName}<{argumentNames}>";
    }

    private static Type GetFirstNonGenericType(Type type)
    {
        while (type.IsGenericType)
            type = type.GetGenericArguments()[0];

        return type;
    }
}