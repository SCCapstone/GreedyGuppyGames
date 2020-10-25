// <copyright file="ChromaticAberrationComponent.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

namespace UnityEngine.PostProcessing
{
    public sealed class ChromaticAberrationComponent : PostProcessingComponentRenderTexture<ChromaticAberrationModel>
    {
        private static class Uniforms
        {
            internal static readonly int _ChromaticAberration_Amount = Shader.PropertyToID("_ChromaticAberration_Amount");
            internal static readonly int _ChromaticAberration_Spectrum = Shader.PropertyToID("_ChromaticAberration_Spectrum");
        }

        private Texture2D m_SpectrumLut;

        public override bool active
        {
            get
            {
                return this.model.enabled
                       && this.model.settings.intensity > 0f
                       && !this.context.interrupted;
            }
        }

        public override void OnDisable()
        {
            GraphicsUtils.Destroy(this.m_SpectrumLut);
            this.m_SpectrumLut = null;
        }

        public override void Prepare(Material uberMaterial)
        {
            var settings = this.model.settings;
            var spectralLut = settings.spectralTexture;

            if (spectralLut == null)
            {
                if (this.m_SpectrumLut == null)
                {
                    this.m_SpectrumLut = new Texture2D(3, 1, TextureFormat.RGB24, false)
                    {
                        name = "Chromatic Aberration Spectrum Lookup",
                        filterMode = FilterMode.Bilinear,
                        wrapMode = TextureWrapMode.Clamp,
                        anisoLevel = 0,
                        hideFlags = HideFlags.DontSave
                    };

                    var pixels = new Color[3];
                    pixels[0] = new Color(1f, 0f, 0f);
                    pixels[1] = new Color(0f, 1f, 0f);
                    pixels[2] = new Color(0f, 0f, 1f);
                    this.m_SpectrumLut.SetPixels(pixels);
                    this.m_SpectrumLut.Apply();
                }

                spectralLut = this.m_SpectrumLut;
            }

            uberMaterial.EnableKeyword("CHROMATIC_ABERRATION");
            uberMaterial.SetFloat(Uniforms._ChromaticAberration_Amount, settings.intensity * 0.03f);
            uberMaterial.SetTexture(Uniforms._ChromaticAberration_Spectrum, spectralLut);
        }
    }
}
