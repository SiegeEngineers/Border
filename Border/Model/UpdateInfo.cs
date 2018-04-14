namespace Border.Model
{
    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class UpdateInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("latest")]
        public History Latest { get; set; }

        [JsonProperty("history")]
        public History[] History { get; set; }
    }

    public partial class History
    {
        [JsonProperty("id")]
        public long? Id { get; set; }

        [JsonProperty("date")]
        public System.DateTimeOffset? Date { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }
    }

    public partial class UpdateInfo
    {
        public static UpdateInfo FromJson(string json) => JsonConvert.DeserializeObject<UpdateInfo>(json, UpdateInfoConverter.Settings);
    }

    public static class UpdateInfoSerialize
    {
        public static string ToJson(this UpdateInfo self) => JsonConvert.SerializeObject(self, UpdateInfoConverter.Settings);
    }

    internal class UpdateInfoConverter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
