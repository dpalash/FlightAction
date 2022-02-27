namespace FlightAction.Core.Api
{
    public static partial class ApiCollection
    {
        public const string DefaultHeader = "ProApiVersion";
        public const string AuthorizationHeader = "Authorization"; 

        public struct AuthenticationApi
        {
            public const string DefaultVersion = "1.0";
            public const string Segment = "authentication/authenticate";
        }

        public struct FileUploadApi
        {
            public const string DefaultVersion = "1.0";
            public const string Segment = "/ticket/register";
        }
    }
}
