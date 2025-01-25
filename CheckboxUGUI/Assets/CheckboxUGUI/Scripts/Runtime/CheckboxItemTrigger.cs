using UnityEngine;
using UnityEngine.Events;

namespace CheckboxUGUI
{
    [AddComponentMenu("UI/CheckboxItemTrigger")]
    [RequireComponent(typeof(RectTransform))]
    public class CheckboxItemTrigger : CheckboxItem, ICheckboxItem
    {
        public bool InvokeOnAwake = true;

        public UnityEvent<bool> OnValueChanged;

        protected override void Awake()
        {
            base.Awake();
            if (InvokeOnAwake)
            {
                OnValueChanged.Invoke(IsOn);
            }
        }

        public override void SetState(bool isOn)
        {
            base.SetState(isOn);
            OnValueChanged.Invoke(IsOn);
        }
    }
}
