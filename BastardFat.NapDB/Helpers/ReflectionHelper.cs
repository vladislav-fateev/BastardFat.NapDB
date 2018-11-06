using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BastardFat.NapDB.Helpers
{
    internal enum RestrictionsKind
    {
        None,
        All,
        Any
    }

    internal static class ReflectionHelper
    {
        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(this Expression<Func<TSource, TProperty>> propertyLambda)
        {
            return GetPropertyInfo(propertyLambda, RestrictionsKind.None);
        }
        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(this Expression<Func<TSource, TProperty>> propertyLambda, RestrictionsKind restrictionsKind, params Type[] typeRestrictions)
        {
            Type type = typeof(TSource);

            if (!(propertyLambda.Body is MemberExpression member))
                throw new ArgumentException(string.Format(
                    "Expression '{0}' doesn't refers to a property",
                    propertyLambda.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            if (restrictionsKind == RestrictionsKind.All)
            {
                var failedRestrict = typeRestrictions.FirstOrDefault(t => !t.IsAssignableFrom(propInfo.PropertyType));
                if (failedRestrict != null)
                    throw new ArgumentException(string.Format(
                        "Expression '{0}' refers to a property that is not assignable to type {1}.",
                        propertyLambda.ToString(),
                        failedRestrict));
            }

            if (restrictionsKind == RestrictionsKind.Any)
            {
                if (typeRestrictions.All(t => !t.IsAssignableFrom(propInfo.PropertyType)))
                    throw new ArgumentException(string.Format(
                        "Expression '{0}' refers to a property that is not assignable to any of allowed types: {1}.",
                        propertyLambda.ToString(),
                        String.Join(", ", (object[])typeRestrictions)));
            }
            

            return propInfo;
        }

        public static bool IsPropertyConfigurable(PropertyInfo property)
        {
            if (!property.CanRead || !property.CanWrite)
                return false;
            if (property.PropertyType.IsAbstract)
                return property.PropertyType.IsInterface && property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(IEnumerable<>);
            return !property.IsSpecialName && property.GetMethod.IsPublic && property.SetMethod.IsPublic;
        }

        public static bool IsPropertyProxible(PropertyInfo property)
        {
            return IsPropertyConfigurable(property) && property.GetMethod.IsVirtual && property.SetMethod.IsVirtual;
        }

        public static bool IsTypeIEnumerable(Type type, Type elementType)
        {
            return typeof(IEnumerable<>).MakeGenericType(elementType) == type;
        }

        public static bool IsPropertyGenericInterfaceDefinition(PropertyInfo property, Type ifaceGenericDefinition)
        {
            return property.PropertyType.IsInterface &&
                property.PropertyType.IsGenericType &&
                property.PropertyType.GetGenericTypeDefinition() == ifaceGenericDefinition;
        }
    }
}
