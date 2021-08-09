using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace eCommerceAutomation.Service.Utils
{
    public static class ReflectiveEnumerator
    {
        static ReflectiveEnumerator() { }

        public static IEnumerable<T> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class
        {
            List<T> objects = new List<T>();
            var allTypes = Assembly.GetAssembly(typeof(T)).GetTypes();
            var classTypes = allTypes.Where(x => x.IsClass).ToList();
            var notAbstractTypes = classTypes.Where(x => !x.IsAbstract).ToList();
            var tTypes = notAbstractTypes.Where(x => typeof(T).IsAssignableFrom(x)).ToList();
            foreach (Type type in Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(x => x.IsClass)
                .Where(x => !x.IsAbstract)
                .Where(x => typeof(T).IsAssignableFrom(x)).ToList())
            {
                objects.Add((T)Activator.CreateInstance(type, constructorArgs));
            }

            return objects;
        }
    }
}
