
#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace Dialog.Editor
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }

        private Editor editor;
        private Editor conditionEditor;

        public InspectorView()
        {
        }

        public void UpdateSelection(NodeView node)
        {
            Clear();

            UnityEngine.Object.DestroyImmediate(editor);
            editor = Editor.CreateEditor(node.nodeSO);
            IMGUIContainer container = new IMGUIContainer(() =>
            {
                if (editor.target != null)
                {
                    editor.OnInspectorGUI();
                }
            });
            Add(container);
        }

        public void UpdateSelection(ConditionSO condition)
        {
            Clear();

            if (condition == null) return;

            UnityEngine.Object.DestroyImmediate(conditionEditor);
            conditionEditor = Editor.CreateEditor(condition);
            IMGUIContainer container = new IMGUIContainer(() =>
            {
                if (conditionEditor.target != null)
                {
                    conditionEditor.OnInspectorGUI();
                }
            });
            Add(container);
        }

        public void ClearSelection()
        {
            Clear();
        }
    }
}

#endif