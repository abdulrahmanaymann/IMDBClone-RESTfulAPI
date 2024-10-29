using IMDbClone.Core.Enums;
using Newtonsoft.Json;

namespace IMDbClone.Core.Utilities;

public class GenreEnumConverter : JsonConverter<GenreEnum>
{
    public override void WriteJson(JsonWriter writer, GenreEnum value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString());
    }

    public override GenreEnum ReadJson(JsonReader reader, Type objectType, GenreEnum existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Integer)
        {
            int enumValue = Convert.ToInt32(reader.Value);
            return (GenreEnum)enumValue;
        }
        else if (reader.TokenType == JsonToken.String)
        {
            return Enum.Parse<GenreEnum>(reader.Value.ToString());
        }
        
        throw new JsonSerializationException("Unexpected token type for GenreEnum.");
    }
}