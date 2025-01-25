using UnityEngine;

namespace CheckboxUGUI
{
    public interface ICheckboxItem
    {
        Vector2 GetLocalPosition();

        void NotifySelect();

        void NotifyDeselect();

        void SetState(bool isOn);

        bool GetState();

        bool IsDestroyed();
    }
}
