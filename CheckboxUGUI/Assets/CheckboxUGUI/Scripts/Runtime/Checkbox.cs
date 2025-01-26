using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CheckboxUGUI
{
    [AddComponentMenu("UI/Checkbox")]
    [RequireComponent(typeof(RectTransform))]
    public class Checkbox : Selectable, ISubmitHandler
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

        public bool IsRadioButton = false;

        public bool TurnOnWhenSelected = false;

        public UnityEvent OnValueChanged;

        private static List<ICheckboxItem> _itemBuffer = new List<ICheckboxItem>();

        private int _selectedIndex;

        private ICheckboxItem _selectedItem;

        protected override void Awake()
        {
            base.Awake();

            if (!Application.isPlaying)
            {
                return;
            }

            if (IsRadioButton)
            {
                CollectItems(_itemBuffer);
                if (_itemBuffer.Count > 0 && !_itemBuffer.Any(v => v.GetState()))
                {
                    _itemBuffer[0].SetState(true);
                }
            }
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            if (ItemSelectionBehavior == SelectionBehavior.First)
            {
                _selectedIndex = 0;
            }

            CollectItems(_itemBuffer);
            if (_itemBuffer.Count > 0)
            {
                _selectedIndex = Mathf.Clamp(_selectedIndex, 0, _itemBuffer.Count - 1);
                var nextItem = _itemBuffer[_selectedIndex];
                NotifyChangeSelection(_selectedItem, nextItem);
                _selectedItem = nextItem;
                if (TurnOnWhenSelected)
                {
                    _selectedItem.SetState(true);
                    if (IsRadioButton)
                    {
                        SetOffOthers(_selectedItem, _itemBuffer);
                    }
                    OnValueChanged.Invoke();
                }
            }
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            NotifyChangeSelection(_selectedItem, null);
            _selectedItem = null;
        }

        public override void OnMove(AxisEventData eventData)
        {
            var moveInner = false;
            if (CanInnerMove(eventData.moveDir))
            {
                CollectItems(_itemBuffer);
                if (_itemBuffer.Count > 0)
                {
                    _selectedIndex = Mathf.Clamp(_selectedIndex, 0, _itemBuffer.Count - 1);
                    var nextIndex = FindItem(eventData.moveVector, _itemBuffer[_selectedIndex], _itemBuffer);
                    if (nextIndex >= 0)
                    {
                        _selectedIndex = nextIndex;
                        moveInner = true;
                        var nextItem = _itemBuffer[_selectedIndex];
                        NotifyChangeSelection(_selectedItem, nextItem);
                        _selectedItem = nextItem;
                        if (TurnOnWhenSelected)
                        {
                            _selectedItem.SetState(true);
                            if (IsRadioButton)
                            {
                                SetOffOthers(_selectedItem, _itemBuffer);
                            }
                            OnValueChanged.Invoke();
                        }
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

        private static int FindItem(Vector2 direction, ICheckboxItem currentItem, IReadOnlyList<ICheckboxItem> items)
        {
            var currentPos = currentItem.GetLocalPosition();
            var closestIndex = -1;
            var maxScore = 0f;
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (item == currentItem)
                {
                    continue;
                }

                var itemPos = item.GetLocalPosition();
                var vec = itemPos - currentPos;
                var score = Vector2.Dot(vec, direction) / vec.sqrMagnitude;
                if (score > maxScore)
                {
                    maxScore = score;
                    closestIndex = i;
                }
            }

            return closestIndex;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            var rectTransform = transform as RectTransform;
            var canvas = GetCanvas();
            var camera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, camera, out var localPoint))
            {
                CollectItems(_itemBuffer);
                var index = GetClosestItem(_itemBuffer, localPoint);
                if (index >= 0)
                {
                    _selectedIndex = index;
                    var item = _itemBuffer[_selectedIndex];
                    NotifyChangeSelection(_selectedItem, item);
                    _selectedItem = item;
                    if (IsRadioButton)
                    {
                        item.SetState(true);
                        SetOffOthers(item, _itemBuffer);
                    }
                    else
                    {
                        item.SetState(!item.GetState());
                    }
                    OnValueChanged.Invoke();
                }
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            CollectItems(_itemBuffer);
            if (_itemBuffer.Count > 0 && _selectedIndex < _itemBuffer.Count)
            {
                var item = _itemBuffer[_selectedIndex];
                if (IsRadioButton)
                {
                    item.SetState(true);
                    SetOffOthers(item, _itemBuffer);
                }
                else
                {
                    item.SetState(!item.GetState());
                }
                OnValueChanged.Invoke();
            }
        }

        public void CollectItems(List<ICheckboxItem> results)
        {
            results.Clear();
            transform.GetComponentsInChildren(_itemBuffer);
        }

        public int GetClosestItem(IReadOnlyList<ICheckboxItem> items, Vector2 position)
        {
            var closestIndex = -1;
            var closestDistance = float.MaxValue;
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var distance = (item.GetLocalPosition() - position).sqrMagnitude;
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

        private void NotifyChangeSelection(ICheckboxItem prev, ICheckboxItem next)
        {
            if (prev == next)
            {
                return;
            }

            if (prev != null && !prev.IsDestroyed())
            {
                prev.NotifyDeselect();
            }

            if (next != null && !next.IsDestroyed())
            {
                next.NotifySelect();
            }
        }

        public void SetOffOthers(ICheckboxItem item, IReadOnlyList<ICheckboxItem> items)
        {
            foreach (var i in items)
            {
                if (i != item)
                {
                    i.SetState(false);
                }
            }
        }

        public int GetTurnedOnIndex()
        {
            CollectItems(_itemBuffer);
            return _itemBuffer.FindIndex(v => v.GetState());
        }
    }
}
