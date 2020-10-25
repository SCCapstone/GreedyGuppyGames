// <copyright file="TaaComponent.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System;

namespace UnityEngine.PostProcessing
{
    public sealed class TaaComponent : PostProcessingComponentRenderTexture<AntialiasingModel>
    {
        private static class Uniforms
        {
            internal static int _Jitter = Shader.PropertyToID("_Jitter");
            internal static int _SharpenParameters = Shader.PropertyToID("_SharpenParameters");
            internal static int _FinalBlendParameters = Shader.PropertyToID("_FinalBlendParameters");
            internal static int _HistoryTex = Shader.PropertyToID("_HistoryTex");
            internal static int _MainTex = Shader.PropertyToID("_MainTex");
        }

        private const string k_ShaderString = "Hidden/Post FX/Temporal Anti-aliasing";
        private const int k_SampleCount = 8;
        private readonly RenderBuffer[] m_MRT = new RenderBuffer[2];
        private int m_SampleIndex = 0;
        private bool m_ResetHistory = true;
        private RenderTexture m_HistoryTexture;

        public override bool active
        {
            get
            {
                return this.model.enabled
                       && this.model.settings.method == AntialiasingModel.Method.Taa
                       && SystemInfo.supportsMotionVectors
                       && SystemInfo.supportedRenderTargetCount >= 2
                       && !this.context.interrupted;
            }
        }

        public override DepthTextureMode GetCameraFlags()
        {
            return DepthTextureMode.Depth | DepthTextureMode.MotionVectors;
        }

        public Vector2 jitterVector { get; private set; }

        public void ResetHistory()
        {
            this.m_ResetHistory = true;
        }

        public void SetProjectionMatrix(Func<Vector2, Matrix4x4> jitteredFunc)
        {
            var settings = this.model.settings.taaSettings;

            var jitter = this.GenerateRandomOffset();
            jitter *= settings.jitterSpread;

            this.context.camera.nonJitteredProjectionMatrix = this.context.camera.projectionMatrix;

            if (jitteredFunc != null)
            {
                this.context.camera.projectionMatrix = jitteredFunc(jitter);
            }
            else
            {
                this.context.camera.projectionMatrix = this.context.camera.orthographic
                    ? this.GetOrthographicProjectionMatrix(jitter)
                    : this.GetPerspectiveProjectionMatrix(jitter);
            }

#if UNITY_5_5_OR_NEWER
            this.context.camera.useJitteredProjectionMatrixForTransparentRendering = false;
#endif

            jitter.x /= this.context.width;
            jitter.y /= this.context.height;

            var material = this.context.materialFactory.Get(k_ShaderString);
            material.SetVector(Uniforms._Jitter, jitter);

            this.jitterVector = jitter;
        }

        public void Render(RenderTexture source, RenderTexture destination)
        {
            var material = this.context.materialFactory.Get(k_ShaderString);
            material.shaderKeywords = null;

            var settings = this.model.settings.taaSettings;

            if (this.m_ResetHistory || this.m_HistoryTexture == null || this.m_HistoryTexture.width != source.width || this.m_HistoryTexture.height != source.height)
            {
                if (this.m_HistoryTexture)
                {
                    RenderTexture.ReleaseTemporary(this.m_HistoryTexture);
                }

                this.m_HistoryTexture = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
                this.m_HistoryTexture.name = "TAA History";

                Graphics.Blit(source, this.m_HistoryTexture, material, 2);
            }

            const float kMotionAmplification = 100f * 60f;
            material.SetVector(Uniforms._SharpenParameters, new Vector4(settings.sharpen, 0f, 0f, 0f));
            material.SetVector(Uniforms._FinalBlendParameters, new Vector4(settings.stationaryBlending, settings.motionBlending, kMotionAmplification, 0f));
            material.SetTexture(Uniforms._MainTex, source);
            material.SetTexture(Uniforms._HistoryTex, this.m_HistoryTexture);

            var tempHistory = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
            tempHistory.name = "TAA History";

            this.m_MRT[0] = destination.colorBuffer;
            this.m_MRT[1] = tempHistory.colorBuffer;

            Graphics.SetRenderTarget(this.m_MRT, source.depthBuffer);
            GraphicsUtils.Blit(material, this.context.camera.orthographic ? 1 : 0);

            RenderTexture.ReleaseTemporary(this.m_HistoryTexture);
            this.m_HistoryTexture = tempHistory;

            this.m_ResetHistory = false;
        }

