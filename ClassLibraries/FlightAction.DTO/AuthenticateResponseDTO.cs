using System;
using Framework.Base.ModelEntity;
using Newtonsoft.Json;

namespace FlightAction.DTO
{
    [Serializable]
    public class AuthenticateResponseDTO : ModelEntityBase
    {
        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("emailAddress")]
        public string EmailAddress { get; set; }

        [JsonProperty("employeeId")]
        public int EmployeeId { get; set; }

        [JsonProperty("companyId")]
        public int CompanyId { get; set; }

        [JsonProperty("roleId")]
        public int RoleId { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("isRemember")]
        public bool IsRemember { get; set; }

        [JsonConstructor]
        protected AuthenticateResponseDTO(int id, string userName, int employeeId, string emailAddress, int roleId, string token, bool isRemember)
        {
            Id = id;
            UserName = userName;
            EmployeeId = employeeId;
            EmailAddress = emailAddress;
            RoleId = roleId;
            Token = token;
            IsRemember = isRemember;
        }

        public static AuthenticateResponseDTO Create(int id, string userName, int employeeId, string emailAddress, int roleId, string token, bool isRemember)
        {
            return new AuthenticateResponseDTO(id, userName, employeeId, emailAddress, roleId, token, isRemember);
        }

        public static AuthenticateResponseDTO Error()
        {
            return null;
        }
    }
}
