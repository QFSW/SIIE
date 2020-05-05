using UnityEngine;

namespace QFSW.SIIE
{
    /// <summary>
    /// Image effect for selectable inversion of the camera.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    [AddComponentMenu("Image Effects/Selectable Inversion")]
    public class SelectableInversion : MonoBehaviour
    {
        /// <summary>If the inverted image should converge to a color as the inversion approaches 50%.</summary>
        [Tooltip("If the inverted image should converge to a color as the inversion approaches 50%.")]
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

        private const string InversionLayer = "SelectableInversion";
        private static readonly int ShaderMask = Shader.PropertyToID("_Mask");
        private static readonly int ShaderIsColored = Shader.PropertyToID("_IsColored");
        private static readonly int ShaderUseMaskCol = Shader.PropertyToID("_UseMaskCol");
        private static readonly int ShaderMidCol = Shader.PropertyToID("_MidCol");

        private void OnEnable()
        {
            if (LayerMask.NameToLayer(InversionLayer) < 0)
            {
                //Removes itself if required layer is not present
                Debug.LogError("Please add the layer SelectableInversion then add this image effect!");
                DestroyImmediate(this);
            }
            else
            {
                //Gets the main camera and the inversion camera
                if (!mainCamera) { mainCamera = GetComponent<Camera>(); }
                mainCamera.cullingMask &= ~(1 << LayerMask.NameToLayer(InversionLayer));
                GetInversionCamera();

                //Copies over aspect ratio and creates the render texture
                inversionCamera.aspect = mainCamera.aspect;
                inversionCamera.targetTexture = new RenderTexture(mainCamera.pixelWidth, mainCamera.pixelHeight, 0, RenderTextureFormat.ARGBHalf);

                //Creates the inversion material
                Shader inversionShader = Shader.Find("Hidden/ImageEffects/SelectableInversion");
                inversionMaterial = new Material(inversionShader);
                inversionMaterial.SetTexture(ShaderMask, inversionCamera.targetTexture);
            }
        }

        private void OnDestroy()
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

            //Creates a new hidden camera, initializes it and copies over the configuration from main camera
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
            inversionCamera.cullingMask = 1 << LayerMask.NameToLayer(InversionLayer);
        }

        private void Update()
        {
            inversionCamera.backgroundColor = clearColor;
            if (inversionCamera.targetTexture.height != mainCamera.pixelHeight || inversionCamera.targetTexture.width != mainCamera.pixelWidth)
            {
                inversionCamera.aspect = mainCamera.aspect;
                inversionCamera.targetTexture = new RenderTexture(mainCamera.pixelWidth, mainCamera.pixelHeight, 0, RenderTextureFormat.ARGBHalf);
                inversionCamera.projectionMatrix = mainCamera.projectionMatrix;
                inversionMaterial.SetTexture(ShaderMask, inversionCamera.targetTexture);
            }
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            //Passes data to the shader and performs the inversion
            inversionMaterial.SetInt(ShaderIsColored, useColoredInversion ? 1 : 0);
            inversionMaterial.SetInt(ShaderUseMaskCol, useMaskColor ? 1 : 0);
            inversionMaterial.SetColor(ShaderMidCol, midInversionColor);
            Graphics.Blit(src, dest, inversionMaterial);
        }
    }
}
