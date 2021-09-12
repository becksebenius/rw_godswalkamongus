using System.Collections.Generic;
using RimWorld;
using Verse;

namespace GodsWalkAmongUs
{
    public static class DeityInfoGeneration
    {
        public static void Generate(DeityInfo deityInfo)
        {
            deityInfo.Domains.Clear();

            SelectRandomDomain(deityInfo);
            deityInfo.Deity.type = GenerateTypeString(deityInfo);
            Log.Message("New type: " + deityInfo.Deity.type);
        }

        public static string GenerateTypeString(DeityInfo deityInfo)
        {
            string value = "God of ";
            for (int i = 0; i < deityInfo.Domains.Count; ++i)
            {
                var domain = deityInfo.Domains[i];
                if (i == deityInfo.Domains.Count - 1)
                {
                    if (1 < deityInfo.Domains.Count)
                    {
                        value += " and ";
                    }
                }
                else
                {
                    if (i != 0)
                    {
                        value += ", ";
                    }
                }
                value += domain.label;
            }

            return value;
        }

        private static readonly int[] numDomainsToSelect =
        {
            1,1,1,1,1,1,1,1,1,
            2,2,2,2,
            3,3
        };

        static void SelectRandomDomain(DeityInfo deityInfo)
        {
            int numDomains = numDomainsToSelect[Rand.Range(0, numDomainsToSelect.Length)];

            for (int i = 0; i < numDomains; ++i)
            {
                int j = 0;
                do
                {
                    var domain = SelectRandomDomain();

                    if (!((IdeoFoundation_Deity) deityInfo.Ideo.foundation)
                        .DeitiesListForReading
                        .Any(d => 
                            DeityTracker.Instance
                                .GetDeityInfo(deityInfo.Ideo, d)
                                ?.Domains.Contains(domain) 
                                ?? false))
                    {
                        deityInfo.Domains.Add(domain);
                        break;
                    }
                } while (j++ < 10);
            }
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