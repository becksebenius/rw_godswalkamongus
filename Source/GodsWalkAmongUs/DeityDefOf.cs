using RimWorld;
using Verse;

namespace GodsWalkAmongUs
{
    [DefOf]
    public static class DeityDefOf
    {
        // Factions
        public static FactionDef DeityFaction;
        
        // PawnKind
        public static PawnKindDef DeityPawn;
        
        static DeityDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof (DeityDefOf));
    }
}