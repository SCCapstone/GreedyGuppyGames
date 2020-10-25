// <copyright file="GrainComponent.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

namespace UnityEngine.PostProcessing
{
    public sealed class GrainComponent : PostProcessingComponentRenderTexture<GrainModel>
    {
        private static class Uniforms
        {
            internal static readonly int _Grain_Params1 = Shader.PropertyToID("_Grain_Params1");
            internal static readonly int _Grain_Params2 = Shader.PropertyToID("_Grain_Params2");
            internal static readonly int _GrainTex = Shader.PropertyToID("_GrainTex");
            internal static readonly int _Phase = Shader.PropertyToID("_Phase");
        }

        public override bool active
        {
            get
            {
                return this.model.enabled
                       && this.model.settings.intensity > 0f
                       && SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf)
                       && !this.context.interrupted;
            }
        }

        private RenderTexture m_GrainLookupRT;

        public override void OnDisable()
        {
            GraphicsUtils.Destroy(this.m_GrainLookupRT);
            this.m_GrainLookupRT = null;
        }

        public override void Prepare(Material uberMaterial)
        {
            var settings = this.model.settings;

            uberMaterial.EnableKeyword("GRAIN");

            float rndOffsetX;
            float rndOffsetY;

#if POSTFX_DEBUG_STATIC_GRAIN
            // Chosen by a fair dice roll
            float time = 4f;
            rndOffsetX = 0f;
            rndOffsetY = 0f;
#else
            float time = Time.realtimeSinceStartup;
            rndOffsetX = Random.value;
            rndOffsetY = Random.value;
#endif

            // Generate the grain lut for the current frame first
            if (this.m_GrainLookupRT == null || !this.m_GrainLookupRT.IsCreated())
            {
                GraphicsUtils.Destroy(this.m_GrainLookupRT);

                this.m_GrainLookupRT = new RenderTexture(192, 192, 0, RenderTextureFormat.ARGBHalf)
                {
                    filterMode = FilterMode.Bilinear,
                    wrapMode = TextureWrapMode.Repeat,
                    anisoLevel = 0,
                    name = "Grain Lookup Texture"
                };

                this.m_GrainLookupRT.Create();
            }

            var grainMaterial = this.context.materialFactory.Get("Hidden/Post FX/Grain Generator");
            grainMaterial.SetFloat(Uniforms._Phase, time / 20f);

            Graphics.Blit((Texture)null, this.m_GrainLookupRT, grainMaterial, settings.colored ? 1 : 0);

            // Send everything to the uber shader
            uberMaterial.SetTexture(Uniforms._GrainTex, this.m_GrainLookupRT);
            uberMaterial.SetVector(Uniforms._Grain_Params1, new Vector2(settings.luminanceContribution, settings.intensity * 20f));
            uberMaterial.SetVector(Uniforms._Grain_Params2, new Vector4((float)this.context.width / (float)this.m_GrainLookupRT.width / settings.size, (float)this.context.height / (float)this.m_GrainLookupRT.height / settings.size, rndOffsetX, rndOffsetY));
        }
    }
}
