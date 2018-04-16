namespace Border.Model
{
    using System.ComponentModel;
    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class SettingsData
    {
        [DefaultValue("#FFFFFF")]
        [JsonProperty("BackgroundColor", NullValueHandling = NullValueHandling.Ignore)]
        public string BackgroundColor { get; set; } = "#FFFFFF";

        [DefaultValue("#12ACFF")]
        [JsonProperty("ActiveColor", NullValueHandling = NullValueHandling.Ignore)]
        public string ActiveColor { get; set; } = "#12ACFF";

        [DefaultValue("#000000")]
        [JsonProperty("BorderColor", NullValueHandling = NullValueHandling.Ignore)]
        public string BorderColor { get; set; } = "#000000";

        [DefaultValue("#000000")]
        [JsonProperty("FontColor", NullValueHandling = NullValueHandling.Ignore)]
        public string FontColor { get; set; } = "#000000";

        [DefaultValue(150.0)]
        [JsonProperty("PanelWidth", NullValueHandling = NullValueHandling.Ignore)]
        public double PanelWidth { get; set; } = 150;
        [DefaultValue(50.0)]
        [JsonProperty("PanelHeight", NullValueHandling = NullValueHandling.Ignore)]
        public double PanelHeight { get; set; } = 50;

        [DefaultValue(true)]
        [JsonProperty("WarnClickThrough", NullValueHandling = NullValueHandling.Ignore)]
        public bool WarnClickThrough { get; set; } = true;

        private double _Opacity = 0.7;
        [DefaultValue(0.6)]
        [JsonProperty("Opacity", NullValueHandling = NullValueHandling.Ignore)]
        public double Opacity
        {
            get
            {
                return _Opacity;
            }
            set
            {
                if (value < 0.1)
                {
                    _Opacity = 0.1;
                }
                else
                {
                    _Opacity = value;
                }
            }
        }

        private double _ClickThroughOpacity = 0.5;

        [DefaultValue(0.3)]
        [JsonProperty("ClickThroughOpacity", NullValueHandling = NullValueHandling.Ignore)]
        public double ClickThroughOpacity
        {
            get
            {
                return _ClickThroughOpacity;
            }
            set
            {
                if (value < 0.1)
                {
                    _ClickThroughOpacity = 0.1;
                }
                else
                {
                    _ClickThroughOpacity = value;
                }
            }
        }

        [DefaultValue(true)]
        [JsonProperty("AlwaysOnTop", NullValueHandling = NullValueHandling.Ignore)]
        public bool AlwaysOnTop { get; set; } = true;

        [DefaultValue(false)]
        [JsonProperty("ClickThrough", NullValueHandling = NullValueHandling.Ignore)]
        public bool ClickThrough { get; set; } = false;

        [DefaultValue(false)]
        [JsonProperty("Compact", NullValueHandling = NullValueHandling.Ignore)]
        public bool Compact { get; set; } = false;

        [DefaultValue(false)]
        [JsonProperty("Vertical", NullValueHandling = NullValueHandling.Ignore)]
        public bool Vertical { get; set; } = false;

        [JsonProperty("Buttons", NullValueHandling = NullValueHandling.Ignore)]
        public Buttons Buttons { get; set; } = new Buttons();

        [DefaultValue(5)]
        [JsonProperty("X", NullValueHandling = NullValueHandling.Ignore)]
        public long X { get; set; } = 5;

        [DefaultValue(5)]
        [JsonProperty("Y", NullValueHandling = NullValueHandling.Ignore)]
        public long Y { get; set; } = 5;

        [DefaultValue(800)]
        [JsonProperty("HorizontalWidth", NullValueHandling = NullValueHandling.Ignore)]
        public double HorizontalWidth { get; set; } = 800;

        [DefaultValue(150)]
        [JsonProperty("HorizontalHeight", NullValueHandling = NullValueHandling.Ignore)]
        public double HorizontalHeight { get; set; } = 150;

        [DefaultValue(800)]
        [JsonProperty("VerticalWidth", NullValueHandling = NullValueHandling.Ignore)]
        public double VerticalWidth { get; set; } = 200;

        [DefaultValue(150)]
        [JsonProperty("VerticalHeight", NullValueHandling = NullValueHandling.Ignore)]
        public double VerticalHeight { get; set; } = 800;

        [JsonProperty("LastBO")]
        public string LastBO { get; set; } = null;

        [DefaultValue("http://abductedplatypus.com/BuildOrderOverlay/updates/update.json")]
        [JsonProperty("UpdateURL")]
        public string UpdateURL { get; set; } = "http://abductedplatypus.com/BuildOrderOverlay/updates/update.json";
    }

    public partial class Buttons
    {
        [JsonProperty("ToggleClickThrough", NullValueHandling = NullValueHandling.Ignore)]
        public ButtonData ToggleClickThrough { get; set; } = new ButtonData { KeyCode = "0xDC", Alt = true, HumanReadable = "Alt+|" };

        [JsonProperty("NextBO", NullValueHandling = NullValueHandling.Ignore)]
        public ButtonData NextBO { get; set; } = new ButtonData { KeyCode = "0x26", Alt = true, HumanReadable = "Alt+Up" };

        [JsonProperty("PreviousBO", NullValueHandling = NullValueHandling.Ignore)]
        public ButtonData PreviousBO { get; set; } = new ButtonData { KeyCode = "0x28", Alt = true, HumanReadable = "Alt+Down" };

        [JsonProperty("NextTask", NullValueHandling = NullValueHandling.Ignore)]
        public ButtonData NextTask { get; set; } = new ButtonData { KeyCode = "0x20", Alt = true, HumanReadable = "Alt+Space", };

        [JsonProperty("PreviousTask", NullValueHandling = NullValueHandling.Ignore)]
        public ButtonData PreviousTask { get; set; } = new ButtonData { KeyCode = "0x20", Alt = true, Control = true, HumanReadable = "Alt+Ctrl+Space" };

        [JsonProperty("ToggleAlwaysOnTop", NullValueHandling = NullValueHandling.Ignore)]
        public ButtonData ToggleAlwaysOnTop { get; internal set; } = new ButtonData { KeyCode = "0xDC", Alt = true, Control = true, HumanReadable = "Alt+Ctrl+|" };
    }

    public partial class ButtonData
    {
        [JsonProperty("HumanReadable")]
        public string HumanReadable { get; set; }

        [JsonProperty("KeyCode")]
        public string KeyCode { get; set; }

        [JsonProperty("Shift")]
        public bool Shift { get; set; } = false;

        [JsonProperty("Control")]
        public bool Control { get; set; } = false;

        [JsonProperty("Alt")]
        public bool Alt { get; set; } = false;
    }

    public partial class SettingsData
    {
        public static SettingsData FromJson(string json) => JsonConvert.DeserializeObject<SettingsData>(json, SettingsDataConverter.Settings);
    }

    public static class SettingsDataSerialize
    {
        public static string ToJson(this SettingsData self) => JsonConvert.SerializeObject(self, SettingsDataConverter.Settings);
    }

    internal class SettingsDataConverter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            DefaultValueHandling = DefaultValueHandling.Populate,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
