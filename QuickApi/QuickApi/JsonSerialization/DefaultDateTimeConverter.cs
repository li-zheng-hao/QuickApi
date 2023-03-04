using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace QuickApi.JsonSerialization;


public class DefaultDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Debug.Assert(typeToConvert == typeof(DateTime));
        return DateTime.Parse(reader.GetString());
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToLocalTime().ToString("yyyy-M-d HH:mm:ss"));
    }
}

