using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
namespace QFSW.SIIE.Internal.Editor
{
    /// <summary>Custom inspector for the SelectableInversion effect.</summary>
    [CustomEditor(typeof(SelectableInversion))]
    public class SelectableInversionEditor : UnityEditor.Editor
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
#endif

namespace QFSW.SIIE
{
    /// <summary>Image effect for selectable inversion of the camera.</summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Image Effects/Selectable Inversion")]
    public class SelectableInversion : MonoBehaviour
    {
        /// <summary>If the inverted image should converge to a colour as the inversion approaches 50%.</summary>
        [Tooltip("If the inverted image should converge to a colour as the inversion approaches 50%.")]
        public bool useColoredInversion;

        /// <summary>Uses the color of the inversion camera's render texture as inversion converges to 50%.</summary>
        [Tooltip("Uses the color of the inversion camera's render texture as inversion converges to 50%.")]
        public bool useMaskColor;

        /// <summary>The colour to converge to as the inversion converges to 50%.</summary>
        [Tooltip("The colour to converge to as the inversion converges to 50%.")]
        public Color midInversionColor = new Color(0.5f, 0.5f, 0.5f);

        /// <summary>The background color that the image effect clears to.</summary>
        [Tooltip("The background color that the image effect clears to.")]
        public Color clearColor = new Color(0, 0, 0);

        [SerializeField] [HideInInspector] private Material inversionMaterial;
        [SerializeField] [HideInInspector] private Camera mainCamera;
        [SerializeField] [HideInInspector] private Camera inversionCamera;

        void OnEnable()
        {
            if (LayerMask.NameToLayer("SelectableInversion") < 0)
            {
                //Removes itself if required layer is not present
                Debug.LogError("Please add the layer SelectableInversion then add this image effect!");
                DestroyImmediate(this);
            }
            else
            {
                //Gets the main camera and the inversion camera
                if (!mainCamera) { mainCamera = GetComponent<Camera>(); }
                mainCamera.cullingMask = mainCamera.cullingMask & ~(1 << LayerMask.NameToLayer("SelectableInversion"));
                GetInversionCamera();

                //Copies over aspect ratio and creates the render texture
                inversionCamera.aspect = mainCamera.aspect;
                inversionCamera.targetTexture = new RenderTexture(mainCamera.pixelWidth, mainCamera.pixelHeight, 0, RenderTextureFormat.ARGBHalf);

                //Creates the inversion material
                Shader inversionShader = Shader.Find("Hidden/ImageEffects/SelectableInversion");
                inversionMaterial = new Material(inversionShader);
                inversionMaterial.SetTexture("_Mask", inversionCamera.targetTexture);
            }
        }

        void OnDestroy()
        {
            if (inversionCamera)
            {
                //Destroys the second camera
                DestroyImmediate(inversionCamera.gameObject);
            }
        }

        private void GetInversionCamera()
        {
            //Attempts to retrieve the camera if it exists
            if (inversionCamera) { return; }
            Transform inversionTransform = this.transform.Find("_SelectableInversionCamera");
            if (inversionTransform)
            {
                inversionCamera = inversionTransform.GetComponent<Camera>();
                return;
            }

            //Creates a new hidden camera, initialises it and copies over the configuration from main camera
            GameObject cameraObj = new GameObject("_SelectableInversionCamera");
            cameraObj.hideFlags = HideFlags.NotEditable | HideFlags.HideAndDontSave;
            cameraObj.transform.parent = mainCamera.transform;
            cameraObj.transform.localPosition = Vector3.zero;
            cameraObj.transform.localRotation = Quaternion.identity;
            inversionCamera = cameraObj.AddComponent<Camera>();
            inversionCamera.depth = mainCamera.depth;
            inversionCamera.renderingPath = mainCamera.renderingPath;
            inversionCamera.projectionMatrix = mainCamera.projectionMatrix;
            inversionCamera.fieldOfView = mainCamera.fieldOfView;
            inversionCamera.orthographic = mainCamera.orthographic;
            inversionCamera.orthographicSize = mainCamera.orthographicSize;
            inversionCamera.clearFlags = CameraClearFlags.SolidColor;
            inversionCamera.backgroundColor = clearColor;
            inversionCamera.cullingMask = 1 << LayerMask.NameToLayer("SelectableInversion");
        }

        void Update()
        {
            inversionCamera.backgroundColor = clearColor;
            if (inversionCamera.targetTexture.height != mainCamera.pixelHeight || inversionCamera.targetTexture.width != mainCamera.pixelWidth)
            {
                inversionCamera.aspect = mainCamera.aspect;
                inversionCamera.targetTexture = new RenderTexture(mainCamera.pixelWidth, mainCamera.pixelHeight, 0, RenderTextureFormat.ARGBHalf);
                inversionCamera.projectionMatrix = mainCamera.projectionMatrix;
                inversionMaterial.SetTexture("_Mask", inversionCamera.targetTexture);
            }
        }

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            //Passes data to the shader and performs the inversion
            inversionMaterial.SetInt("_IsColoured", useColoredInversion ? 1 : 0);
            inversionMaterial.SetInt("_UseMaskCol", useMaskColor ? 1 : 0);
            inversionMaterial.SetColor("_MidCol", midInversionColor);
            Graphics.Blit(src, dest, inversionMaterial);
        }
    }
}
