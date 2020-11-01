namespace FlightAction.Api
{
    public static partial class ApiCollection
    {
        public const string DefaultHeader = "sf-api-version";

        public struct FileUploadApi
        {
            public const string DefaultVersion = "1.0";
            public const string Segment = "/uploadFile";
        }
    }
}
