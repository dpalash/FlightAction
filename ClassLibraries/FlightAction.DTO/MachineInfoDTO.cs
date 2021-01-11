using System;
using Newtonsoft.Json;

namespace FlightAction.DTO
{
    [Serializable]
    public class MachineInfoDTO
    {
        [JsonProperty("osVersion")]
        public string OsVersion { get; set; }

        [JsonProperty("os64")]
        public bool Os64 { get; set; }

        [JsonProperty("machineName")]
        public string MachineName { get; set; }

        [JsonProperty("numberOfCpu")]
        public int NumberOfCpu { get; set; }

        protected MachineInfoDTO()
        {
            OsVersion = Environment.OSVersion.ToString();
            Os64 = Environment.Is64BitOperatingSystem;
            MachineName = Environment.MachineName;
            NumberOfCpu = Environment.ProcessorCount;
        }

        public static MachineInfoDTO Create()
        {
            return new MachineInfoDTO();
        }
    }
}
