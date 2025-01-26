using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;

namespace CheckboxUGUI.Editor
{
    public static class MenuOptions
    {
        private enum MenuOptionsPriorityOrder
        {
            Checkbox = 3000,
            RadioButtons = 3001
        };

        [MenuItem("GameObject/UI/Checkbox", false, (int)MenuOptionsPriorityOrder.Checkbox)]
        private static void AddCheckbox(MenuCommand menuCommand)
        {
            var parent = UICreateUtility.FindUIPlacement(menuCommand.context as GameObject);
            if (parent == null)
            {
                var errorMessage = "Checkbox: Canvas not found. Please create a Canvas and try again.";
                Debug.LogError(errorMessage);
                EditorUtility.DisplayDialog("Error", errorMessage, "Ok");
                return;
            }

            var element = CreateCheckbox(parent, 3);
            Undo.RegisterFullObjectHierarchyUndo(element, "");
            Undo.SetCurrentGroupName("Create Checkbox");
            Selection.activeGameObject = element;
        }

        public static GameObject CreateCheckbox(GameObject parent, int itemCount)
        {
            var resources = UICreateUtility.GetStandardResources();
            var root = ObjectFactory.CreateGameObject("Checkbox", typeof(Checkbox), typeof(GridLayoutGroup));
            UICreateUtility.SetParentAndAlign(root, parent);
            var rectTransform = root.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(100, 100);
            var checkbox = root.GetComponent<Checkbox>();
            checkbox.transition = Selectable.Transition.None;
            var gridLayoutGroup = root.GetComponent<GridLayoutGroup>();
            gridLayoutGroup.cellSize = new Vector2(20, 20);
            gridLayoutGroup.spacing = new Vector2(5, 5);
            gridLayoutGroup.childAlignment = TextAnchor.MiddleCenter;

            var childColor = new Color(0.9f, 0.9f, 0.9f, 1.0f);
            for (int i = 0; i < itemCount; i++)
            {
                var child = ObjectFactory.CreateGameObject("Item", typeof(Image), typeof(CheckboxItemTrigger));
                Undo.SetTransformParent(child.transform, root.transform, "");
                var image = child.GetComponent<Image>();
                image.color = childColor;
                image.sprite = resources.background;

                var childBg = ObjectFactory.CreateGameObject("Background", typeof(Image));
                Undo.SetTransformParent(childBg.transform, child.transform, "");
                childBg.SetActive(false);
                var rectTransformBg = childBg.GetComponent<RectTransform>();
                rectTransformBg.anchorMin = Vector2.zero;
                rectTransformBg.anchorMax = Vector2.one;
                rectTransformBg.anchoredPosition = Vector2.zero;
                rectTransformBg.sizeDelta = Vector2.zero;
                var imageBg = childBg.GetComponent<Image>();
                imageBg.sprite = resources.background;
                imageBg.raycastTarget = false;

                var childFront = ObjectFactory.CreateGameObject("Checkmark", typeof(Image));
                Undo.SetTransformParent(childFront.transform, child.transform, "");
                var rectTransformFront = childFront.GetComponent<RectTransform>();
                rectTransformFront.anchorMin = Vector2.zero;
                rectTransformFront.anchorMax = Vector2.one;
                rectTransformFront.anchoredPosition = Vector2.zero;
                rectTransformFront.sizeDelta = Vector2.zero;
                var imageFront = childFront.GetComponent<Image>();
                imageFront.sprite = resources.checkmark;
                imageFront.raycastTarget = false;

                var trigger = child.GetComponent<CheckboxItemTrigger>();
                UnityEventTools.AddBoolPersistentListener(trigger.OnSelected, childBg.SetActive, true);
                UnityEventTools.AddBoolPersistentListener(trigger.OnDeselected, childBg.SetActive, false);
                UnityEventTools.AddPersistentListener(trigger.OnValueChanged, childFront.SetActive);
            }

            UICreateUtility.SetLayerRecursively(root, parent.layer);
            return root;
        }

        [MenuItem("GameObject/UI/RadioButtons", false, (int)MenuOptionsPriorityOrder.RadioButtons)]
        private static void AddRadioButtons(MenuCommand menuCommand)
        {
            var parent = UICreateUtility.FindUIPlacement(menuCommand.context as GameObject);
            if (parent == null)
            {
                var errorMessage = "RadioButtons: Canvas not found. Please create a Canvas and try again.";
                Debug.LogError(errorMessage);
                EditorUtility.DisplayDialog("Error", errorMessage, "Ok");
                return;
            }

            var element = CreateRadioButtons(parent, 3);
            Undo.RegisterFullObjectHierarchyUndo(element, "");
            Undo.SetCurrentGroupName("Create RadioButtons");
            Selection.activeGameObject = element;
        }

        public static GameObject CreateRadioButtons(GameObject parent, int itemCount)
        {
            var root = CreateCheckbox(parent, itemCount);
            root.name = "RadioButtons";
            var checkbox = root.GetComponent<Checkbox>();
            checkbox.IsRadioButton = true;
            checkbox.TurnOnWhenSelected = true;
            return root;
        }
    }
}
