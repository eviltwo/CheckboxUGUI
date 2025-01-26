using UnityEngine;

namespace CheckboxUGUI.RuntimeCheck
{
    public class CheckboxEventChecker : MonoBehaviour
    {
        public void OnValueChanged()
        {
            Debug.Log($"{gameObject.name}: OnValueChanged");
        }
    }
}
