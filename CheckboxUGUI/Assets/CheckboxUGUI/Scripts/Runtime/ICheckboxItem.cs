using UnityEngine;

namespace CheckboxUGUI
{
    public interface ICheckboxItem
    {
        Vector2 GetLocalPosition();

        void SetState(bool isOn);

        bool GetState();
    }
}
