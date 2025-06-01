using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StockPriceMonitoringAndAlerts.Converters
{
    public class DateTimeConverter_ddMMyyyy_HHmmss : JsonConverter<DateTime>
    {
        private static readonly string[] AllowedFormats = new[]
        {
        "dd/MM/yyyy HH:mm:ss",
        "dd/MM/yyyy"
        };

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (value is null)
                throw new JsonException("Date string is null.");

            if (DateTime.TryParseExact(value, AllowedFormats, CultureInfo.InvariantCulture,
                                       DateTimeStyles.None, out var result))
            {
                return result;
            }

            throw new JsonException($"Invalid date format. Only 'dd/MM/yyyy HH:mm:ss' and 'dd/MM/yyyy' are allowed.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            // Always output full format with time
            writer.WriteStringValue(value.ToString("dd/MM/yyyy HH:mm:ss"));
        }
    }
}
