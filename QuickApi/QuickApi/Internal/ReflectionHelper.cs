using System;
using System.Linq;
using System.Reflection;

namespace QuickApi.Internal
{
    internal static class ReflectionHelper
    {
        public static bool MethodHasAttribute(MethodInfo method, Type attributeType)
        {
            return method.GetCustomAttributes(attributeType, true).Length > 0;
        }
        public static MethodInfo[] GetAllMethods(Type type)
        {
            return type.GetMethods();
        }
        public static MethodInfo[] GetMethodsByAttribute(Type type, Type attributeType)
        {
            return type.GetMethods().Where(m => MethodHasAttribute(m, attributeType)).ToArray();
        }
        public static bool TypeMethodHasAttribute(Type type, Type attributeType)
        {
            return type.GetMethods().Any(m => MethodHasAttribute(m, attributeType));
        }
        
        public static bool TypeHasAttribute(Type type, Type attributeType)
        {
            return type.GetCustomAttributes(attributeType, true).Length > 0;
        }
   
    }
}