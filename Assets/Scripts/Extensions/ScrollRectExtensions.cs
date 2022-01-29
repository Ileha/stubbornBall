using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Extensions
{
    public static class ScrollRectExtensions
    {
        public static Tween GetSnapToPositionToBringChildIntoView(this ScrollRect instance, RectTransform child, float duration)
        {
            Canvas.ForceUpdateCanvases();
            var viewPortRect = instance.viewport.GetWorldRect();
            var childRect = child.GetWorldRect();

            Vector2 childDeltaPos = Vector2.zero;

            if (!(childRect.xMin > viewPortRect.xMin && childRect.xMax < viewPortRect.xMax))
            {
                if (Mathf.Abs(viewPortRect.xMin - childRect.xMin) < Mathf.Abs(viewPortRect.xMax - childRect.xMax))
                {
                    childDeltaPos.x = viewPortRect.xMin - childRect.xMin;
                }
                else
                {
                    childDeltaPos.x = viewPortRect.xMax - childRect.xMax;
                }
            }
            
            if (!(childRect.yMin > viewPortRect.yMin && childRect.yMax < viewPortRect.yMax))
            {
                if (Mathf.Abs(viewPortRect.yMin - childRect.yMin) < Mathf.Abs(viewPortRect.yMax - childRect.yMax))
                {
                    childDeltaPos.y = viewPortRect.yMin - childRect.yMin;
                }
                else
                {
                    childDeltaPos.y = viewPortRect.yMax - childRect.yMax;
                }
            }
            
            Vector2 scrollConstrains = new Vector2(instance.horizontal ? 1 : 0, instance.vertical ? 1 : 0);
            childDeltaPos = Vector2.Scale(childDeltaPos, scrollConstrains);

            return DOTween.Sequence()
                .Append(instance.content.DOAnchorPos(instance.content.anchoredPosition + childDeltaPos, duration));
        }
    }
}