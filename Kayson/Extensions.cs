using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using Kayson.Attributes;

namespace Kayson
{
    /// <summary>
    /// Provides helper object extensions for the API.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Ensures that a user is permitted to access a resource.
        /// </summary>
        /// <param name="context">The HttpContext of the user accessing the resource.</param>
        /// <param name="type">The type to search for permission attributes on.</param>
        /// <param name="failedOn">The IPermissionAttribute that rejected the request upon failure.</param>
        /// <returns>True if the user can access the resource, false otherwise.</returns>
        public static bool EnsurePermitted(this HttpContext context, Type type, out IPermissionAttribute failedOn)
        {
            failedOn = null;
            object[] attrs = type.GetCustomAttributes(typeof(IPermissionAttribute), true);
            bool permitted = attrs.Length == 0;

            if (!permitted)
            {
                Dictionary<Type, List<IPermissionAttribute>> bag = new Dictionary<Type, List<IPermissionAttribute>>();

                // Group by type.
                foreach (IPermissionAttribute attr in attrs)
                {
                    Type attrType = attr.GetType();

                    if (!bag.ContainsKey(attrType))
                    {
                        bag.Add(attrType, new List<IPermissionAttribute>());
                    }

                    bag[attrType].Add(attr);
                }

                // Evaluate each type group.
                foreach (KeyValuePair<Type, List<IPermissionAttribute>> pair in bag)
                {
                    // Get the group join and start the group bool expression.
                    PermissionJoinType join = pair.Value[0].Join;
                    permitted = (join == PermissionJoinType.And) ? true : false;

                    // Evaluate the group.
                    foreach (IPermissionAttribute attr in pair.Value)
                    {
                        permitted = join == PermissionJoinType.And ?
                            permitted && attr.EnsurePermitted(context) :
                            permitted || attr.EnsurePermitted(context);

                        // Save the attribute we failed on.
                        if (!permitted)
                        {
                            failedOn = attr;

                            // Fail out?
                            if (join == PermissionJoinType.And)
                            {
                                break;
                            }
                        }
                    }

                    // Keep going?
                    if (!permitted)
                    {
                        break;
                    }
                }
            }

            return permitted;
        }

        /// <summary>
        /// Converts a base-64 encoded string to standard encoding.
        /// </summary>
        /// <param name="str">The string to decode.</param>
        /// <returns>The decoded string.</returns>
        public static string FromBase64(this string str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                Decoder decoder = new UTF8Encoding().GetDecoder();

                byte[] buffer = Convert.FromBase64String(str);
                char[] chars = new char[decoder.GetCharCount(buffer, 0, buffer.Length)];
                decoder.GetChars(buffer, 0, buffer.Length, chars, 0);

                str = new String(chars);
            }

            return str ?? String.Empty;
        }

        /// <summary>
        /// De-serializes an object from JSON.
        /// </summary>
        /// <typeparam name="T">The specific type to run the serialization against.</typeparam>
        /// <param name="str">The JSON string to de-serialize.</param>
        /// <returns>A de-serialized object.</returns>
        public static T FromJson<T>(this string str)
        {
            return str.FromJson<T>(new List<Type>());
        }

        /// <summary>
        /// De-serializes an object from JSON.
        /// </summary>
        /// <typeparam name="T">The specific type to run the serialization against.</typeparam>
        /// <param name="str">The JSON string to de-serialize.</param>
        /// <param name="knownTypes">The known types in the object graph.</param>
        /// <returns>A de-serialized object.</returns>
        public static T FromJson<T>(this string str, IEnumerable<Type> knownTypes)
        {
            return (T)FromJson(typeof(T), str, knownTypes);
        }

        /// <summary>
        /// De-serializes an object from JSON.
        /// </summary>
        /// <param name="type">The type of object to de-serialize.</param>
        /// <param name="str">The JSON string to de-serialize.</param>
        /// <param name="knownTypes">The known types in the object graph.</param>
        /// <returns>A de-serialized object.</returns>
        public static object FromJson(Type type, string str, IEnumerable<Type> knownTypes)
        {
            try
            {
                if (!String.IsNullOrEmpty(str))
                {
                    using (Stream stream = new MemoryStream())
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            writer.Write(str);
                            writer.Flush();
                            stream.Position = 0;

                            return new DataContractJsonSerializer(type, knownTypes).ReadObject(stream);
                        }
                    }
                }
                else
                {
                    return Activator.CreateInstance(type);
                }
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException(type, ex);
            }
        }

        /// <summary>
        /// Converts a string to it's base-64 equivalent.
        /// </summary>
        /// <param name="str">The string to encode.</param>
        /// <returns>The encoded string.</returns>
        public static string ToBase64(this string str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                byte[] buffer = new byte[str.Length];
                buffer = Encoding.UTF8.GetBytes(str);
                str = Convert.ToBase64String(buffer);
            }

            return str ?? String.Empty;
        }

        /// <summary>
        /// Serializes an object to JSON. Object must be Serializable.
        /// </summary>
        /// <typeparam name="T">The specific type to run the serialization against.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The serialized JSON.</returns>
        public static string ToJson<T>(this T obj)
        {
            return obj.ToJson<T>(new List<Type>());
        }

        /// <summary>
        /// Serializes an object to JSON. Object must be Serializable.
        /// </summary>
        /// <typeparam name="T">The specific type to run the serialization against.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="knownTypes">The known types in the object graph.</param>
        /// <returns>The serialized JSON.</returns>
        public static string ToJson<T>(this T obj, IEnumerable<Type> knownTypes)
        {
            return ToJson(typeof(T), obj, knownTypes);
        }

        /// <summary>
        /// Serializes an object to JSON. Object must be Serializable.
        /// </summary>
        /// <param name="type">The type of object to serialize.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="knownTypes">The known types in the object graph.</param>
        /// <returns>The serialized JSON.</returns>
        public static string ToJson(Type type, object obj, IEnumerable<Type> knownTypes)
        {
            try
            {
                if (obj != null)
                {
                    using (Stream stream = new MemoryStream())
                    {
                        new DataContractJsonSerializer(type, knownTypes).WriteObject(stream, obj);
                        stream.Position = 0;

                        using (StreamReader reader = new StreamReader(stream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
                else
                {
                    return String.Empty;
                }
            }
            catch (Exception ex)
            {
                throw new JsonSerializationException(type, ex);
            }
        }
    }
}
