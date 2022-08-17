using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Hammock.AssetView.Platinum.Tools.RC.RelayService.Common
{
    public static class SettingsFramework
    {
        private readonly static Regex _regex = new Regex(@"^.*?\[Start-Section\](.*)\[End-Section\].*?$", RegexOptions.Singleline | RegexOptions.Compiled);

        public static T Load<T>(string path)
        {
            if (!File.Exists(path)) throw new FileNotFoundException(path);

            string content = Encrypt.DecryptFromFile(Path.GetFullPath(path));

            var match = _regex.Match(content);
            if (!match.Success) throw new ArgumentException();

            using (var reader = new StringReader(match.Groups[1].Value.Trim()))
            {
                return JsonUtils.FromJson<T>(reader);
            }
        }

        public static void Save<T>(string path, T value)
        {
            var sb = new StringBuilder();

            _ = sb.AppendLine("[Start-Section]");

            using (var writer = new StringWriter(sb))
            {
                JsonUtils.ToJson(writer, value);
            }

            _ = sb.AppendLine();
            _ = sb.AppendLine("[End-Section]");

            Encrypt.EncryptToFile(Path.GetFullPath(path), sb.ToString());
        }
    }
}
