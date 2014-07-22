using Sweetspot.Util;

namespace Sweetspot.ScreenManagement
{
    public enum Cue
    {
        [StringValue("Arrow")]
        BaselineArrow = 0,

        [StringValue("Text")]
        BaselineText = 1,

        [StringValue("Brightness")]
        Brightness = 2,

        [StringValue("Contrast")]
        Contrast = 3,

        [StringValue("Saturation")]
        Saturation = 4,

        [StringValue("Pixelate")]
        Pixelate = 5,

        [StringValue("Distort")]
        Distort = 6,

        [StringValue("Jitter")]
        Jitter = 7
    }
}
