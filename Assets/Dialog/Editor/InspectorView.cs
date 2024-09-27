using UnityEditor;
using UnityEngine.UIElements;

namespace Dialog
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits { }

        Editor editor;

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
    }
}
