using RimWorld;
using UnityEngine;
using Verse;

namespace GodsWalkAmongUs
{
    public static class DeityListDrawer
    {
        public static readonly Vector2 PawnPortraitSize = new Vector2(92f, 128f);
        
        public static void DrawDeities(
            IdeoFoundation_Deity ideoFoundation,
            ref float curY,
            float width,
            IdeoEditMode editMode)
        {
            for (int index = 0; index < ideoFoundation.DeitiesListForReading.Count; ++index)
            {
                IdeoFoundation_Deity.Deity curDeity = ideoFoundation.DeitiesListForReading[index];
                var deityInfo = DeityTracker.Instance.GetOrCreateDeityInfo(ideoFoundation.ideo, curDeity);
                var pawn = DeityTracker.Instance.GetOrCreatePawnForDeity(ideoFoundation.ideo, curDeity);
                
                var rect = new Rect(0, curY, width, 100);
                Widgets.DrawLightHighlight(rect);
                
                if (Mouse.IsOver(rect))
                {
                    Widgets.DrawHighlight(rect);
                    string str = curDeity.name.Colorize(ColoredText.TipSectionTitleColor) + "\n" + curDeity.type;
                    TooltipHandler.TipRegion(rect, (TipSignal) str);
                }
                
                var portraitRect = rect;
                portraitRect.width = PawnPortraitSize.x / PawnPortraitSize.y * rect.height;
                RenderTexture renderTexture = PortraitsCache.Get(pawn, PawnPortraitSize, Rot4.South, default(Vector3), renderHeadgear: true, renderClothes: true, stylingStation: true);
                GUI.DrawTexture(portraitRect, renderTexture);

                Rect rightArea;
                rect.Split(portraitRect.width/rect.width, out _, out rightArea);
                
                Widgets.Label(rightArea, curDeity.name);

                curY += rect.height;
                curY += 4;
            }
        }
    }
}