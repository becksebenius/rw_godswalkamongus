using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace GodsWalkAmongUs
{
    public class GodArrival : IncidentWorker_NeutralGroup
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!(parms.target is Map map))
            {
                Log.Error("target must be a map.");
                return false;
            }

            using (var ideos = ReuseableList<Ideo>.GetInstance())
            {
                GetRelevantIdeos(ideos);

                if (!ideos.All(IsValidIdeoTarget))
                {
                    return false;
                }
            }

            return true;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            using (var ideos = ReuseableList<Ideo>.GetInstance())
            {
                GetRelevantIdeos(ideos);
                ideos.Filter(IsValidIdeoTarget);

                var selectedIdeo = SelectIdeo(ideos);
                var selectedDeity = SelectDeity(selectedIdeo);

                var pawn = CreatePawnForDeity(parms, selectedIdeo, selectedDeity);

                var deityInfo = DeityTracker.Instance.GetOrCreateDeityInfo(selectedIdeo, selectedDeity);
                Find.LetterStack.ReceiveLetter(
                    new TaggedString(selectedDeity.name + " Arrives!"),
                    new TaggedString("A god appears."),
                    LetterDefOf.PositiveEvent);
                return true;
            }
        }

        protected override void ResolveParmsPoints(IncidentParms parms)
        {
            
        }

        Pawn CreatePawnForDeity(IncidentParms parms, Ideo ideo, IdeoFoundation_Deity.Deity deity)
        {
            var map = (Map) parms.target;
            var position = CellFinder.RandomEdgeCell(map);

            var pawn = DeityTracker.Instance.GetOrCreatePawnForDeity(ideo, deity);
            SpawnPawnOnMap(pawn, map, position);
            pawn.Name = new NameSingle(deity.name);
            
            return pawn;
        }
        
        void SpawnPawnOnMap(
            Pawn pawn,
            Map map,
            IntVec3 position)
        {
            // Put the spawn on the map
            GenSpawn.Spawn(pawn, position, map);

            // If the pawn has a faction, and the faction is NOT the player faction
            if (pawn.Faction != null && pawn.Faction != Faction.OfPlayer)
            {
                Lord lord = null;
                
                // If there are any other pawns on the map with the same faction, use the same lord as them
                if (pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction).Any(p => p != pawn))
                {
                    // Find the closest pawn in the faction and use their lord
                    lord = ((Pawn) GenClosest.ClosestThing_Global(
                        pawn.Position,
                        pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction),
                        validator: p => p != pawn && ((Pawn) p).GetLord() != null)).GetLord();
                }

                // Assuming that didn't work
                if (lord == null)
                {
                    // Make a lord that just tells the pawn to defend the position where the pawn spawned
                    LordJob_DefendPoint lordJobDefendPoint = new LordJob_DefendPoint(pawn.Position);
                    lord = LordMaker.MakeNewLord(pawn.Faction, lordJobDefendPoint, map);
                }

                // Tell the lord to direct this pawn
                lord.AddPawn(pawn);
            }
        }
        
        IdeoFoundation_Deity.Deity SelectDeity(Ideo ideo)
        {
            var foundation = (IdeoFoundation_Deity)ideo.foundation;
            var selected = Rand.Range(0, foundation.DeitiesListForReading.Count);
            return foundation.DeitiesListForReading[selected];
        }

        Ideo SelectIdeo(IReadOnlyList<Ideo> ideos)
        {
            int selected = Rand.Range(0, ideos.Count);
            return ideos[selected];
        }

        bool IsValidIdeoTarget(Ideo ideo)
        {
            return ideo.foundation is IdeoFoundation_Deity;
        }

        void GetRelevantIdeos(IList<Ideo> buffer)
        {
            foreach (var ideo in Faction.OfPlayer.ideos.AllIdeos)
            {
                buffer.Add(ideo);
            }
        }
    }
}