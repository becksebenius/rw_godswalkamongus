using HarmonyLib;
using Verse;

namespace GodsWalkAmongUs.HarmonyPatches
{
    [StaticConstructorOnStartup]
    public static class HarmonyInit
    {
        static HarmonyInit()
        {
            new Harmony("GWAU.Mod").PatchAll();
        }
    }
}