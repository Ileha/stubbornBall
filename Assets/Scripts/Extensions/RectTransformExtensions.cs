using UnityEngine;

namespace Extensions
{
    public static class RectTransformExtensions
    {
        private static Vector3[] _corners = new Vector3[4];
        
        public static Rect GetWorldRect(this RectTransform rectTransform)
        {
            rectTransform.GetWorldCorners(_corners);
            
            return new Rect(_corners[0], _corners[2]-_corners[0]);
        }
    }
}