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
        private SerializedProperty useColoredInversion;
        private SerializedProperty useMaskColor;
        private SerializedProperty midInversionColor;
        private SerializedProperty clearColor;

        private void OnEnable()
        {
            useColoredInversion = serializedObject.FindProperty("useColoredInversion");
            useMaskColor = serializedObject.FindProperty("useMaskColor");
            midInversionColor = serializedObject.FindProperty("midInversionColor");
            clearColor = serializedObject.FindProperty("clearColor");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(useColoredInversion, new GUIContent("Use Colored Inversion", "If the inverted image should converge to a colour as the inversion approaches 50%."));
            if (useColoredInversion.boolValue)
            {
                EditorGUILayout.PropertyField(useMaskColor, new GUIContent("Use Mask Color", "Uses the color of the inversion camera's render texture as inversion converges to 50%."));
                if (!useMaskColor.boolValue)
                {
                    EditorGUILayout.PropertyField(midInversionColor, new GUIContent("Mid Inversion Color", "The colour to converge to as the inversion converges to 50%."));
                }
            }

            EditorGUILayout.PropertyField(clearColor, new GUIContent("Clear Color", "The background color that the image effect clears to."));

            serializedObject.ApplyModifiedProperties();
        }
    }
}