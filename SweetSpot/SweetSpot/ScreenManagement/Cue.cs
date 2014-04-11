using SweetSpot.Util;

namespace SweetSpot.ScreenManagement
{
    public enum Cue
    {
        [StringValue("Baseline")]
        Baseline = 0,

        [StringValue("Arrow")]
        BaselineArrow = 1,

        [StringValue("Text")]
        BaselineText = 2,

        [StringValue("Brightness")]
        Brightness = 3,

        [StringValue("Contrast")]
        Contrast = 4,

        [StringValue("Saturation")]
        Saturation = 5,

        [StringValue("Pixelate")]
        Pixelate = 6,

        [StringValue("Distort")]
        Distort = 7,

        [StringValue("Jitter")]
        Jitter = 8
    }
}
