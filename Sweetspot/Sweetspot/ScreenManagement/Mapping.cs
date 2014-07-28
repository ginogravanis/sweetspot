using SweetspotApp.Util;

namespace SweetspotApp.ScreenManagement
{
    public enum Mapping
    {
        [StringValue("Linear")]
        Linear = 0,

        [StringValue("QuickStart")]
        QuickStart = 1,

        [StringValue("SlowStart")]
        SlowStart = 2,

        [StringValue("SCurve")]
        SCurve = 3,
    }
}
