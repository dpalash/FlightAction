using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace FlightAction.DTO
{
    [Serializable]
    public class AuthenticateRequestDTO
    {
        [JsonProperty("userName")]
        [Required]
        public string UserName { get; set; }

        [JsonProperty("password")]
        [Required]
        public string Password { get; set; }

        [JsonProperty("isRemember")]
        public bool IsRemember { get; set; }
    }
}
