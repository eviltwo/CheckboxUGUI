using UnityEngine;
using UnityEngine.EventSystems;

namespace CheckboxUGUI
{
    public abstract class CheckboxItem : UIBehaviour, ICheckboxItem
    {
        public bool IsOn;

        public virtual Vector2 GetLocalPosition()
        {
            // Return center
            var rectTransform = transform as RectTransform;
            return (Vector2)rectTransform.localPosition + rectTransform.rect.position + rectTransform.rect.size * 0.5f;
        }

        public abstract void NotifySelect();

        public abstract void NotifyDeselect();

        public virtual void SetState(bool isOn)
        {
            IsOn = isOn;
        }

        public virtual bool GetState()
        {
            return IsOn;
        }
    }
}
