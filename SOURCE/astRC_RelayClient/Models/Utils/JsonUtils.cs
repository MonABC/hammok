using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayClient.Models.Utils
{
    public static class JsonUtils
    {
        public static void ToJson<T>(Stream stream, T value)
        {
            using (var writer = new StreamWriter(stream))
            {
                ToJson<T>(writer, value);
            }
        }

        public static void ToJson<T>(TextWriter writer, T value)
        {
#if DEBUG
            var sb = new StringBuilder();

            using (var textWriter = new StringWriter(sb))
            using (var jsonTextWriter = new JsonTextWriter(textWriter))
#else
            using (var jsonTextWriter = new JsonTextWriter(writer))
#endif
            {
                jsonTextWriter.Formatting = Formatting.Indented;

                var serializer = GetSerializer();
                serializer.Serialize(jsonTextWriter, value);
            }

#if DEBUG
            var text = sb.ToString();

            Logger.Debug(string.Format("ToJson: {0}", text));

            writer.Write(text);
#endif
        }

        public static T FromJson<T>(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                return FromJson<T>(reader);
            }
        }

        public static T FromJson<T>(TextReader reader)
        {
#if DEBUG
            var text = reader.ReadToEnd();

            Logger.Debug(string.Format("FromJson: {0}", text));

            using (var textReader = new StringReader(text))
            using (var jsonTextReader = new JsonTextReader(textReader))
#else
            using (var jsonTextReader = new JsonTextReader(reader))
#endif
            {
                var serializer = GetSerializer();
                return serializer.Deserialize<T>(jsonTextReader);
            }
        }

        private static JsonSerializer GetSerializer()
        {
            var serializer = new JsonSerializer();
            serializer.Converters.Add(new IsoDateTimeConverter() { DateTimeFormat = "yyyy/MM/ddTHH:mm:ssZ" });
            serializer.Converters.Add(new StringEnumConverter());
            return serializer;
        }
    }
}
