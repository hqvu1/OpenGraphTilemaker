﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// ReSharper disable UnusedMember.Global

namespace Common.Extensions
{
    public static class ObjectExtensions
    {
        public static string ReturnDump(this object subject) {
            var settings =
                new JsonSerializerSettings {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.Indented
                };

            return JsonConvert.SerializeObject(subject, settings);
        }

        public static string ReturnDumpFlat(this object subject) {
            var settings =
                new JsonSerializerSettings {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.None
                };

            return JsonConvert.SerializeObject(subject, settings);
        }

        public static Dictionary<string, dynamic> PropertiesToDictionary(this object subject) =>
            subject.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(subject, null));

        public static PropertyInfo PublicProperty(this object subject, string propertyName) =>
            subject.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);

        public static IEnumerable<PropertyInfo> PublicProperties(this object subject) =>
            subject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);

        public static string JSONSerialize(this object subject) {
            var settings =
                new JsonSerializerSettings {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.None,
                    TypeNameHandling = TypeNameHandling.Arrays
                };

            string serializeObject;

            try {
                serializeObject = JsonConvert.SerializeObject(subject, settings);
            }
            catch (Exception e) {
                throw new InvalidOperationException(
                    $"JSON serialization failed for object of type: {subject.GetType()}.\r\n{e.Message}", e);
            }

            return serializeObject;
        }

        public static T JSONUnserialize<T>(this string stringObj) {
            var unserialize = (T)JsonConvert.DeserializeObject(stringObj, typeof(T));
            var uIsList = unserialize.GetType().IsGenericType && unserialize is IEnumerable;

            // -- performance optimization
            if (uIsList)
                return unserialize;

            var item = (T)Activator.CreateInstance(typeof(T));
            var iIsList = item.GetType().IsGenericType && item is IEnumerable;

            if (!iIsList)
                return unserialize;

            //_log.Warn("JSON Object was not deserialized as 'List', now trying special algorithm!");

            return (T)JsonConvert.DeserializeObject(stringObj, typeof(T), new FlexibleCollectionConverter());
        }

        /// <summary>
        ///     FlexibleCollectionConverter
        /// </summary>
        private class FlexibleCollectionConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => serializer.Serialize(writer, value);

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                JsonSerializer serializer) {
                if (reader.TokenType == JsonToken.StartArray)
                    return serializer.Deserialize(reader, objectType);

                var array = new JArray(JToken.ReadFrom(reader));
                return array.ToObject(objectType);
            }

            public override bool CanConvert(Type objectType) => typeof(IEnumerable).IsAssignableFrom(objectType);
        }

        /// <summary>
        ///     GenericListCreationJsonConverter
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        private class GenericListCreationJsonConverter<T> : JsonConverter
        {
            public override bool CanRead => true;
            public override bool CanWrite => false;
            public override bool CanConvert(Type objectType) => true;

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                JsonSerializer serializer) {
                if (reader.TokenType == JsonToken.StartArray)
                    return serializer.Deserialize<List<T>>(reader);
                var t = serializer.Deserialize<T>(reader);
                return new List<T>(new[] { t });
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
        }
    }
}
