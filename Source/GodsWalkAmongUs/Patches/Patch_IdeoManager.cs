using HarmonyLib;
using RimWorld;

namespace GodsWalkAmongUs.HarmonyPatches
{
    [HarmonyPatch(typeof(IdeoManager), nameof(IdeoManager.Remove))]
    public class IdeoManager_Remove
    {
        public static void Postfix(Ideo ideo, bool __result)
        {
            if (__result)
            {
                DeityTracker.Instance.OnIdeoRemoved(ideo);
            }
        }
    }
    
    [HarmonyPatch(typeof(IdeoManager), nameof(IdeoManager.Add))]
    public class IdeoManager_Add
    {
        public static void Postfix(Ideo ideo, bool __result)
        {
            if (__result)
            {
                DeityTracker.Instance.OnIdeoAdded(ideo);
            }
        }
    }
}