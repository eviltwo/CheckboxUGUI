using System;
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
        public enum SelectionBehavior
        {
            First = 0,
            LastSelected = 1,
        }

        [Tooltip("Which item should be selected when the checkbox is selected using keyboard.")]
        public SelectionBehavior ItemSelectionBehavior = SelectionBehavior.LastSelected;

        [Flags]
        public enum Axis
        {
            Horizontal = 1 << 0,
            Vertical = 1 << 1,
        }

        public Axis InnerNavigationAxis = Axis.Horizontal;

        private static List<ICheckboxIcon> _iconBuffer = new List<ICheckboxIcon>();

        private int _selectedIndex;

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            if (ItemSelectionBehavior == SelectionBehavior.First)
            {
                _selectedIndex = 0;
            }
        }

        public override void OnMove(AxisEventData eventData)
        {
            var moveInner = false;
            if (CanInnerMove(eventData.moveDir))
            {
                CollectIcons(_iconBuffer);
                if (_iconBuffer.Count > 0)
                {
                    _selectedIndex = Mathf.Clamp(_selectedIndex, 0, _iconBuffer.Count - 1);
                    var currentIcon = _iconBuffer[_selectedIndex];
                    var nextIndex = FindIcon(eventData.moveVector, currentIcon, _iconBuffer);
                    if (nextIndex >= 0)
                    {
                        _selectedIndex = nextIndex;
                        moveInner = true;
                    }
                }
            }

            if (!moveInner)
            {
                base.OnMove(eventData);
            }
        }

        private bool CanInnerMove(MoveDirection moveDirection)
        {
            if (moveDirection == MoveDirection.Left || moveDirection == MoveDirection.Right)
            {
                return (InnerNavigationAxis & Axis.Horizontal) != 0;
            }
            else
            {
                return (InnerNavigationAxis & Axis.Vertical) != 0;
            }
        }

        private static int FindIcon(Vector2 direction, ICheckboxIcon currentIcon, IReadOnlyList<ICheckboxIcon> icons)
        {
            var currentPos = currentIcon.GetLocalPosition();
            var closestIndex = -1;
            var maxScore = 0f;
            for (int i = 0; i < icons.Count; i++)
            {
                var icon = icons[i];
                if (icon == currentIcon)
                {
                    continue;
                }

                var iconPos = icon.GetLocalPosition();
                var vec = iconPos - currentPos;
                var score = Vector2.Dot(vec, direction) / vec.sqrMagnitude;
                if (score > maxScore)
                {
                    maxScore = score;
                    closestIndex = i;
                }
            }

            return closestIndex;
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
                    _selectedIndex = index;
                    var icon = _iconBuffer[_selectedIndex];
                    icon.SetState(!icon.GetState());
                }
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            CollectIcons(_iconBuffer);
            if (_iconBuffer.Count > 0 && _selectedIndex < _iconBuffer.Count)
            {
                var icon = _iconBuffer[_selectedIndex];
                icon.SetState(!icon.GetState());
            }
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
