using UnityEngine;
using UnityEngine.Events;

namespace CheckboxUGUI
{
    [AddComponentMenu("UI/CheckboxItemTrigger")]
    [RequireComponent(typeof(RectTransform))]
    public class CheckboxItemTrigger : CheckboxItem, ICheckboxItem
    {
        public UnityEvent OnSelected;

        public UnityEvent OnDeselected;

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

        public override void NotifySelect()
        {
            OnSelected.Invoke();
        }

        public override void NotifyDeselect()
        {
            OnDeselected.Invoke();
        }

        public override void SetState(bool isOn)
        {
            base.SetState(isOn);
            OnValueChanged.Invoke(IsOn);
        }
    }
}
