using System.Collections.Generic;
using Verse;

namespace GodsWalkAmongUs
{
    public static class DeityInfoGeneration
    {
        public static void Generate(DeityInfo deityInfo)
        {
            deityInfo.Domain = SelectRandomDomain();
        }

        static DeityDomainDef SelectRandomDomain()
        {
            List<DeityDomainDef> selectedDomains = new List<DeityDomainDef>();
            List<int> incrementalWeights = new List<int>();

            foreach (var domainDef in DefDatabase<DeityDomainDef>.AllDefsListForReading)
            {
                selectedDomains.Add(domainDef);
                incrementalWeights.Add(domainDef.generationWeight + (incrementalWeights.Count == 0 ? 0 : incrementalWeights[incrementalWeights.Count-1]));
            }

            if (incrementalWeights.Count == 0)
            {
                return null;
            }

            int value = Rand.RangeInclusive(0, incrementalWeights[incrementalWeights.Count - 1]);
            for (int i = 0; i < incrementalWeights.Count; ++i)
            {
                var weight = incrementalWeights[i];
                if (value <= weight)
                {
                    return selectedDomains[i];
                }
            }

            return null;
        }
    }
}