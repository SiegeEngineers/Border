
namespace Border.Model
{
    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class BuildOrderData
    {
        [JsonProperty("revision")]
        public long Revision { get; set; }

        [JsonProperty("date")]
        public System.DateTimeOffset Date { get; set; }

        [JsonProperty("game_versions")]
        public string[] GameVersions { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("build_orders")]
        public BuildOrder[] BuildOrders { get; set; }
    }

    public partial class BuildOrder
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("queue")]
        public QueueTask[] Queue { get; set; }
    }

    public partial class QueueTask
    {
        [JsonProperty("time")]
        public long Time { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

    }   

    public partial class BuildOrderData
    {
        public static BuildOrderData FromJson(string json) => JsonConvert.DeserializeObject<BuildOrderData>(json, BuildOrderDataConverter.Settings);
    }

    public static class BuildOrderDataSerialize
    {
        public static string ToJson(this BuildOrderData self) => JsonConvert.SerializeObject(self, BuildOrderDataConverter.Settings);
    }

    internal class BuildOrderDataConverter
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
