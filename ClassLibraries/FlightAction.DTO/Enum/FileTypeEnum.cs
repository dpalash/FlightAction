using System.ComponentModel;

namespace FlightAction.DTO.Enum
{
    public enum FileTypeEnum
    {
        [Description(".AIR")]
        Air = 1,

        [Description(".MIR")]
        Mir,

        [Description(".PNR")]
        Pnr,
    }
}
