using System;
using FlightAction.DTO.Enum;
using Newtonsoft.Json;

namespace FlightAction.DTO
{
    [Serializable]
    public class TicketFileDTO
    {
        [JsonProperty("fileName")]
        public string FileName { get; set; }

        [JsonProperty("fileType")]
        public FileTypeEnum FileType { get; set; }

        [JsonProperty("machineInfoDTO")]
        public MachineInfoDTO MachineInfoDTO { get; set; }

        [JsonProperty("employeeId")]
        public int EmployeeId { get; set; }

        [JsonProperty("companyId")]
        public int CompanyId { get; set; }

        [JsonProperty("fileBytes")]
        public byte[] FileBytes { get; set; }
    }
}