        private float GetHaltonValue(int index, int radix)
        {
            float result = 0f;
            float fraction = 1f / (float)radix;

            while (index > 0)
            {
                result += (float)(index % radix) * fraction;

                index /= radix;
                fraction /= (float)radix;
            }

            return result;
        }

        private Vector2 GenerateRandomOffset()
        {
            var offset = new Vector2(
                    this.GetHaltonValue(this.m_SampleIndex & 1023, 2),
                    this.GetHaltonValue(this.m_SampleIndex & 1023, 3));

            if (++this.m_SampleIndex >= k_SampleCount)
            {
                this.m_SampleIndex = 0;
            }

            return offset;
        }

        // Adapted heavily from PlayDead's TAA code
        // https://github.com/playdeadgames/temporal/blob/master/Assets/Scripts/Extensions.cs
        private Matrix4x4 GetPerspectiveProjectionMatrix(Vector2 offset)
        {
            float vertical = Mathf.Tan(0.5f * Mathf.Deg2Rad * this.context.camera.fieldOfView);
            float horizontal = vertical * this.context.camera.aspect;

            offset.x *= horizontal / (0.5f * this.context.width);
            offset.y *= vertical / (0.5f * this.context.height);

            float left = (offset.x - horizontal) * this.context.camera.nearClipPlane;
            float right = (offset.x + horizontal) * this.context.camera.nearClipPlane;
            float top = (offset.y + vertical) * this.context.camera.nearClipPlane;
            float bottom = (offset.y - vertical) * this.context.camera.nearClipPlane;

            var matrix = new Matrix4x4();

            matrix[0, 0] = (2f * this.context.camera.nearClipPlane) / (right - left);
            matrix[0, 1] = 0f;
            matrix[0, 2] = (right + left) / (right - left);
            matrix[0, 3] = 0f;

            matrix[1, 0] = 0f;
            matrix[1, 1] = (2f * this.context.camera.nearClipPlane) / (top - bottom);
            matrix[1, 2] = (top + bottom) / (top - bottom);
            matrix[1, 3] = 0f;

            matrix[2, 0] = 0f;
            matrix[2, 1] = 0f;
            matrix[2, 2] = -(this.context.camera.farClipPlane + this.context.camera.nearClipPlane) / (this.context.camera.farClipPlane - this.context.camera.nearClipPlane);
            matrix[2, 3] = -(2f * this.context.camera.farClipPlane * this.context.camera.nearClipPlane) / (this.context.camera.farClipPlane - this.context.camera.nearClipPlane);

            matrix[3, 0] = 0f;
            matrix[3, 1] = 0f;
            matrix[3, 2] = -1f;
            matrix[3, 3] = 0f;

            return matrix;
        }

        private Matrix4x4 GetOrthographicProjectionMatrix(Vector2 offset)
        {
            float vertical = this.context.camera.orthographicSize;
            float horizontal = vertical * this.context.camera.aspect;

            offset.x *= horizontal / (0.5f * this.context.width);
            offset.y *= vertical / (0.5f * this.context.height);

            float left = offset.x - horizontal;
            float right = offset.x + horizontal;
            float top = offset.y + vertical;
            float bottom = offset.y - vertical;

            return Matrix4x4.Ortho(left, right, bottom, top, this.context.camera.nearClipPlane, this.context.camera.farClipPlane);
        }

        public override void OnDisable()
        {
            if (this.m_HistoryTexture != null)
            {
                RenderTexture.ReleaseTemporary(this.m_HistoryTexture);
            }

            this.m_HistoryTexture = null;
            this.m_SampleIndex = 0;
            this.ResetHistory();
        }
    }
}
