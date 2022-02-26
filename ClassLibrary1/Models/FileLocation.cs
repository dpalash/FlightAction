using System;
using Newtonsoft.Json;

namespace Framework.Models
{
    [Serializable]
    public class FileLocation
    {
        [JsonProperty("air")]
        public string Air { get; set; }

        [JsonProperty("pnr")]
        public string Pnr { get; set; }

        [JsonProperty("mir")]
        public string Mir { get; set; }
    }
}
