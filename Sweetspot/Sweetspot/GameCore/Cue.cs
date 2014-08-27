using SweetspotApp.Util;

namespace SweetspotApp.GameCore
{
    public enum Cue
    {
        [StringValue("Arrow")]
        Baseline = 0,

        [StringValue("Brightness")]
        Brightness = 1,

        [StringValue("Contrast")]
        Contrast = 2,

        [StringValue("Saturation")]
        Saturation = 3,

        [StringValue("Pixelate")]
        Pixelate = 4,

        [StringValue("Distort")]
        Distort = 5,

        [StringValue("Jitter")]
        Jitter = 6
    }
}
