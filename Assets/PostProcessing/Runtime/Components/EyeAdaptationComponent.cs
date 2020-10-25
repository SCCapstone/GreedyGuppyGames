// <copyright file="EyeAdaptationComponent.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

namespace UnityEngine.PostProcessing
{
    public sealed class EyeAdaptationComponent : PostProcessingComponentRenderTexture<EyeAdaptationModel>
    {
        private static class Uniforms
        {
            internal static readonly int _Params = Shader.PropertyToID("_Params");
            internal static readonly int _Speed = Shader.PropertyToID("_Speed");
            internal static readonly int _ScaleOffsetRes = Shader.PropertyToID("_ScaleOffsetRes");
            internal static readonly int _ExposureCompensation = Shader.PropertyToID("_ExposureCompensation");
            internal static readonly int _AutoExposure = Shader.PropertyToID("_AutoExposure");
            internal static readonly int _DebugWidth = Shader.PropertyToID("_DebugWidth");
        }

        private ComputeShader m_EyeCompute;
        private ComputeBuffer m_HistogramBuffer;
        private readonly RenderTexture[] m_AutoExposurePool = new RenderTexture[2];
        private int m_AutoExposurePingPing;
        private RenderTexture m_CurrentAutoExposure;
        private RenderTexture m_DebugHistogram;
        private static uint[] s_EmptyHistogramBuffer;
        private bool m_FirstFrame = true;

        // Don't forget to update 'EyeAdaptation.cginc' if you change these values !
        private const int k_HistogramBins = 64;
        private const int k_HistogramThreadX = 16;
        private const int k_HistogramThreadY = 16;

        public override bool active
        {
            get
            {
                return this.model.enabled
                       && SystemInfo.supportsComputeShaders
                       && !this.context.interrupted;
            }
        }

        public void ResetHistory()
        {
            this.m_FirstFrame = true;
        }

        public override void OnEnable()
        {
            this.m_FirstFrame = true;
        }

        public override void OnDisable()
        {
            foreach (var rt in this.m_AutoExposurePool)
            {
                GraphicsUtils.Destroy(rt);
            }

            if (this.m_HistogramBuffer != null)
            {
                this.m_HistogramBuffer.Release();
            }

            this.m_HistogramBuffer = null;

            if (this.m_DebugHistogram != null)
            {
                this.m_DebugHistogram.Release();
            }

            this.m_DebugHistogram = null;
        }

        private Vector4 GetHistogramScaleOffsetRes()
        {
            var settings = this.model.settings;
            float diff = settings.logMax - settings.logMin;
            float scale = 1f / diff;
            float offset = -settings.logMin * scale;
            return new Vector4(scale, offset, Mathf.Floor(this.context.width / 2f), Mathf.Floor(this.context.height / 2f));
        }

