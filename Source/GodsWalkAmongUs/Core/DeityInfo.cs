using System.Collections.Generic;
using RimWorld;
using Verse;

namespace GodsWalkAmongUs
{
    public class DeityInfo : IExposable
    {
        public Ideo Ideo;
        public int DeityId;

        public IdeoFoundation_Deity.Deity Deity =>
            ((IdeoFoundation_Deity) Ideo.foundation).DeitiesListForReading[DeityId];

        public Faction Faction;
        public Pawn Pawn;

        public List<DeityDomainDef> Domains = new List<DeityDomainDef>();
        
        // Serialization constructor
        public DeityInfo() {}
        public DeityInfo(Ideo ideo, int deityId)
        {
            Ideo = ideo;
            DeityId = deityId;
        }

        public void ExposeData()
        {
            Scribe_References.Look(ref Ideo, nameof(Ideo));
            Scribe_Values.Look(ref DeityId, nameof(DeityId));
            Scribe_References.Look(ref Faction, nameof(Faction));
            Scribe_References.Look(ref Pawn, nameof(Pawn));
            Scribe_Collections.Look(ref Domains, nameof(Domains));
        }
    }
}