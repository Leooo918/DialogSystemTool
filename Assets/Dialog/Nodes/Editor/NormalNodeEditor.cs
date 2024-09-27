using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Dialog
{
    [CustomEditor(typeof(NormalNodeSO))]
    public class NormalNodeEditor : Editor
    {
        private SerializedProperty _reader;
        private SerializedProperty _content;

        private SerializedProperty _useForVisualNovel;
        private SerializedProperty _useImage;
        private SerializedProperty _useBackground;
        private SerializedProperty _images;
        private SerializedProperty _background;

        private SerializedProperty _nextNode;

        private GUIStyle _textAreaStyle;

        private void OnEnable()
        {
            _reader = serializedObject.FindProperty("reader");
            _content = serializedObject.FindProperty("contents");
            _useImage = serializedObject.FindProperty("useImage");
            _useForVisualNovel = serializedObject.FindProperty("useForVisualNovel");
            _useBackground = serializedObject.FindProperty("useBackground");
            _images = serializedObject.FindProperty("images");
            _background = serializedObject.FindProperty("background");
            _nextNode = serializedObject.FindProperty("nextNode");
            //_canSkip = serializedObject.FindProperty("canSkip");
        }


        public override bool RequiresConstantRepaint()
        {

            return true;
        }


        public override void OnInspectorGUI()
        {
            StyleSetup();
            serializedObject.Update();

            EditorGUILayout.BeginHorizontal("HelpBox");
            {
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.PropertyField(_reader);

                    EditorGUILayout.LabelField("대화 내용");
                    _content.stringValue = EditorGUILayout.TextArea(
                        _content.stringValue,
                        _textAreaStyle,
                        GUILayout.Height(70)
                        );

                    EditorGUILayout.PropertyField(_useForVisualNovel);

                    if(_useForVisualNovel.boolValue)
                    {
                        EditorGUILayout.PropertyField(_useImage);

                        if (_useImage.boolValue)
                        {
                            EditorGUILayout.PropertyField(_images);
                        }

                        EditorGUILayout.PropertyField(_useBackground);

                        if (_useBackground.boolValue)
                        {
                            EditorGUILayout.PropertyField(_background);
                        }
                    }
                        EditorGUILayout.PropertyField(_nextNode);
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();
        }


        private void StyleSetup()
        {
            if (_textAreaStyle == null)
            {
                _textAreaStyle = new GUIStyle(EditorStyles.textArea);
                _textAreaStyle.wordWrap = true;
            }
        }
    }
}
