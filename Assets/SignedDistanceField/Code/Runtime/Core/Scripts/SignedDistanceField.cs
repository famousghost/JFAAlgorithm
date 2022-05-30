namespace McCore.SignedDistanceField
{
    using UnityEngine;
    using UnityEngine.Rendering.HighDefinition;
    using UnityEngine.Rendering;
    using UnityEngine.Experimental.Rendering;

    public sealed class SignedDistanceField : CustomPass
    {
        #region Inspector Variables
        [SerializeField] private int _TextureSize = 1024;
        [SerializeField] private LayerMask _RenderersMask;
        [SerializeField] private SignedDistanceFieldProperties _SignedDistanceFieldProperties;

        //Debug Only
        [SerializeField] private RenderTexture _SignedDistanceFieldJFARenderTexture;
        [SerializeField] private RenderTexture _SignedDistanceFieldRenderTexture;
        #endregion Inspector Variables

        #region Private Variables
        private Shader _SignedDistanceFieldShader;
        private Shader _SignedDistanceFieldJFAShader;
        private Material _SignedDistanceFieldMaterial;
        private Material _SignedDistanceFieldJFAMaterial;

        private static readonly int _SignedDistanceFieldPreparationTextureId = Shader.PropertyToID("_SignedDistanceFieldPreparationTexture");
        private static readonly int _OffsetId = Shader.PropertyToID("_Offset");
        #endregion  Private Variables

        #region Private Methods
        protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
        {
            _SignedDistanceFieldJFARenderTexture = new RenderTexture(_TextureSize, _TextureSize, 24, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear)
            {
                enableRandomWrite = true
            };

            _SignedDistanceFieldProperties.SignedDistanceFieldJFATexture = RTHandles.Alloc(_TextureSize, _TextureSize, TextureXR.slices,
                                                             colorFormat: GraphicsFormat.R32G32B32A32_SFloat,
                                                              useDynamicScale: true, name: "SignedDistanceFieldJFABuffer");

            _SignedDistanceFieldProperties.SignedDistanceFieldPreparationTexture = RTHandles.Alloc(_TextureSize, _TextureSize, TextureXR.slices,
                                       colorFormat: GraphicsFormat.R32G32B32A32_SFloat,
                                       useDynamicScale: true, name: "SignedDistanceFieldPreparationBuffer");

            _SignedDistanceFieldShader = Shader.Find("McCore/SignedDistanceField");
            _SignedDistanceFieldJFAShader = Shader.Find("McCore/SignedDistanceFieldJFA");


            _SignedDistanceFieldMaterial = CoreUtils.CreateEngineMaterial(_SignedDistanceFieldShader);

            _SignedDistanceFieldJFAMaterial = CoreUtils.CreateEngineMaterial(_SignedDistanceFieldJFAShader);


        }

        protected override void Execute(CustomPassContext ctx)
        {
            CustomPassUtils.RenderFromCamera(ctx,
                                             ctx.hdCamera.camera,
                                             _SignedDistanceFieldProperties.SignedDistanceFieldPreparationTexture,
                                             null,
                                             ClearFlag.All,
                                             _RenderersMask,
                                             RenderQueueType.All,
                                             _SignedDistanceFieldMaterial);



            float k = 256.0f;

            while(k != 1.0f)
            {
                _SignedDistanceFieldJFAMaterial.SetTexture(_SignedDistanceFieldPreparationTextureId,
                                                           _SignedDistanceFieldProperties.SignedDistanceFieldPreparationTexture);
                _SignedDistanceFieldJFAMaterial.SetFloat(_OffsetId, k);

                CustomPassUtils.RenderFromCamera(ctx,
                                                 ctx.hdCamera.camera,
                                                 _SignedDistanceFieldProperties.SignedDistanceFieldJFATexture,
                                                 null,
                                                 ClearFlag.All,
                                                 _RenderersMask,
                                                 RenderQueueType.All,
                                                 _SignedDistanceFieldJFAMaterial);

                ctx.cmd.CopyTexture(_SignedDistanceFieldProperties.SignedDistanceFieldJFATexture, _SignedDistanceFieldProperties.SignedDistanceFieldPreparationTexture);

                k /= 2.0f;
            }

            _SignedDistanceFieldJFAMaterial.SetTexture(_SignedDistanceFieldPreparationTextureId,
                                           _SignedDistanceFieldProperties.SignedDistanceFieldPreparationTexture);
            _SignedDistanceFieldJFAMaterial.SetFloat(_OffsetId, k);

            CustomPassUtils.RenderFromCamera(ctx,
                                             ctx.hdCamera.camera,
                                             _SignedDistanceFieldProperties.SignedDistanceFieldJFATexture,
                                             null,
                                             ClearFlag.All,
                                             _RenderersMask,
                                             RenderQueueType.All,
                                             _SignedDistanceFieldJFAMaterial);


            _SignedDistanceFieldJFARenderTexture = _SignedDistanceFieldProperties.SignedDistanceFieldJFATexture.rt;
            _SignedDistanceFieldRenderTexture = _SignedDistanceFieldProperties.SignedDistanceFieldPreparationTexture.rt;

        }

        protected override void Cleanup()
        {
            _SignedDistanceFieldProperties.SignedDistanceFieldJFATexture.Release();
            _SignedDistanceFieldProperties.SignedDistanceFieldPreparationTexture.Release();
            CoreUtils.Destroy(_SignedDistanceFieldMaterial);
            CoreUtils.Destroy(_SignedDistanceFieldJFAMaterial);
        }
        #endregion  Private Methods
    }
}