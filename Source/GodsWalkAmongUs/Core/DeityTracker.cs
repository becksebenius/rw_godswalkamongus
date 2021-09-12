using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace GodsWalkAmongUs
{
    [StaticConstructorOnStartup]
    public class DeityTracker : GameComponent
    {
        public static int GetDeityId(Ideo ideo, IdeoFoundation_Deity.Deity deity)
            => ((IdeoFoundation_Deity) ideo.foundation).DeitiesListForReading.IndexOf(deity);
        
        public static DeityTracker Instance { get; private set; }

        private List<DeityInfo> deities = new List<DeityInfo>();

        public DeityTracker(Game game)
        {
            Instance = this;
        }

        public void OnIdeoAdded(Ideo ideo)
        {
            Log.Message("Ideo added: " + ideo.name);
        }

        public void OnIdeoRemoved(Ideo ideo)
        {
            Log.Message("Ideo removed: " + ideo.name);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref deities, "deities");
        }

        public Pawn GetOrCreatePawnForDeity(Ideo ideo, IdeoFoundation_Deity.Deity deity)
            => GetOrCreatePawnForDeity(ideo, GetDeityId(ideo, deity));
        public Pawn GetOrCreatePawnForDeity(Ideo ideo, int deityId)
        {
            var deityInfo = GetOrCreateDeityInfo(ideo, deityId);

            if (deityInfo.Pawn == null)
            {
                CreatePawn(deityInfo);
            }

            return deityInfo.Pawn;
        }

        public DeityInfo GetOrCreateDeityInfo(Ideo ideo, IdeoFoundation_Deity.Deity deity)
            => GetOrCreateDeityInfo(ideo, GetDeityId(ideo, deity));
        public DeityInfo GetOrCreateDeityInfo(Ideo ideo, int deityId)
        {
            var deityInfo = GetDeityInfo(ideo, deityId);
            if (deityInfo != null) return deityInfo;
            
            deityInfo = new DeityInfo(ideo, deityId);
            deities.Add(deityInfo);
            
            DeityInfoGeneration.Generate(deityInfo);
            
            return deityInfo;
        }

        public DeityInfo GetDeityInfo(Ideo ideo, IdeoFoundation_Deity.Deity deity)
            => GetDeityInfo(ideo, GetDeityId(ideo, deity));
        public DeityInfo GetDeityInfo(Ideo ideo, int deityId)
        {
            var deityInfo = deities.FirstOrDefault(info => info.Ideo == ideo && info.DeityId == deityId);
            if (deityInfo != null) return deityInfo;
            return null;
        }

        public void RemoveDeitiesWithIdeo(Ideo ideo)
        {
            deities.RemoveAll(d => d.Ideo == ideo);
        }
        
        void CreatePawn(DeityInfo deityInfo)
        {
            if (deityInfo.Pawn != null)
            {
                return;
            }
            
            CreateFaction(deityInfo);

            Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(DeityDefOf.DeityPawn, deityInfo.Faction)
            {
                FixedIdeo = deityInfo.Ideo,
                ForceNoBackstory = true,
                ForbidAnyTitle = true
            });
            pawn.Name = new NameSingle(deityInfo.Deity.name);

            deityInfo.Faction.leader = pawn;
            deityInfo.Pawn = pawn;
        }

        void CreateFaction(DeityInfo deityInfo)
        {
            if (deityInfo.Faction != null)
            {
                return;
            }
            
            var factionDef = DeityDefOf.DeityFaction;
            var faction = new Faction
            {
                def = factionDef,
                loadID = Find.UniqueIDsManager.GetNextFactionID(),
                color = deityInfo.Ideo.Color,
                hidden = true
            };
            faction.ideos = new FactionIdeosTracker(faction);
            faction.ideos.SetPrimary(deityInfo.Ideo);
            faction.Name = deityInfo.Deity.name;
            faction.centralMelanin = Rand.Value;
            
            // TODO: Establish relationships between the god and other factions

            faction.leader = deityInfo.Pawn;

            Find.FactionManager.Add(faction);
            
            deityInfo.Faction = faction;
        }
    }
}