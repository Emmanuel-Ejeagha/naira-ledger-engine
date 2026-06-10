using System.Text.Json;
using System.Text.Json.Serialization;

namespace NairaLedger.Domain.ValueObjects;

public class IdempotencyKeyConverter : JsonConverter<IdempotencyKey>
{
    public override IdempotencyKey? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();
            return value is not null ? new IdempotencyKey(value) : null;
        }
        else if (reader.TokenType == JsonTokenType.StartObject)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            var root = doc.RootElement;
            if (root.TryGetProperty("value", out var valueProp) &&
                valueProp.ValueKind == JsonValueKind.String)
            {
                return new IdempotencyKey(valueProp.GetString()!);
            }
        }

        throw new JsonException("IdempotencyKey must be a string or an object with a 'value' property.");
    }

    public override void Write(Utf8JsonWriter writer, IdempotencyKey value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value);
    }
}