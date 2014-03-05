#region License
//   Copyright 2010 John Sheehan
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License. 
#endregion

using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

#if NETFX_CORE
using System.Reflection.Emit;
using System.Collections.Generic;
#endif

namespace RestSharp.Extensions
{
	/// <summary>
	/// Reflection extensions
	/// </summary>
	public static class ReflectionExtensions
	{
		/// <summary>
		/// Retrieve an attribute from a member (property)
		/// </summary>
		/// <typeparam name="T">Type of attribute to retrieve</typeparam>
		/// <param name="prop">Member to retrieve attribute from</param>
		/// <returns></returns>
		public static T GetAttribute<T>(this MemberInfo prop) where T : Attribute {
#if NETFX_CORE
            return typeof(MemberInfo).GetTypeInfo().GetCustomAttribute<T>();
#else
            return Attribute.GetCustomAttribute(prop, typeof(T)) as T;
#endif
        }

		/// <summary>
		/// Retrieve an attribute from a type
		/// </summary>
		/// <typeparam name="T">Type of attribute to retrieve</typeparam>
		/// <param name="type">Type to retrieve attribute from</param>
		/// <returns></returns>
		public static T GetAttribute<T>(this Type type) where T : Attribute {
#if NETFX_CORE
            return type.GetTypeInfo().GetCustomAttribute<T>();
#else
            return Attribute.GetCustomAttribute(type, typeof(T)) as T;
#endif
        }


		/// <summary>
		/// Checks a type to see if it derives from a raw generic (e.g. List[[]])
		/// </summary>
		/// <param name="toCheck"></param>
		/// <param name="generic"></param>
		/// <returns></returns>
		public static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic) {
			while (toCheck != typeof(object)) {
				var cur = toCheck.IsGenericType() ? toCheck.GetGenericTypeDefinition() : toCheck;
				if (generic == cur) {
					return true;
				}
				toCheck = toCheck.BaseType();
			}
			return false;
		}

		public static object ChangeType(this object source, Type newType)
		{
#if FRAMEWORK && !PocketPC
			return Convert.ChangeType(source, newType);
#else
			return Convert.ChangeType(source, newType, null);
#endif
		}

		public static object ChangeType(this object source, Type newType, CultureInfo culture)
		{
#if FRAMEWORK || SILVERLIGHT || WINDOWS_PHONE
			return Convert.ChangeType(source, newType, culture);
#else
			return Convert.ChangeType(source, newType, null);
#endif
		}

		/// <summary>
		/// Find a value from a System.Enum by trying several possible variants
		/// of the string value of the enum.
		/// </summary>
		/// <param name="type">Type of enum</param>
		/// <param name="value">Value for which to search</param>
		/// <param name="culture">The culture used to calculate the name variants</param>
		/// <returns></returns>
		public static object FindEnumValue(this Type type, string value, CultureInfo culture)
		{
#if FRAMEWORK && !PocketPC
			var ret = Enum.GetValues( type )
			.Cast<Enum>()
			.FirstOrDefault(v => v.ToString().GetNameVariants(culture).Contains(value, StringComparer.Create(culture, true)));

			if (ret == null)
			{
				var enumValueAsUnderlyingType = Convert.ChangeType(value, Enum.GetUnderlyingType(type), culture);
				if (enumValueAsUnderlyingType != null && Enum.IsDefined(type, enumValueAsUnderlyingType))
				{
					ret = (Enum) Enum.ToObject(type, enumValueAsUnderlyingType);
				}
			}

			return ret;
#else
			return Enum.Parse(type, value, true);
#endif
		}

        public static PropertyInfo GetProperty(this Type type, String propertyName)
        {
            return type.GetTypeInfo().GetDeclaredProperty(propertyName);
        }

        public static MethodInfo GetMethod(this Type type, String methodName)
        {
            return type.GetTypeInfo().GetDeclaredMethod(methodName);
        }

        public static bool IsSubclassOf(this Type type, Type parentType)
        {
            return type.GetTypeInfo().IsSubclassOf(parentType);
        }

        public static bool IsAssignableFrom(this Type type, Type parentType)
        {
            return type.GetTypeInfo().IsAssignableFrom(parentType.GetTypeInfo());
        }

        public static bool IsEnum(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().IsEnum;
#else
            return type.IsEnum;
#endif
        }
        public static bool IsValueType(this Type type)
        {
            
#if NETFX_CORE
            return type.GetTypeInfo().IsValueType;
#else
            return type.IsValueType;
#endif
        }
        public static bool IsPrimitive(this Type type)
        {
            
#if NETFX_CORE
            return type.GetTypeInfo().IsPrimitive;
#else
            return type.IsPrimitive;
#endif
        }
        public static bool IsPublic(this Type type)
        {

#if NETFX_CORE
            return type.GetTypeInfo().IsPublic;
#else
            return type.IsPublic;
#endif
        }

        public static Type GetBaseType(this Type type)
        {
            return type.GetTypeInfo().BaseType;
        }


        public static Type[] GetGenericArguments(this Type type)
        {              
#if NETFX_CORE
            return type.GetTypeInfo().GenericTypeArguments;
#else
            return type.GetGenericArguments();
#endif
        }

        public static object GetPropertyValue(this Object instance, string propertyValue)
        {
            return instance.GetType().GetTypeInfo().GetDeclaredProperty(propertyValue).GetValue(instance);
        }

        public static bool IsGenericType(this Type type)
        {

#if NETFX_CORE
            return type.GetTypeInfo().IsGenericType;
#else
            return type.IsGenericType;
#endif
        }

        public static Type BaseType(this Type type)
        {

#if NETFX_CORE
            return type.GetTypeInfo().BaseType;
#else
            return type.BaseType;
#endif
        }

        public static IEnumerable<PropertyInfo> GetProperties(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().DeclaredProperties;
#else
            return type.GetProperties();
#endif
        }

        public static IEnumerable<Type> GetInterfaces(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().ImplementedInterfaces;
#else
            return type.GetInterfaces();
#endif
        }

	}
}
