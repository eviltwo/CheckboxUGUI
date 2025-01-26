using UnityEngine;

namespace CheckboxUGUI.RuntimeCheck
{
    public class CheckboxForceSetOnAwake : MonoBehaviour
    {
        [SerializeField]
        private int _index = 0;

        private void Awake()
        {
            var checkbox = GetComponent<Checkbox>();
            checkbox.SetItemState(_index, true, false);
        }
    }
}
