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

            public DeityDomainDef domain;
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
            
            float builtinHeight = EditFieldHeight * 4 + 35f;
            var rect = new Rect(windowRect.x, windowRect.y + builtinHeight, windowRect.width, 0f);
            Rect left, right;
            
            Row(ref rect, out left, out right);
            {
                Widgets.Label(left, "DeityDomain".Translate());
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label(right, currentState.domain.label);
                Text.Anchor = TextAnchor.UpperLeft;
                Widgets.DrawHighlightIfMouseover(right);
                if (Widgets.ButtonInvisible(right))
                {
                    List<FloatMenuOption> options = new List<FloatMenuOption>();
                    foreach (var domainDef in DefDatabase<DeityDomainDef>.AllDefs)
                    {
                        var domainDefCopy = domainDef;
                        options.Add(new FloatMenuOption(domainDef.label, () => currentState.domain = domainDefCopy));
                    }
                    Find.WindowStack.Add((Window) new FloatMenu(options));
                }
            }
        }

        static void InitCurrentState()
        {
            var deityInfo = DeityTracker.Instance.GetOrCreateDeityInfo(currentState.ideo, currentState.deity);
            currentState.domain = deityInfo.Domain;
        }
        
        public static void ApplyChanges()
        {
            var deityInfo = DeityTracker.Instance.GetOrCreateDeityInfo(currentState.ideo, currentState.deity);
            deityInfo.Domain = currentState.domain;
            
            currentState = null;
        }

        static void Row(ref Rect rect, out Rect left, out Rect right)
        {
            NextRow(ref rect, EditFieldHeight);
            Split(rect, EditFieldRatio, out left, out right);
        }
        
        static void Split(Rect rect, float ratio, out Rect left, out Rect right)
        {
            left = new Rect(rect.x, rect.y, rect.width * ratio, rect.height);
            right = new Rect(left.x + left.width, rect.y, rect.width - left.width, rect.height);
        }
        
        static void NextRow(ref Rect rect, float rowHeight)
        {
            rect.y += rect.height + 10;
            rect.height = rowHeight;
        }
    }
}