using System.Diagnostics.CodeAnalysis;

namespace Framework.Utility
{
    public static partial class Constants
    {
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public struct DateFormatter
        {
            // naming is intentional for readability
            public const string yyyy_MM_dd = "yyyyMMdd";
            public const string yyyy_MM_dd_Dash_Delimited = "yyyy-MM-dd";
            public const string MM_dd_yyyy_Slash_Delimited = "MM/dd/yyyy";
            public const string yyyy_MM_dd_HH_mm_ss_Not_Delimited = "yyyyMMddTHHmmss";
            public const string yyyy_MM_dd_Dash_Delimited_HH_mm_ss_Colon_Delimited = "yyyy-MM-dd HH:mm:ss";
            public const string yyyy_MM_dd_Not_Delimited_Underscore_hh_mm_ss_Not_Delimited = "yyyyMMdd_hhmmss";
        }
    }
}
