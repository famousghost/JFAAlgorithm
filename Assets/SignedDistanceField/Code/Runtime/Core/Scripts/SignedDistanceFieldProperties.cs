namespace McCore.SignedDistanceField
{
    using UnityEngine;
    using UnityEngine.Rendering;

    public class SignedDistanceFieldProperties : MonoBehaviour
    {
        #region Public Variables
        public RTHandle SignedDistanceFieldPreparationTexture;
        public RTHandle SignedDistanceFieldJFATexture;
        #endregion Public Variables

        #region Unity Methods
        private void Start()
        {
            _Material = GetComponent<MeshRenderer>().sharedMaterial;
        }

        private void Update()
        {
            _Material.SetTexture(_SignedDistanceFieldJFATextureId, SignedDistanceFieldJFATexture);
        }
        #endregion Unity Methods

        #region Private Variables
        private Material _Material;

        private static readonly int _SignedDistanceFieldJFATextureId = Shader.PropertyToID("_SignedDistanceFieldTexture");
        #endregion Private Variables
    }
}
