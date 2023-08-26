using UnityEngine;

namespace Util
{
    public static class ColorUtil
    {
        public static Color WithAlpha(this Color c, float a)
        {
            return new Color(c.r, c.g, c.b, a);
        }
    }
}