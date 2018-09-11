using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

/// <summary>Custom inspector for the SelectableInversion effect.</summary>
[CustomEditor(typeof(SelectableInversion))]
public class SelectableInversionEditor : Editor
{
    /// <summary>The SelectableInversion that this inspector is displaying.</summary>
    private SelectableInversion SelectableInversionInstance;

    /// <summary>If the inverted image should converge to a colour as the inversion approaches 50%.</summary>
    private SerializedProperty UseColoredInversion;
    /// <summary>Uses the color of the inversion camera's render texture as inversion converges to 50%.</summary>
    private SerializedProperty UseMaskColor;
    /// <summary>The colour to converge to as the inversion converges to 50%.</summary>
    private SerializedProperty MidInversionColor;
    /// <summary>The background color that the image effect clears to.</summary>
    private SerializedProperty ClearColor;

    //Initialises inspector
    private void OnEnable()
    {
        //Retrieves the SelectableInversion
        SelectableInversionInstance = (SelectableInversion)target;

        //Caches serialised properties
        UseColoredInversion = serializedObject.FindProperty("UseColoredInversion");
        UseMaskColor = serializedObject.FindProperty("UseMaskColor");
        MidInversionColor = serializedObject.FindProperty("MidInversionColor");
        ClearColor = serializedObject.FindProperty("ClearColor");
    }

    //Draws SelectableInversion inspector
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //Handles colored inversion
        EditorGUILayout.PropertyField(UseColoredInversion, new GUIContent("Use Colored Inversion", "If the inverted image should converge to a colour as the inversion approaches 50%."));
        if (UseColoredInversion.boolValue)
        {
            EditorGUILayout.PropertyField(UseMaskColor, new GUIContent("Use Mask Color", "Uses the color of the inversion camera's render texture as inversion converges to 50%."));
            if (!UseMaskColor.boolValue)
            {
                EditorGUILayout.PropertyField(MidInversionColor, new GUIContent("Mid Inversion Color", "The colour to converge to as the inversion converges to 50%."));
            }
        }

        EditorGUILayout.PropertyField(ClearColor, new GUIContent("Clear Color", "The background color that the image effect clears to."));

        serializedObject.ApplyModifiedProperties();
    }
}
#endif

/// <summary>Image effect for selectable inversion of the camera.</summary>
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Selectable Inversion")]
public class SelectableInversion : MonoBehaviour
{
    /// <summary>If the inverted image should converge to a colour as the inversion approaches 50%.</summary>
    [Tooltip("If the inverted image should converge to a colour as the inversion approaches 50%.")]
    public bool UseColoredInversion;

    /// <summary>Uses the color of the inversion camera's render texture as inversion converges to 50%.</summary>
    [Tooltip("Uses the color of the inversion camera's render texture as inversion converges to 50%.")]
    public bool UseMaskColor;

    /// <summary>The colour to converge to as the inversion converges to 50%.</summary>
    [Tooltip("The colour to converge to as the inversion converges to 50%.")]
    public Color MidInversionColor = new Color(0.5f, 0.5f, 0.5f);

    /// <summary>The background color that the image effect clears to.</summary>
    [Tooltip("The background color that the image effect clears to.")]
    public Color ClearColor = new Color(0, 0, 0);

    /// <summary>The material created for inverting the texture.</summary>
    [HideInInspector]
    [SerializeField]
    private Material InversionMaterial;

    /// <summary>The main camera used to render the scene.</summary>
    [SerializeField]
    [HideInInspector]
    private Camera MainCamera;

    /// <summary>The secondary camera used for creating the inversion mask.</summary>
    [SerializeField]
    [HideInInspector]
    private Camera InversionCamera;

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
            if (!MainCamera) { MainCamera = GetComponent<Camera>(); }
            MainCamera.cullingMask = MainCamera.cullingMask & ~(1 << LayerMask.NameToLayer("SelectableInversion"));
            GetInversionCamera();

            //Copies over aspect ratio and creates the render texture
            InversionCamera.aspect = MainCamera.aspect;
            InversionCamera.targetTexture = new RenderTexture(MainCamera.pixelWidth, MainCamera.pixelHeight, 0, RenderTextureFormat.ARGBHalf);

            //Creates the inversion material
            Shader InversionShader = Shader.Find("Hidden/ImageEffects/SelectableInversion");
            InversionMaterial = new Material(InversionShader);
            InversionMaterial.SetTexture("_Mask", InversionCamera.targetTexture);
        }
    }

    void OnDestroy()
    {
        if (InversionCamera)
        {
            //Destroys the second camera
            DestroyImmediate(InversionCamera.gameObject);
        }
    }

    /// <summary>Gets or creates the camera used to produce the inversion mask.</summary>
    private void GetInversionCamera()
    {
        //Attempts to retrieve the camera if it exists
        if (InversionCamera) { return; }
        Transform InversionTransform = this.transform.Find("_SelectableInversionCamera");
        if (InversionTransform)
        {
            InversionCamera = InversionTransform.GetComponent<Camera>();
            return;
        }

        //Creates a new hidden camera, initialises it and copies over the configuration from main camera
        GameObject CameraObj = new GameObject("_SelectableInversionCamera");
        CameraObj.hideFlags = HideFlags.NotEditable | HideFlags.HideAndDontSave;
        CameraObj.transform.parent = MainCamera.transform;
        CameraObj.transform.localPosition = Vector3.zero;
        CameraObj.transform.localRotation = Quaternion.identity;
        InversionCamera = CameraObj.AddComponent<Camera>();
        InversionCamera.depth = MainCamera.depth;
        InversionCamera.renderingPath = MainCamera.renderingPath;
        InversionCamera.projectionMatrix = MainCamera.projectionMatrix;
        InversionCamera.fieldOfView = MainCamera.fieldOfView;
        InversionCamera.orthographic = MainCamera.orthographic;
        InversionCamera.orthographicSize = MainCamera.orthographicSize;
        InversionCamera.clearFlags = CameraClearFlags.SolidColor;
        InversionCamera.backgroundColor = ClearColor;
        InversionCamera.cullingMask = 1 << LayerMask.NameToLayer("SelectableInversion");
    }

    void Update()
    {
        InversionCamera.backgroundColor = ClearColor;
        if (InversionCamera.targetTexture.height != MainCamera.pixelHeight || InversionCamera.targetTexture.width != MainCamera.pixelWidth)
        {
            InversionCamera.aspect = MainCamera.aspect;
            InversionCamera.targetTexture = new RenderTexture(MainCamera.pixelWidth, MainCamera.pixelHeight, 0, RenderTextureFormat.ARGBHalf);
            InversionCamera.projectionMatrix = MainCamera.projectionMatrix;
            InversionMaterial.SetTexture("_Mask", InversionCamera.targetTexture);
        }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        //Passes data to the shader and performs the inversion
        InversionMaterial.SetInt("_IsColoured", UseColoredInversion ? 1 : 0);
        InversionMaterial.SetInt("_UseMaskCol", UseMaskColor ? 1 : 0);
        InversionMaterial.SetColor("_MidCol", MidInversionColor);
        Graphics.Blit(src, dest, InversionMaterial);
    }
}