        public Texture Prepare(RenderTexture source, Material uberMaterial)
        {
            var settings = this.model.settings;

            // Setup compute
            if (this.m_EyeCompute == null)
            {
                this.m_EyeCompute = Resources.Load<ComputeShader>("Shaders/EyeHistogram");
            }

            var material = this.context.materialFactory.Get("Hidden/Post FX/Eye Adaptation");
            material.shaderKeywords = null;

            if (this.m_HistogramBuffer == null)
            {
                this.m_HistogramBuffer = new ComputeBuffer(k_HistogramBins, sizeof(uint));
            }

            if (s_EmptyHistogramBuffer == null)
            {
                s_EmptyHistogramBuffer = new uint[k_HistogramBins];
            }

            // Downscale the framebuffer, we don't need an absolute precision for auto exposure and it
            // helps making it more stable
            var scaleOffsetRes = this.GetHistogramScaleOffsetRes();

            var rt = this.context.renderTextureFactory.Get((int)scaleOffsetRes.z, (int)scaleOffsetRes.w, 0, source.format);
            Graphics.Blit(source, rt);

            if (this.m_AutoExposurePool[0] == null || !this.m_AutoExposurePool[0].IsCreated())
            {
                this.m_AutoExposurePool[0] = new RenderTexture(1, 1, 0, RenderTextureFormat.RFloat);
            }

            if (this.m_AutoExposurePool[1] == null || !this.m_AutoExposurePool[1].IsCreated())
            {
                this.m_AutoExposurePool[1] = new RenderTexture(1, 1, 0, RenderTextureFormat.RFloat);
            }

            // Clears the buffer on every frame as we use it to accumulate luminance values on each frame
            this.m_HistogramBuffer.SetData(s_EmptyHistogramBuffer);

            // Gets a log histogram
            int kernel = this.m_EyeCompute.FindKernel("KEyeHistogram");
            this.m_EyeCompute.SetBuffer(kernel, "_Histogram", this.m_HistogramBuffer);
            this.m_EyeCompute.SetTexture(kernel, "_Source", rt);
            this.m_EyeCompute.SetVector("_ScaleOffsetRes", scaleOffsetRes);
            this.m_EyeCompute.Dispatch(kernel, Mathf.CeilToInt(rt.width / (float)k_HistogramThreadX), Mathf.CeilToInt(rt.height / (float)k_HistogramThreadY), 1);

            // Cleanup
            this.context.renderTextureFactory.Release(rt);

            // Make sure filtering values are correct to avoid apocalyptic consequences
            const float minDelta = 1e-2f;
            settings.highPercent = Mathf.Clamp(settings.highPercent, 1f + minDelta, 99f);
            settings.lowPercent = Mathf.Clamp(settings.lowPercent, 1f, settings.highPercent - minDelta);

            // Compute auto exposure
            material.SetBuffer("_Histogram", this.m_HistogramBuffer); // No (int, buffer) overload for SetBuffer ?
            material.SetVector(Uniforms._Params, new Vector4(settings.lowPercent * 0.01f, settings.highPercent * 0.01f, Mathf.Exp(settings.minLuminance * 0.69314718055994530941723212145818f), Mathf.Exp(settings.maxLuminance * 0.69314718055994530941723212145818f)));
            material.SetVector(Uniforms._Speed, new Vector2(settings.speedDown, settings.speedUp));
            material.SetVector(Uniforms._ScaleOffsetRes, scaleOffsetRes);
            material.SetFloat(Uniforms._ExposureCompensation, settings.keyValue);

            if (settings.dynamicKeyValue)
            {
                material.EnableKeyword("AUTO_KEY_VALUE");
            }

            if (this.m_FirstFrame || !Application.isPlaying)
            {
                // We don't want eye adaptation when not in play mode because the GameView isn't
                // animated, thus making it harder to tweak. Just use the final audo exposure value.
                this.m_CurrentAutoExposure = this.m_AutoExposurePool[0];
                Graphics.Blit(null, this.m_CurrentAutoExposure, material, (int)EyeAdaptationModel.EyeAdaptationType.Fixed);

                // Copy current exposure to the other pingpong target to avoid adapting from black
                Graphics.Blit(this.m_AutoExposurePool[0], this.m_AutoExposurePool[1]);
            }
            else
            {
                int pp = this.m_AutoExposurePingPing;
                var src = this.m_AutoExposurePool[++pp % 2];
                var dst = this.m_AutoExposurePool[++pp % 2];
                Graphics.Blit(src, dst, material, (int)settings.adaptationType);
                this.m_AutoExposurePingPing = ++pp % 2;
                this.m_CurrentAutoExposure = dst;
            }

            // Generate debug histogram
            if (this.context.profile.debugViews.IsModeActive(BuiltinDebugViewsModel.Mode.EyeAdaptation))
            {
                if (this.m_DebugHistogram == null || !this.m_DebugHistogram.IsCreated())
                {
                    this.m_DebugHistogram = new RenderTexture(256, 128, 0, RenderTextureFormat.ARGB32)
                    {
                        filterMode = FilterMode.Point,
                        wrapMode = TextureWrapMode.Clamp
                    };
                }

                material.SetFloat(Uniforms._DebugWidth, this.m_DebugHistogram.width);
                Graphics.Blit(null, this.m_DebugHistogram, material, 2);
            }

            this.m_FirstFrame = false;
            return this.m_CurrentAutoExposure;
        }

        public void OnGUI()
        {
            if (this.m_DebugHistogram == null || !this.m_DebugHistogram.IsCreated())
            {
                return;
            }

            var rect = new Rect(this.context.viewport.x * Screen.width + 8f, 8f, this.m_DebugHistogram.width, this.m_DebugHistogram.height);
            GUI.DrawTexture(rect, this.m_DebugHistogram);
        }
    }
}
