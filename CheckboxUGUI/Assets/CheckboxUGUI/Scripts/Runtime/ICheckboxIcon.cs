using UnityEngine;

namespace CheckboxUGUI
{
    public interface ICheckboxIcon
    {
        Vector2 GetLocalPosition();

        void SetState(bool isOn);

        bool GetState();
    }
}
