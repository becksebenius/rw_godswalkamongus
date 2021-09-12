using UnityEngine;

namespace GodsWalkAmongUs
{
    public static class RectExtensions
    {
        public static void NextRow(this Rect rect, float rowHeight, float ratio)
        {
            rect.NextRow(rowHeight, ratio, out Rect left, out Rect right);
        }
        
        public static void NextRow(this Rect rect, float rowHeight, float ratio, out Rect left, out Rect right)
        {
            rect.NextRow(rowHeight);
            rect.Split(ratio, out left, out right);
        }
        
        public static void Split(this Rect rect, float ratio, out Rect left, out Rect right)
        {
            left = new Rect(rect.x, rect.y, rect.width * ratio, rect.height);
            right = new Rect(left.x + left.width, rect.y, rect.width - left.width, rect.height);
        }
        
        public static void NextRow(this Rect rect, float rowHeight)
        {
            rect.y += rect.height + 10;
            rect.height = rowHeight;
        }
    }
}