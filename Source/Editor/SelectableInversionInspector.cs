using UnityEditor;
using UnityEngine;

namespace QFSW.SIIE.Editor
{
    /// <summary>
    /// Custom inspector for the SelectableInversion effect.
    /// </summary>
    [CustomEditor(typeof(SelectableInversion))]
    public class SelectableInversionInspector : UnityEditor.Editor
    {
        private SerializedProperty _useColoredInversion;
        private SerializedProperty _useMaskColor;
        private SerializedProperty _midInversionColor;
        private SerializedProperty _clearColor;

        private void OnEnable()
        {
            _useColoredInversion = serializedObject.FindProperty("useColoredInversion");
            _useMaskColor = serializedObject.FindProperty("useMaskColor");
            _midInversionColor = serializedObject.FindProperty("midInversionColor");
            _clearColor = serializedObject.FindProperty("clearColor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_useColoredInversion, new GUIContent("Use Colored Inversion", "If the inverted image should converge to a color as the inversion approaches 50%."));
            if (_useColoredInversion.boolValue)
            {
                EditorGUILayout.PropertyField(_useMaskColor, new GUIContent("Use Mask Color", "Uses the color of the inversion camera's render texture as inversion converges to 50%."));
                if (!_useMaskColor.boolValue)
                {
                    EditorGUILayout.PropertyField(_midInversionColor, new GUIContent("Mid Inversion Color", "The color to converge to as the inversion converges to 50%."));
                }
            }

            EditorGUILayout.PropertyField(_clearColor, new GUIContent("Clear Color", "The background color that the image effect clears to."));

            serializedObject.ApplyModifiedProperties();
        }
    }
}