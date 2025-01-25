using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CheckboxUGUI
{
    [AddComponentMenu("UI/CheckboxIcon")]
    [RequireComponent(typeof(RectTransform))]
    public class CheckboxIcon : UIBehaviour, ICheckboxIcon
    {
        public bool IsOn;

        public Graphic Graphic;

        protected override void Awake()
        {
            UpdateGraphics();
        }

        public Vector2 GetLocalPosition()
        {
            // Return center
            var rectTransform = transform as RectTransform;
            return (Vector2)rectTransform.localPosition + rectTransform.rect.position + rectTransform.rect.size * 0.5f;
        }

        public void SetState(bool isOn)
        {
            IsOn = isOn;
            UpdateGraphics();
        }

        public bool GetState()
        {
            return IsOn;
        }

        public void UpdateGraphics()
        {
            if (Graphic != null)
            {
                Graphic.enabled = IsOn;
            }
        }
    }
}
