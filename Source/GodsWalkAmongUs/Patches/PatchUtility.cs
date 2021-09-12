using System;
using System.Collections.Generic;

namespace GodsWalkAmongUs.HarmonyPatches
{
    public static class PatchUtility
    {
        public static void TrackBack<T>(List<T> list, ref int index, Func<T, bool> predicate)
        {
            for (; 0 <= index; --index)
            {
                if (predicate(list[index]))
                {
                    return;
                }
            }
        }
        
        public static void TrackForward<T>(List<T> list, ref int index, Func<T, bool> predicate)
        {
            for (; index < list.Count; ++index)
            {
                if (predicate(list[index]))
                {
                    return;
                }
            }
        }
    }
}