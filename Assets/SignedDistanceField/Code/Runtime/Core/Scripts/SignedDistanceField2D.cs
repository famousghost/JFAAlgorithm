namespace McCore.SignedDistanceField
{
    using UnityEngine;

    public class SignedDistanceField2D : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private Vector2 _TextureSize;
        [SerializeField] private RenderTexture _EarthTexture;
        [SerializeField] private RenderTexture _EarthTextureResult;
        [SerializeField] private RenderTexture _VoronoiTexture;
        [SerializeField] private Texture _DefaultEarthTexture;
        [SerializeField] private ComputeShader _Shader;
        [SerializeField] private bool _DoIt = false;
        [SerializeField] private bool _DoIt2 = false;

        [SerializeField] private float _Offset = 4.0f;
        #endregion Inspector Variables

        #region Public Variables

        #endregion Public Variables

        #region Unity Methods
        private void Start()
        {
            _TextureSize = new Vector2(_DefaultEarthTexture.width, _DefaultEarthTexture.height);
            _EarthTexture = new RenderTexture((int)_TextureSize.x, (int)_TextureSize.y, 24, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear)
            {
                enableRandomWrite = true
            };

            _EarthTextureResult = new RenderTexture((int)_TextureSize.x, (int)_TextureSize.y, 24, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear)
            {
                enableRandomWrite = true
            };

            _VoronoiTexture = new RenderTexture((int)_TextureSize.x, (int)_TextureSize.y, 24, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear)
            {
                enableRandomWrite = true
            };

            _SignedDistanceKernelId = _Shader.FindKernel("DistanceField");
            _FillTextureKernelId = _Shader.FindKernel("FillTexture");
            _VoronoiKernelId = _Shader.FindKernel("VoronoiDiagram");

            _Shader.SetTexture(_FillTextureKernelId, _EarthTextureId, _DefaultEarthTexture);
            _Shader.SetTexture(_FillTextureKernelId, _EarthTetxureResultId, _EarthTexture);
            _Shader.SetVector(_TextureResolutionID, _TextureSize);

            _Shader.Dispatch(_FillTextureKernelId, (int)_TextureSize.x / 8, (int)_TextureSize.y / 8, 1);

        }

        private void Update()
        {

            if (_Offset < 1.0f)
            {
                return;
            }
                _Shader.SetTexture(_SignedDistanceKernelId, _EarthTextureId, _EarthTexture);
                _Shader.SetTexture(_SignedDistanceKernelId, _EarthTetxureResultId, _EarthTextureResult);
                _Shader.SetFloat(_OffsetId, _Offset);
                _Shader.SetVector(_TextureResolutionID, _TextureSize);

                _Shader.Dispatch(_SignedDistanceKernelId, (int)_TextureSize.x / 8, (int)_TextureSize.y / 8, 1);

                _Shader.SetTexture(_FillTextureKernelId, _EarthTextureId, _EarthTextureResult);
                _Shader.SetTexture(_FillTextureKernelId, _EarthTetxureResultId, _EarthTexture);

                _Shader.Dispatch(_FillTextureKernelId, (int)_TextureSize.x / 8, (int)_TextureSize.y / 8, 1);

                _Offset /= 2.0f;

                _Shader.SetVector(_TextureResolutionID, _TextureSize);
                _Shader.SetTexture(_VoronoiKernelId, _EarthTetxureResultId, _EarthTextureResult);
                _Shader.SetTexture(_VoronoiKernelId, _VoronoiTextureId, _VoronoiTexture);

                _Shader.Dispatch(_VoronoiKernelId, (int)_TextureSize.x / 8, (int)_TextureSize.y / 8, 1);



        }
        #endregion Unity Methods

        #region Private Variables
        private static readonly int _EarthTextureId = Shader.PropertyToID("_EarthTexture");
        private static readonly int _EarthTetxureResultId = Shader.PropertyToID("_EarthTextureResult");
        private static readonly int _TextureResolutionID = Shader.PropertyToID("_TextureResolution");
        private static readonly int _VoronoiTextureId = Shader.PropertyToID("_VoronoiTexture");
        private static readonly int _OffsetId = Shader.PropertyToID("_Offset");
        private int _SignedDistanceKernelId;
        private int _FillTextureKernelId;
        private int _VoronoiKernelId;

        #endregion Private Variables

        #region Private Methods

        #endregion  Private Methods
    }
}
