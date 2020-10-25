// <copyright file="DepthOfFieldComponent.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

namespace UnityEngine.PostProcessing
{
    using DebugMode = BuiltinDebugViewsModel.Mode;

    public sealed class DepthOfFieldComponent : PostProcessingComponentRenderTexture<DepthOfFieldModel>
    {
        private static class Uniforms
        {
            internal static readonly int _DepthOfFieldTex = Shader.PropertyToID("_DepthOfFieldTex");
            internal static readonly int _DepthOfFieldCoCTex = Shader.PropertyToID("_DepthOfFieldCoCTex");
            internal static readonly int _Distance = Shader.PropertyToID("_Distance");
            internal static readonly int _LensCoeff = Shader.PropertyToID("_LensCoeff");
            internal static readonly int _MaxCoC = Shader.PropertyToID("_MaxCoC");
            internal static readonly int _RcpMaxCoC = Shader.PropertyToID("_RcpMaxCoC");
            internal static readonly int _RcpAspect = Shader.PropertyToID("_RcpAspect");
            internal static readonly int _MainTex = Shader.PropertyToID("_MainTex");
            internal static readonly int _CoCTex = Shader.PropertyToID("_CoCTex");
            internal static readonly int _TaaParams = Shader.PropertyToID("_TaaParams");
            internal static readonly int _DepthOfFieldParams = Shader.PropertyToID("_DepthOfFieldParams");
        }

        private const string k_ShaderString = "Hidden/Post FX/Depth Of Field";

        public override bool active
        {
            get
            {
                return this.model.enabled
                       && SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf)
                       && !this.context.interrupted;
            }
        }

        public override DepthTextureMode GetCameraFlags()
        {
            return DepthTextureMode.Depth;
        }

        private RenderTexture m_CoCHistory;

        // Height of the 35mm full-frame format (36mm x 24mm)
        private const float k_FilmHeight = 0.024f;

        private float CalculateFocalLength()
        {
            var settings = this.model.settings;

            if (!settings.useCameraFov)
            {
                return settings.focalLength / 1000f;
            }

            float fov = this.context.camera.fieldOfView * Mathf.Deg2Rad;
            return 0.5f * k_FilmHeight / Mathf.Tan(0.5f * fov);
        }

        private float CalculateMaxCoCRadius(int screenHeight)
        {
            // Estimate the allowable maximum radius of CoC from the kernel
            // size (the equation below was empirically derived).
            float radiusInPixels = (float)this.model.settings.kernelSize * 4f + 6f;

            // Applying a 5% limit to the CoC radius to keep the size of
            // TileMax/NeighborMax small enough.
            return Mathf.Min(0.05f, radiusInPixels / screenHeight);
        }

        private bool CheckHistory(int width, int height)
        {
            return this.m_CoCHistory != null && this.m_CoCHistory.IsCreated() &&
                this.m_CoCHistory.width == width && this.m_CoCHistory.height == height;
        }

        private RenderTextureFormat SelectFormat(RenderTextureFormat primary, RenderTextureFormat secondary)
        {
            if (SystemInfo.SupportsRenderTextureFormat(primary))
            {
                return primary;
            }

            if (SystemInfo.SupportsRenderTextureFormat(secondary))
            {
                return secondary;
            }

            return RenderTextureFormat.Default;
        }

        public void Prepare(RenderTexture source, Material uberMaterial, bool antialiasCoC, Vector2 taaJitter, float taaBlending)
        {
            var settings = this.model.settings;
            var colorFormat = RenderTextureFormat.ARGBHalf;
            var cocFormat = this.SelectFormat(RenderTextureFormat.R8, RenderTextureFormat.RHalf);

            // Avoid using R8 on OSX with Metal. #896121, https://goo.gl/MgKqu6
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Metal)
                cocFormat = SelectFormat(RenderTextureFormat.RHalf, RenderTextureFormat.Default);
#endif

            // Material setup
            var f = this.CalculateFocalLength();
            var s1 = Mathf.Max(settings.focusDistance, f);
            var aspect = (float)source.width / source.height;
            var coeff = f * f / (settings.aperture * (s1 - f) * k_FilmHeight * 2);
            var maxCoC = this.CalculateMaxCoCRadius(source.height);

            var material = this.context.materialFactory.Get(k_ShaderString);
            material.SetFloat(Uniforms._Distance, s1);
            material.SetFloat(Uniforms._LensCoeff, coeff);
            material.SetFloat(Uniforms._MaxCoC, maxCoC);
            material.SetFloat(Uniforms._RcpMaxCoC, 1f / maxCoC);
            material.SetFloat(Uniforms._RcpAspect, 1f / aspect);

            // CoC calculation pass
            var rtCoC = this.context.renderTextureFactory.Get(this.context.width, this.context.height, 0, cocFormat);
            Graphics.Blit(null, rtCoC, material, 0);

            if (antialiasCoC)
            {
                // CoC temporal filter pass
                material.SetTexture(Uniforms._CoCTex, rtCoC);

                var blend = this.CheckHistory(this.context.width, this.context.height) ? taaBlending : 0f;
                material.SetVector(Uniforms._TaaParams, new Vector3(taaJitter.x, taaJitter.y, blend));

                var rtFiltered = RenderTexture.GetTemporary(this.context.width, this.context.height, 0, cocFormat);
                Graphics.Blit(this.m_CoCHistory, rtFiltered, material, 1);

                this.context.renderTextureFactory.Release(rtCoC);
                if (this.m_CoCHistory != null)
                {
                    RenderTexture.ReleaseTemporary(this.m_CoCHistory);
                }

                this.m_CoCHistory = rtCoC = rtFiltered;
            }

            // Downsampling and prefiltering pass
            var rt1 = this.context.renderTextureFactory.Get(this.context.width / 2, this.context.height / 2, 0, colorFormat);
            material.SetTexture(Uniforms._CoCTex, rtCoC);
            Graphics.Blit(source, rt1, material, 2);

            // Bokeh simulation pass
            var rt2 = this.context.renderTextureFactory.Get(this.context.width / 2, this.context.height / 2, 0, colorFormat);
            Graphics.Blit(rt1, rt2, material, 3 + (int)settings.kernelSize);

            // Postfilter pass
            Graphics.Blit(rt2, rt1, material, 7);

            // Give the results to the uber shader.
            uberMaterial.SetVector(Uniforms._DepthOfFieldParams, new Vector3(s1, coeff, maxCoC));

            if (this.context.profile.debugViews.IsModeActive(DebugMode.FocusPlane))
            {
                uberMaterial.EnableKeyword("DEPTH_OF_FIELD_COC_VIEW");
                this.context.Interrupt();
            }
            else
            {
                uberMaterial.SetTexture(Uniforms._DepthOfFieldTex, rt1);
                uberMaterial.SetTexture(Uniforms._DepthOfFieldCoCTex, rtCoC);
                uberMaterial.EnableKeyword("DEPTH_OF_FIELD");
            }

            this.context.renderTextureFactory.Release(rt2);
        }

        public override void OnDisable()
        {
            if (this.m_CoCHistory != null)
            {
                RenderTexture.ReleaseTemporary(this.m_CoCHistory);
            }

            this.m_CoCHistory = null;
        }
    }
}
