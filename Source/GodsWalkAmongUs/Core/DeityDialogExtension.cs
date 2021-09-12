using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace GodsWalkAmongUs
{
    public static class DeityDialogExtension
    {
        private static readonly float EditFieldHeight = 30f;
        private static readonly float EditFieldRatio = 1 / 3f;

        class DeityDialogExtensionState
        {
            public Ideo ideo;
            public IdeoFoundation_Deity.Deity deity;

            public List<DeityDomainDef> domains = new List<DeityDomainDef>();
        }

        private static DeityDialogExtensionState currentState; 

        public static void Draw(Rect windowRect, Ideo ideo, IdeoFoundation_Deity.Deity deity)
        {
            if (currentState == null
            || currentState.ideo != ideo 
            || currentState.deity != deity)
            {
                currentState = new DeityDialogExtensionState
                {
                    ideo = ideo,
                    deity = deity
                };
                InitCurrentState();
            }
            
            float headerHeight = 35f;
            var rect = new Rect(windowRect.x, windowRect.y + headerHeight, windowRect.width, 0f);
            Rect left, right;
            
            // Exiting row - name
            rect.NextRow(EditFieldHeight, EditFieldRatio); 
            
            // Overwriting existing row - Type
            rect.NextRow(EditFieldHeight, EditFieldRatio, out left, out right);
            {
                Widgets.Label(left, "DeityDomain".Translate());
                Text.Anchor = TextAnchor.MiddleLeft;

                string label;
                if (currentState.domains.Count == 0)
                {
                    label = "None";
                }
                else
                {
                    label = "";
                    for (int i = 0; i < currentState.domains.Count; ++i)
                    {
                        var domain = currentState.domains[i];
                        label += domain.label;
                        if (i != currentState.domains.Count - 1)
                        {
                            label += ", ";
                        }
                    }
                }
                Widgets.Label(right, label);
                Text.Anchor = TextAnchor.UpperLeft;
                Widgets.DrawHighlightIfMouseover(right);
                if (Widgets.ButtonInvisible(right))
                {
                    List<FloatMenuOption> options = new List<FloatMenuOption>();
                    foreach (var domainDef in DefDatabase<DeityDomainDef>.AllDefs)
                    {
                        var domainDefCopy = domainDef;
                        var domainDefLabel = domainDef.label;

                        bool isIncluded = currentState.domains.Contains(domainDef);
                        if (isIncluded)
                        {
                            domainDefLabel = "* " + domainDefLabel;
                        }
                        options.Add(new FloatMenuOption(domainDefLabel, () =>
                        {
                            if (isIncluded)
                            {
                                currentState.domains.Remove(domainDefCopy);
                            }
                            else
                            {
                                currentState.domains.Add(domainDefCopy);
                            }
                        }));
                    }
                    Find.WindowStack.Add((Window) new FloatMenu(options));
                }
            }
            
            // Exiting row - gender
            rect.NextRow(EditFieldHeight, EditFieldRatio);
        }

        static void InitCurrentState()
        {
            var deityInfo = DeityTracker.Instance.GetOrCreateDeityInfo(currentState.ideo, currentState.deity);
            currentState.domains.Clear();
            currentState.domains.AddRange(deityInfo.Domains);
        }
        
        public static void ApplyChanges()
        {
            var deityInfo = DeityTracker.Instance.GetOrCreateDeityInfo(currentState.ideo, currentState.deity);
            deityInfo.Domains.Clear();
            deityInfo.Domains.AddRange(currentState.domains);

            deityInfo.Deity.type = DeityInfoGeneration.GenerateTypeString(deityInfo);
            
            currentState = null;
        }
    }
}