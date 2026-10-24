/***************************************************************************
 * DynamicJson - the object CurlResult.AsJsonDynamic() returns on System.Text.Json builds.
 *
 * JsonDocument is not a dynamic object: `d.rate.remaining` on it throws RuntimeBinderException.
 * This converts the parsed document into plain .NET values once (objects become DynamicJsonObject,
 * arrays List<object?>, numbers int/long/double, strings, bools and null), so member access,
 * indexers and foreach all work the way they do on Newtonsoft's JObject.
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

#if !NETSTANDARD2_0
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.Json;

namespace CurlDotNet.Core
{
    /// <summary>
    /// Converts a parsed JSON document into values that C# <c>dynamic</c> can navigate.
    /// </summary>
    internal static class DynamicJson
    {
        /// <summary>Parses <paramref name="json"/> and converts the whole tree.</summary>
        public static object? Parse(string json)
        {
            using var document = JsonDocument.Parse(json, new JsonDocumentOptions { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip });
            return Convert(document.RootElement);
        }

        /// <summary>Converts one element (recursively).</summary>
        public static object? Convert(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    var obj = new DynamicJsonObject(element.GetRawText());
                    foreach (var property in element.EnumerateObject())
                    {
                        obj.Values[property.Name] = Convert(property.Value);
                    }
                    return obj;
                case JsonValueKind.Array:
                    return element.EnumerateArray().Select(Convert).ToList();
                case JsonValueKind.String:
                    return element.GetString();
                case JsonValueKind.Number:
                    if (element.TryGetInt32(out var i)) return i;
                    if (element.TryGetInt64(out var l)) return l;
                    return element.GetDouble();
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// A JSON object for <c>dynamic</c> code: <c>d.name</c>, <c>d["name"]</c>, <c>foreach (var p in d)</c>.
    /// A missing property reads as null, like Newtonsoft's JObject.
    /// </summary>
    internal sealed class DynamicJsonObject : DynamicObject, IEnumerable<KeyValuePair<string, object?>>
    {
        private readonly string _rawJson;

        public DynamicJsonObject(string rawJson)
        {
            _rawJson = rawJson;
        }

        /// <summary>The properties in document order (JSON property names are case-sensitive).</summary>
        public Dictionary<string, object?> Values { get; } = new Dictionary<string, object?>(StringComparer.Ordinal);

        private object? Lookup(string name)
        {
            if (Values.TryGetValue(name, out var value))
            {
                return value;
            }
            // d.Name for {"name": ...}: fall back to a case-insensitive match when it is unambiguous
            var matches = Values.Where(p => string.Equals(p.Key, name, StringComparison.OrdinalIgnoreCase)).ToList();
            return matches.Count == 1 ? matches[0].Value : null;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object? result)
        {
            result = Lookup(binder.Name);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object? value)
        {
            Values[binder.Name] = value;
            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object? result)
        {
            if (indexes.Length == 1 && indexes[0] is string name)
            {
                result = Lookup(name);
                return true;
            }
            result = null;
            return false;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object? value)
        {
            if (indexes.Length == 1 && indexes[0] is string name)
            {
                Values[name] = value;
                return true;
            }
            return false;
        }

        public override bool TryConvert(ConvertBinder binder, out object? result)
        {
            if (binder.Type.IsAssignableFrom(typeof(Dictionary<string, object?>)))
            {
                result = Values;
                return true;
            }
            if (binder.Type == typeof(string))
            {
                result = _rawJson;
                return true;
            }
            result = null;
            return false;
        }

        public override IEnumerable<string> GetDynamicMemberNames() => Values.Keys;

        public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() => Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>The object's JSON text, as it appeared in the response.</summary>
        public override string ToString() => _rawJson;
    }
}
#endif
