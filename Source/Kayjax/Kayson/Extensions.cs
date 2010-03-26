

namespace Kayson
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Web;

    /// <summary>
    /// Provides helper object extensions for the API.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Gets a value indicating whether the HTTP request for the given context accepts
        /// GZip-compressed responses.
        /// </summary>
        /// <param name="context">The HTTP context to get GZip acceptance for.</param>
        /// <returns>True if the requesting user agent accepts GZip responses, false otherwise.</returns>
        public static bool AcceptsGZip(this HttpContext context)
        {
            return (context.Request.Headers["accept-encoding"] ?? String.Empty).ToUpperInvariant().Contains("GZIP");
        }

        /// <summary>
        /// Ensures that a user is permitted to access a resource.
        /// </summary>
        /// <param name="context">The HttpContext of the user accessing the resource.</param>
        /// <param name="type">The type to search for permission attributes on.</param>
        /// <param name="failedOn">The IPermissionAttribute that rejected the request upon failure.</param>
        /// <returns>True if the user can access the resource, false otherwise.</returns>
        [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", Justification = "By design.")]
        public static bool EnsurePermitted(this HttpContext context, MemberInfo type, out IPermission failedOn)
        {
            failedOn = null;
            object[] attrs = type.GetCustomAttributes(typeof(IPermission), true);
            bool permitted = attrs.Length == 0;

            if (!permitted)
            {
                Dictionary<Type, List<IPermission>> bag = new Dictionary<Type, List<IPermission>>();

                // Group by type.
                foreach (IPermission attr in attrs)
                {
                    Type attrType = attr.GetType();

                    if (!bag.ContainsKey(attrType))
                    {
                        bag.Add(attrType, new List<IPermission>());
                    }

                    bag[attrType].Add(attr);
                }

                // Evaluate each type group.
                foreach (KeyValuePair<Type, List<IPermission>> pair in bag)
                {
                    // Get the group join and start the group bool expression.
                    PermissionJoinType join = pair.Value[0].Join;
                    permitted = (join == PermissionJoinType.And) ? true : false;

                    // Evaluate the group.
                    foreach (IPermission attr in pair.Value)
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
        /// <param name="value">The string to decode.</param>
        /// <returns>The decoded string.</returns>
        public static string FromBase64(this string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                Decoder decoder = new UTF8Encoding().GetDecoder();

                byte[] buffer = Convert.FromBase64String(value);
                char[] chars = new char[decoder.GetCharCount(buffer, 0, buffer.Length)];
                decoder.GetChars(buffer, 0, buffer.Length, chars, 0);

                value = new String(chars);
            }

            return value ?? String.Empty;
        }

        /// <summary>
        /// De-serializes an object from JSON.
        /// </summary>
        /// <param name="type">The type of object to de-serialize.</param>
        /// <param name="value">The JSON string to de-serialize.</param>
        /// <returns>A de-serialized object.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Spelling is correct.")]
        public static object FromJson(Type type, string value)
        {
            return FromJson(type, value, new List<Type>());
        }

        /// <summary>
        /// De-serializes an object from JSON.
        /// </summary>
        /// <param name="type">The type of object to de-serialize.</param>
        /// <param name="value">The JSON string to de-serialize.</param>
        /// <param name="knownTypes">The known types in the object graph.</param>
        /// <returns>A de-serialized object.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Spelling is correct.")]
        public static object FromJson(Type type, string value, IEnumerable<Type> knownTypes)
        {
            return FromJson(type, value, Encoding.Default, knownTypes);
        }

        /// <summary>
        /// De-serializes an object from JSON.
        /// </summary>
        /// <param name="type">The type of object to de-serialize.</param>
        /// <param name="value">The JSON string to de-serialize.</param>
        /// <param name="encoding">The encoding to use when reading the JSON.</param>
        /// <param name="knownTypes">The known types in the object graph.</param>
        /// <returns>A de-serialized object.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Spelling is correct.")]
        public static object FromJson(Type type, string value, Encoding encoding, IEnumerable<Type> knownTypes)
        {
            try
            {
                if (!String.IsNullOrEmpty(value))
                {
                    using (Stream stream = new MemoryStream())
                    {
                        using (StreamWriter writer = new StreamWriter(stream, encoding))
                        {
                            writer.Write(value);
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
        /// <param name="value">The string to encode.</param>
        /// <returns>The encoded string.</returns>
        public static string ToBase64(this string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                byte[] buffer = new byte[value.Length];
                buffer = Encoding.UTF8.GetBytes(value);
                value = Convert.ToBase64String(buffer);
            }

            return value ?? String.Empty;
        }

        /// <summary>
        /// Serializes an object to JSON. Object must be Serializable.
        /// </summary>
        /// <typeparam name="T">The specific type to run the serialization against.</typeparam>
        /// <param name="value">The object to serialize.</param>
        /// <returns>The serialized JSON.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Spelling is correct.")]
        public static string ToJson<T>(this T value)
        {
            return value.ToJson<T>(new List<Type>());
        }

        /// <summary>
        /// Serializes an object to JSON. Object must be Serializable.
        /// </summary>
        /// <typeparam name="T">The specific type to run the serialization against.</typeparam>
        /// <param name="value">The object to serialize.</param>
        /// <param name="knownTypes">The known types in the object graph.</param>
        /// <returns>The serialized JSON.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Spelling is correct.")]
        public static string ToJson<T>(this T value, IEnumerable<Type> knownTypes)
        {
            return ToJson(typeof(T), value, knownTypes);
        }

        /// <summary>
        /// Serializes an object to JSON. Object must be Serializable.
        /// </summary>
        /// <param name="type">The type of object to serialize.</param>
        /// <param name="value">The object to serialize.</param>
        /// <param name="knownTypes">The known types in the object graph.</param>
        /// <returns>The serialized JSON.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", Justification = "Spelling is correct.")]
        public static string ToJson(Type type, object value, IEnumerable<Type> knownTypes)
        {
            try
            {
                if (value != null)
                {
                    using (Stream stream = new MemoryStream())
                    {
                        new DataContractJsonSerializer(type, knownTypes).WriteObject(stream, value);
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
