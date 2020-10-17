using System.Diagnostics.CodeAnalysis;

namespace FlightAction.Api
{
    public static partial class ApiCollection
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public struct FileUploadApi
        {
            public const string Version = "v1";
            public const string Segment = "file/upload";
        }
    }
}
