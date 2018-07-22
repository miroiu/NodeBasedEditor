using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VEdit.Common
{
    public static class Extensions
    {
        #region Int

        public static Guid ToGuid(this int value)
        {
            byte[] bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }

        #endregion

        #region String

        public static Guid ToGuid(this string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.Default.GetBytes(input));
                return new Guid(hash);
            }
        }

        public static string Unique(this string value, IEnumerable<string> collection)
        {
            HashSet<string> set = new HashSet<string>(collection);
            string result = value;
            int count = 0;

            while (!set.Add(result))
            {
                result = $"{value} {++count}";
            }

            return result;
        }

        public static bool ContainsIgnoreCase(this string value, string toCheck)
        {
            return value != null && toCheck != null && value.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static string Beautify(this string value)
        {
            if(value.StartsWith("_"))
            {
                return string.Empty;
            }

            var beauty = Regex.Replace(value, @"(\B[A-Z0-9])", " $1");
            return new CultureInfo("en-US", false).TextInfo.ToTitleCase(beauty);
        }

        private static Dictionary<string, FileType> _extensions = new Dictionary<string, FileType>();

        public static FileType GetFileType(this string value)
        {
            if (_extensions.Count == 0)
            {
                var type = typeof(FileType);
                var names = Enum.GetNames(type);
                var members = type.GetMembers().Where(m => m.Name.In(names));
                foreach (var member in members)
                {
                    var memberToValue = (FileType)Enum.Parse(type, member.Name);
                    var attributes = member.GetCustomAttributes(typeof(ExtensionAttribute), false).Cast<ExtensionAttribute>();

                    foreach (var attribute in attributes)
                    {
                        foreach (var extension in attribute.Extensions)
                        {
                            if (!_extensions.ContainsKey(extension))
                            {
                                _extensions.Add(extension, memberToValue);
                            }
                        }
                    }
                }
            }

            string ext = Path.GetExtension(value).Replace(".", string.Empty);
            if (_extensions.TryGetValue(ext, out FileType file))
            {
                return file;
            }
            return FileType.Unknown;
        }

        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            var type = value.GetType();
            var memInfo = type.GetMember(value.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0) ? (T)attributes[0] : null;
        }

        #endregion

        #region Task

        public static readonly Task CompletedTask = Task.FromResult(false);

        #endregion

        #region Type

        public static string GetFullName(this MethodInfo method)
        {
            return $"{method.DeclaringType.AssemblyQualifiedName}.{method.Name}";
        }

        public static string GetFullNameWithParameters(this MethodInfo method)
        {
            var signature = new StringBuilder();
            if (method.ReturnType != typeof(void))
            {
                signature.Append(method.ReturnType.ToString());
                signature.Append(" ");
            }

            signature.Append(method.Name);
            var @params = new List<ParameterInfo>(method.GetParameters());

            if (@params.Count > 0)
            {
                signature = signature.Append('(');
                for (int i = 0; i < @params.Count; i++)
                {
                    AddParameter(signature, @params[i]);
                    if (i != @params.Count - 1)
                    {
                        signature = signature.Append(", ");
                    }
                }
                signature = signature.Append(')');
            }
            return signature.ToString();

            void AddParameter(StringBuilder sig2, ParameterInfo parameter)
            {
                sig2 = sig2.Append(parameter.ParameterType.ToString());
                if (!string.IsNullOrEmpty(parameter.Name))
                {
                    sig2 = sig2.Append(" ");
                    sig2 = sig2.Append(parameter.Name);
                }
                if (parameter.IsOptional)
                {
                    sig2 = sig2.Append(" = ");
                    if (parameter.DefaultValue is string)
                    {
                        sig2 = sig2.AppendFormat("\"{0}\"", parameter.DefaultValue);
                    }
                    else
                    {
                        sig2 = sig2.Append(parameter.DefaultValue);
                    }
                }
            }
        }

        public static object GetDefaultValue(this Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }

        public static IEnumerable<MethodInfo> GetMarkedMethods<T>(this Type type) where T : Attribute
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
            return methods.Where(m => m.IsDefined(typeof(T)) && !m.IsGenericMethod);
        }

        public static IEnumerable<Type> GetMarkedTypes<T>(this Assembly assembly) where T : Attribute
        {
            var staticTypes = assembly.GetExportedTypes().Where(t => t.IsAbstract && t.IsSealed);
            return staticTypes.Where(t => t.IsDefined(typeof(T)));
        }

        public static bool HasCastDefined(this Type from, Type to)
        {
            return to.IsAssignableFrom(from) || from.HasCastDefined(to, true);
        }

        private static bool HasCastDefined(this Type from, Type to, bool implicitly)
        {
            if ((from.IsPrimitive || from.IsEnum) && (to.IsPrimitive || to.IsEnum))
            {
                if (!implicitly)
                {
                    return from == to || (from != typeof(Boolean) && to != typeof(Boolean));
                }

                Type[][] typeHierarchy = {
                    new Type[] { typeof(Byte),  typeof(SByte), typeof(Char) },
                    new Type[] { typeof(Int16), typeof(UInt16) },
                    new Type[] { typeof(Int32), typeof(UInt32) },
                    new Type[] { typeof(Int64), typeof(UInt64) },
                    new Type[] { typeof(Single) },
                    new Type[] { typeof(Double) }
                };

                IEnumerable<Type> lowerTypes = Enumerable.Empty<Type>();
                foreach (Type[] types in typeHierarchy)
                {
                    if (types.Any(t => t == to))
                    {
                        return lowerTypes.Any(t => t == from);
                    }
                    lowerTypes = lowerTypes.Concat(types);
                }

                return false;   // IntPtr, UIntPtr, Enum, Boolean
            }
            return IsCastDefined(to, m => m.GetParameters()[0].ParameterType, _ => from, implicitly, false)
                || IsCastDefined(from, _ => to, m => m.ReturnType, implicitly, true);
        }

        private static bool IsCastDefined(Type type, Func<MethodInfo, Type> baseType, Func<MethodInfo, Type> derivedType, bool implicitly, bool lookInBase)
        {
            var bindinFlags = BindingFlags.Public | BindingFlags.Static | (lookInBase ? BindingFlags.FlattenHierarchy : BindingFlags.DeclaredOnly);
            return type.GetMethods(bindinFlags).Any(m =>
                (m.Name == "op_Implicit" || (!implicitly && m.Name == "op_Explicit"))
                && baseType(m).IsAssignableFrom(derivedType(m)));
        }

        #endregion

        #region Generic

        public static void Run<T>(this IEnumerable<T> e)
        {
            foreach (var _ in e) ;
        }

        public static void AddRange<T>(this IList<T> collection, IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                collection.Add(value);
            }
        }

        public static bool In<T>(this T value, IEnumerable<T> collection) => collection.Contains(value);

        public static T[] ToOneItemArray<T>(this T item) => new[] { item };

        #endregion
    }
}
