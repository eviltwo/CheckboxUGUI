using UnityEngine;

namespace CheckboxUGUI.RuntimeCheck
{
    public class CheckboxEventChecker : MonoBehaviour
    {
        private int _logCount;

        public void OnValueChanged()
        {
            Debug.Log($"{gameObject.name}: OnValueChanged, count={_logCount++}");
        }
    }
}
