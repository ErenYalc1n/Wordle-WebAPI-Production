using System.Text.Json;
using System.Text.Json.Serialization;

namespace Wordle.Infrastructure.JsonConverters;

public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    private const string Format = "dd.MM.yyyy";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        if (DateOnly.TryParseExact(value, Format, out var date))
        {
            return date;
        }

        throw new JsonException($"Geçersiz tarih formatı. Beklenen format: {Format}");
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(Format));
    }
}
