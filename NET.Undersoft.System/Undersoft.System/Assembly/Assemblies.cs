using System.Reflection;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;

namespace System
{
    public static class Assemblies
    {
        private static bool resolveHandler = ResolveLoad();

        public static Type GetType(string name, string nameSpace = null)
        {
            Type type = Type.GetType(name);
            if (type != null)
                return type;
            var asms = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var asm in asms)
            {
                
                type = asm.GetType(name);
                if (type != null)
                    if(nameSpace == null || type.Namespace == nameSpace)
                        return type;
            }
            return null;
        }
        public static Type GetType(Type argumentType, object argumentValue, Type attributeType = null, string nameSpace = null)
        {
            var asms = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var asm in asms)
            {
                Type[] types = nameSpace != null ?
                                    asm.GetTypes().Where(n => n.Namespace == nameSpace).ToArray() :
                                        asm.GetTypes();
                if (attributeType != null)
                {
                    foreach (var type in types)
                        if (type.GetCustomAttributesData().Where(a => a.AttributeType == attributeType).Where(s => s.ConstructorArguments.Where(o => o.ArgumentType == argumentType &&
                                                        o.Value.Equals(argumentValue)).Any()).Any())
                            return type;
                }
                else
                    foreach (var type in types)
                        if (type.GetCustomAttributesData().Where(s => s.ConstructorArguments.Where(o => o.ArgumentType == argumentType &&
                                                        o.Value.Equals(argumentValue)).Any()).Any())
                            return type;
            }
            return null;
        }

        public static object GetDefault(this Type type)
        {
            if (type == null || !type.IsValueType || type == typeof(void))
                return null;

            if (type.IsPrimitive || !type.IsNotPublic)
            {
                try
                {
                    return Activator.CreateInstance(type);
                }
                catch (Exception e)
                {
                    throw new ArgumentException(
                        "{" + MethodInfo.GetCurrentMethod() + "} Error:\n\nThe Activator.CreateInstance method could not " +
                        "create a default instance of the supplied value type <" + type +
                        "> (Inner Exception message: \"" + e.Message + "\")", e);
                }
            }
            throw new ArgumentException("{" + MethodInfo.GetCurrentMethod() + "} Error:\n\nThe supplied value type <" + type +
                    "> is not a publicly-visible type, so the default value cannot be retrieved");
        }

        public static Queue<object> New(this Queue<object> queue, Type type, int count)
        {
            for (int i = 0; i < count; i++)
            {
                queue.Enqueue(Activator.CreateInstance(type));
            }
            return queue;
        }

        public static object New(this Type type)
        {
            return Activator.CreateInstance(type);
        }
        public static object New(this Type type, params object[] parameters)
        {
            return Activator.CreateInstance(type, parameters);
        }
        public static object[] New(this Type[] types)
        {
            object[] models = new object[types.Length];
            for (int i = 0; i < types.Length; i++)
                models[i] = Activator.CreateInstance(types[i]);
            return models;
        }

        public static string AssemblyCode
        {
            get
            {
                object[] attributes;
                var entryAssembly = Assembly.GetEntryAssembly();
                if (entryAssembly is null)
                    attributes = Assembly.GetCallingAssembly()
                        .GetCustomAttributes(typeof(GuidAttribute), false);
                else
                    attributes = entryAssembly
                        .GetCustomAttributes(typeof(GuidAttribute), false);
                if (attributes.Length == 0)
                    return String.Empty;
                return ((GuidAttribute)attributes[0]).Value.ToUpper();
            }
        }

        public static bool ResolveLoad()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                String resourceName = "AssemblyLoadingAndReflection." + new AssemblyName(args.Name).Name + ".dll";
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    Byte[] assemblyData = new Byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };

            return true;
        }
    }
}
