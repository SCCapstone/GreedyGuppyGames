// <copyright file="DitheringComponent.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

namespace UnityEngine.PostProcessing
{
    public sealed class DitheringComponent : PostProcessingComponentRenderTexture<DitheringModel>
    {
        private static class Uniforms
        {
            internal static readonly int _DitheringTex = Shader.PropertyToID("_DitheringTex");
            internal static readonly int _DitheringCoords = Shader.PropertyToID("_DitheringCoords");
        }

        public override bool active
        {
            get
            {
                return this.model.enabled
                       && !this.context.interrupted;
            }
        }

        // Holds 64 64x64 Alpha8 textures (256kb total)
        private Texture2D[] noiseTextures;
        private int textureIndex = 0;
        private const int k_TextureCount = 64;

        public override void OnDisable()
        {
            this.noiseTextures = null;
        }

        private void LoadNoiseTextures()
        {
            this.noiseTextures = new Texture2D[k_TextureCount];

            for (int i = 0; i < k_TextureCount; i++)
            {
                this.noiseTextures[i] = Resources.Load<Texture2D>("Bluenoise64/LDR_LLL1_" + i);
            }
        }

        public override void Prepare(Material uberMaterial)
        {
            float rndOffsetX;
            float rndOffsetY;

#if POSTFX_DEBUG_STATIC_DITHERING
            textureIndex = 0;
            rndOffsetX = 0f;
            rndOffsetY = 0f;
#else
            if (++this.textureIndex >= k_TextureCount)
                this.textureIndex = 0;

            rndOffsetX = Random.value;
            rndOffsetY = Random.value;
#endif

            if (this.noiseTextures == null)
                this.LoadNoiseTextures();

            var noiseTex = this.noiseTextures[this.textureIndex];

            uberMaterial.EnableKeyword("DITHERING");
            uberMaterial.SetTexture(Uniforms._DitheringTex, noiseTex);
            uberMaterial.SetVector(Uniforms._DitheringCoords, new Vector4(
                (float)this.context.width / (float)noiseTex.width,
                (float)this.context.height / (float)noiseTex.height,
                rndOffsetX,
                rndOffsetY
            ));
        }
    }
}
