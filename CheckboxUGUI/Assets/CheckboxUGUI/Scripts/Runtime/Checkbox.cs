using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CheckboxUGUI
{
    [AddComponentMenu("UI/Checkbox")]
    [RequireComponent(typeof(RectTransform))]
    public class Checkbox : Selectable, IPointerClickHandler, ISubmitHandler
    {
        private static List<ICheckboxIcon> _iconBuffer = new List<ICheckboxIcon>();

        private int _selectedIndex;

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var rectTransform = transform as RectTransform;
            var canvas = GetCanvas();
            var camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, camera, out var localPoint))
            {
                CollectIcons(_iconBuffer);
                var index = GetClosestIcon(_iconBuffer, localPoint);
                if (index >= 0)
                {
                    var icon = _iconBuffer[index];
                    icon.SetState(!icon.GetState());
                    _selectedIndex = index;
                }
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void CollectIcons(List<ICheckboxIcon> results)
        {
            results.Clear();
            transform.GetComponentsInChildren(_iconBuffer);
        }

        public int GetClosestIcon(IReadOnlyList<ICheckboxIcon> icons, Vector2 position)
        {
            var closestIndex = -1;
            var closestDistance = float.MaxValue;
            for (int i = 0; i < icons.Count; i++)
            {
                var icon = icons[i];
                var distance = (icon.GetLocalPosition() - position).sqrMagnitude;
                if (distance < closestDistance)
                {
                    closestIndex = i;
                    closestDistance = distance;
                }
            }

            return closestIndex;
        }

        private Canvas GetCanvas()
        {
            if (targetGraphic != null)
            {
                return targetGraphic.canvas;
            }

            return GetComponentInParent<Canvas>();
        }
    }
}
