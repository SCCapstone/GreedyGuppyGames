// <copyright file="MotionBlurComponent.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using UnityEngine.Rendering;

namespace UnityEngine.PostProcessing
{
    using Settings = MotionBlurModel.Settings;

    public sealed class MotionBlurComponent : PostProcessingComponentCommandBuffer<MotionBlurModel>
    {
        private static class Uniforms
        {
            internal static readonly int _VelocityScale = Shader.PropertyToID("_VelocityScale");
            internal static readonly int _MaxBlurRadius = Shader.PropertyToID("_MaxBlurRadius");
            internal static readonly int _RcpMaxBlurRadius = Shader.PropertyToID("_RcpMaxBlurRadius");
            internal static readonly int _VelocityTex = Shader.PropertyToID("_VelocityTex");
            internal static readonly int _MainTex = Shader.PropertyToID("_MainTex");
            internal static readonly int _Tile2RT = Shader.PropertyToID("_Tile2RT");
            internal static readonly int _Tile4RT = Shader.PropertyToID("_Tile4RT");
            internal static readonly int _Tile8RT = Shader.PropertyToID("_Tile8RT");
            internal static readonly int _TileMaxOffs = Shader.PropertyToID("_TileMaxOffs");
            internal static readonly int _TileMaxLoop = Shader.PropertyToID("_TileMaxLoop");
            internal static readonly int _TileVRT = Shader.PropertyToID("_TileVRT");
            internal static readonly int _NeighborMaxTex = Shader.PropertyToID("_NeighborMaxTex");
            internal static readonly int _LoopCount = Shader.PropertyToID("_LoopCount");
            internal static readonly int _TempRT = Shader.PropertyToID("_TempRT");

            internal static readonly int _History1LumaTex = Shader.PropertyToID("_History1LumaTex");
            internal static readonly int _History2LumaTex = Shader.PropertyToID("_History2LumaTex");
            internal static readonly int _History3LumaTex = Shader.PropertyToID("_History3LumaTex");
            internal static readonly int _History4LumaTex = Shader.PropertyToID("_History4LumaTex");

            internal static readonly int _History1ChromaTex = Shader.PropertyToID("_History1ChromaTex");
            internal static readonly int _History2ChromaTex = Shader.PropertyToID("_History2ChromaTex");
            internal static readonly int _History3ChromaTex = Shader.PropertyToID("_History3ChromaTex");
            internal static readonly int _History4ChromaTex = Shader.PropertyToID("_History4ChromaTex");

            internal static readonly int _History1Weight = Shader.PropertyToID("_History1Weight");
            internal static readonly int _History2Weight = Shader.PropertyToID("_History2Weight");
            internal static readonly int _History3Weight = Shader.PropertyToID("_History3Weight");
            internal static readonly int _History4Weight = Shader.PropertyToID("_History4Weight");
        }

        private enum Pass
        {
            VelocitySetup,
            TileMax1,
            TileMax2,
            TileMaxV,
            NeighborMax,
            Reconstruction,
            FrameCompression,
            FrameBlendingChroma,
            FrameBlendingRaw
        }

        public class ReconstructionFilter
        {
            // Texture format for storing 2D vectors.
            private RenderTextureFormat m_VectorRTFormat = RenderTextureFormat.RGHalf;

            // Texture format for storing packed velocity/depth.
            private RenderTextureFormat m_PackedRTFormat = RenderTextureFormat.ARGB2101010;

            public ReconstructionFilter()
            {
                this.CheckTextureFormatSupport();
            }

            private void CheckTextureFormatSupport()
            {
                // If 2:10:10:10 isn't supported, use ARGB32 instead.
                if (!SystemInfo.SupportsRenderTextureFormat(this.m_PackedRTFormat))
                {
                    this.m_PackedRTFormat = RenderTextureFormat.ARGB32;
                }
            }

            public bool IsSupported()
            {
                return SystemInfo.supportsMotionVectors;
            }

            public void ProcessImage(PostProcessingContext context, CommandBuffer cb, ref Settings settings, RenderTargetIdentifier source, RenderTargetIdentifier destination, Material material)
            {
                const float kMaxBlurRadius = 5f;

                // Calculate the maximum blur radius in pixels.
                int maxBlurPixels = (int)(kMaxBlurRadius * context.height / 100);

                // Calculate the TileMax size.
                // It should be a multiple of 8 and larger than maxBlur.
                int tileSize = ((maxBlurPixels - 1) / 8 + 1) * 8;

                // Pass 1 - Velocity/depth packing
                var velocityScale = settings.shutterAngle / 360f;
                cb.SetGlobalFloat(Uniforms._VelocityScale, velocityScale);
                cb.SetGlobalFloat(Uniforms._MaxBlurRadius, maxBlurPixels);
                cb.SetGlobalFloat(Uniforms._RcpMaxBlurRadius, 1f / maxBlurPixels);

                int vbuffer = Uniforms._VelocityTex;
                cb.GetTemporaryRT(vbuffer, context.width, context.height, 0, FilterMode.Point, this.m_PackedRTFormat, RenderTextureReadWrite.Linear);
                cb.Blit((Texture)null, vbuffer, material, (int)Pass.VelocitySetup);

                // Pass 2 - First TileMax filter (1/2 downsize)
                int tile2 = Uniforms._Tile2RT;
                cb.GetTemporaryRT(tile2, context.width / 2, context.height / 2, 0, FilterMode.Point, this.m_VectorRTFormat, RenderTextureReadWrite.Linear);
                cb.SetGlobalTexture(Uniforms._MainTex, vbuffer);
                cb.Blit(vbuffer, tile2, material, (int)Pass.TileMax1);

                // Pass 3 - Second TileMax filter (1/2 downsize)
                int tile4 = Uniforms._Tile4RT;
                cb.GetTemporaryRT(tile4, context.width / 4, context.height / 4, 0, FilterMode.Point, this.m_VectorRTFormat, RenderTextureReadWrite.Linear);
                cb.SetGlobalTexture(Uniforms._MainTex, tile2);
                cb.Blit(tile2, tile4, material, (int)Pass.TileMax2);
                cb.ReleaseTemporaryRT(tile2);

                // Pass 4 - Third TileMax filter (1/2 downsize)
                int tile8 = Uniforms._Tile8RT;
                cb.GetTemporaryRT(tile8, context.width / 8, context.height / 8, 0, FilterMode.Point, this.m_VectorRTFormat, RenderTextureReadWrite.Linear);
                cb.SetGlobalTexture(Uniforms._MainTex, tile4);
                cb.Blit(tile4, tile8, material, (int)Pass.TileMax2);
                cb.ReleaseTemporaryRT(tile4);

                // Pass 5 - Fourth TileMax filter (reduce to tileSize)
                var tileMaxOffs = Vector2.one * (tileSize / 8f - 1f) * -0.5f;
                cb.SetGlobalVector(Uniforms._TileMaxOffs, tileMaxOffs);
                cb.SetGlobalFloat(Uniforms._TileMaxLoop, (int)(tileSize / 8f));

                int tile = Uniforms._TileVRT;
                cb.GetTemporaryRT(tile, context.width / tileSize, context.height / tileSize, 0, FilterMode.Point, this.m_VectorRTFormat, RenderTextureReadWrite.Linear);
                cb.SetGlobalTexture(Uniforms._MainTex, tile8);
                cb.Blit(tile8, tile, material, (int)Pass.TileMaxV);
                cb.ReleaseTemporaryRT(tile8);

                // Pass 6 - NeighborMax filter
                int neighborMax = Uniforms._NeighborMaxTex;
                int neighborMaxWidth = context.width / tileSize;
                int neighborMaxHeight = context.height / tileSize;
                cb.GetTemporaryRT(neighborMax, neighborMaxWidth, neighborMaxHeight, 0, FilterMode.Point, this.m_VectorRTFormat, RenderTextureReadWrite.Linear);
                cb.SetGlobalTexture(Uniforms._MainTex, tile);
                cb.Blit(tile, neighborMax, material, (int)Pass.NeighborMax);
                cb.ReleaseTemporaryRT(tile);

                // Pass 7 - Reconstruction pass
                cb.SetGlobalFloat(Uniforms._LoopCount, Mathf.Clamp(settings.sampleCount / 2, 1, 64));
                cb.SetGlobalTexture(Uniforms._MainTex, source);

                cb.Blit(source, destination, material, (int)Pass.Reconstruction);

                cb.ReleaseTemporaryRT(vbuffer);
                cb.ReleaseTemporaryRT(neighborMax);
            }
        }

        public class FrameBlendingFilter
        {
            private struct Frame
            {
                public RenderTexture lumaTexture;
                public RenderTexture chromaTexture;
                private float m_Time;
                private RenderTargetIdentifier[] m_MRT;

                public float CalculateWeight(float strength, float currentTime)
                {
                    if (Mathf.Approximately(this.m_Time, 0f))
                    {
                        return 0f;
                    }

                    var coeff = Mathf.Lerp(80f, 16f, strength);
                    return Mathf.Exp((this.m_Time - currentTime) * coeff);
                }

                public void Release()
                {
                    if (this.lumaTexture != null)
                    {
                        RenderTexture.ReleaseTemporary(this.lumaTexture);
                    }

                    if (this.chromaTexture != null)
                    {
                        RenderTexture.ReleaseTemporary(this.chromaTexture);
                    }

                    this.lumaTexture = null;
                    this.chromaTexture = null;
                }

                public void MakeRecord(CommandBuffer cb, RenderTargetIdentifier source, int width, int height, Material material)
                {
                    this.Release();

                    this.lumaTexture = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.R8, RenderTextureReadWrite.Linear);
                    this.chromaTexture = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.R8, RenderTextureReadWrite.Linear);

                    this.lumaTexture.filterMode = FilterMode.Point;
                    this.chromaTexture.filterMode = FilterMode.Point;

                    if (this.m_MRT == null)
                    {
                        this.m_MRT = new RenderTargetIdentifier[2];
                    }

                    this.m_MRT[0] = this.lumaTexture;
                    this.m_MRT[1] = this.chromaTexture;

                    cb.SetGlobalTexture(Uniforms._MainTex, source);
                    cb.SetRenderTarget(this.m_MRT, this.lumaTexture);
                    cb.DrawMesh(GraphicsUtils.quad, Matrix4x4.identity, material, 0, (int)Pass.FrameCompression);

                    this.m_Time = Time.time;
                }

                public void MakeRecordRaw(CommandBuffer cb, RenderTargetIdentifier source, int width, int height, RenderTextureFormat format)
                {
                    this.Release();

                    this.lumaTexture = RenderTexture.GetTemporary(width, height, 0, format);
                    this.lumaTexture.filterMode = FilterMode.Point;

                    cb.SetGlobalTexture(Uniforms._MainTex, source);
                    cb.Blit(source, this.lumaTexture);

                    this.m_Time = Time.time;
                }
            }

            private bool m_UseCompression;
            private RenderTextureFormat m_RawTextureFormat;
            private Frame[] m_FrameList;
            private int m_LastFrameCount;

            public FrameBlendingFilter()
            {
                this.m_UseCompression = CheckSupportCompression();
                this.m_RawTextureFormat = GetPreferredRenderTextureFormat();
                this.m_FrameList = new Frame[4];
            }

            public void Dispose()
            {
                foreach (var frame in this.m_FrameList)
                {
                    frame.Release();
                }
            }

            public void PushFrame(CommandBuffer cb, RenderTargetIdentifier source, int width, int height, Material material)
            {
                // Push only when actual update (do nothing while pausing)
                var frameCount = Time.frameCount;
                if (frameCount == this.m_LastFrameCount)
                {
                    return;
                }

                // Update the frame record.
                var index = frameCount % this.m_FrameList.Length;

                if (this.m_UseCompression)
                {
                    this.m_FrameList[index].MakeRecord(cb, source, width, height, material);
                }
                else
                {
                    this.m_FrameList[index].MakeRecordRaw(cb, source, width, height, this.m_RawTextureFormat);
                }

                this.m_LastFrameCount = frameCount;
            }

            public void BlendFrames(CommandBuffer cb, float strength, RenderTargetIdentifier source, RenderTargetIdentifier destination, Material material)
            {
                var t = Time.time;

                var f1 = this.GetFrameRelative(-1);
                var f2 = this.GetFrameRelative(-2);
                var f3 = this.GetFrameRelative(-3);
                var f4 = this.GetFrameRelative(-4);

                cb.SetGlobalTexture(Uniforms._History1LumaTex, f1.lumaTexture);
                cb.SetGlobalTexture(Uniforms._History2LumaTex, f2.lumaTexture);
                cb.SetGlobalTexture(Uniforms._History3LumaTex, f3.lumaTexture);
                cb.SetGlobalTexture(Uniforms._History4LumaTex, f4.lumaTexture);

                cb.SetGlobalTexture(Uniforms._History1ChromaTex, f1.chromaTexture);
                cb.SetGlobalTexture(Uniforms._History2ChromaTex, f2.chromaTexture);
                cb.SetGlobalTexture(Uniforms._History3ChromaTex, f3.chromaTexture);
                cb.SetGlobalTexture(Uniforms._History4ChromaTex, f4.chromaTexture);

                cb.SetGlobalFloat(Uniforms._History1Weight, f1.CalculateWeight(strength, t));
                cb.SetGlobalFloat(Uniforms._History2Weight, f2.CalculateWeight(strength, t));
                cb.SetGlobalFloat(Uniforms._History3Weight, f3.CalculateWeight(strength, t));
                cb.SetGlobalFloat(Uniforms._History4Weight, f4.CalculateWeight(strength, t));

                cb.SetGlobalTexture(Uniforms._MainTex, source);
                cb.Blit(source, destination, material, this.m_UseCompression ? (int)Pass.FrameBlendingChroma : (int)Pass.FrameBlendingRaw);
            }

            // Check if the platform has the capability of compression.
            private static bool CheckSupportCompression()
            {
                return
                    SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.R8) &&
                    SystemInfo.supportedRenderTargetCount > 1;
            }

            // Determine which 16-bit render texture format is available.
            private static RenderTextureFormat GetPreferredRenderTextureFormat()
            {
                RenderTextureFormat[] formats =
                {
                    RenderTextureFormat.RGB565,
                    RenderTextureFormat.ARGB1555,
                    RenderTextureFormat.ARGB4444
                };

                foreach (var f in formats)
                {
                    if (SystemInfo.SupportsRenderTextureFormat(f))
                    {
                        return f;
                    }
                }

                return RenderTextureFormat.Default;
            }

            // Retrieve a frame record with relative indexing.
            // Use a negative index to refer to previous frames.
            private Frame GetFrameRelative(int offset)
            {
                var index = (Time.frameCount + this.m_FrameList.Length + offset) % this.m_FrameList.Length;
                return this.m_FrameList[index];
            }
        }

        private ReconstructionFilter m_ReconstructionFilter;

        public ReconstructionFilter reconstructionFilter
        {
            get
            {
                if (this.m_ReconstructionFilter == null)
                {
                    this.m_ReconstructionFilter = new ReconstructionFilter();
                }

                return this.m_ReconstructionFilter;
            }
        }

        private FrameBlendingFilter m_FrameBlendingFilter;

        public FrameBlendingFilter frameBlendingFilter
        {
            get
            {
                if (this.m_FrameBlendingFilter == null)
                {
                    this.m_FrameBlendingFilter = new FrameBlendingFilter();
                }

                return this.m_FrameBlendingFilter;
            }
        }

        private bool m_FirstFrame = true;

        public override bool active
        {
            get
            {
                var settings = this.model.settings;
                return this.model.enabled
                       && ((settings.shutterAngle > 0f && this.reconstructionFilter.IsSupported()) || settings.frameBlending > 0f)
                       && SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLES2 // No movecs on GLES2 platforms
                       && !this.context.interrupted;
            }
        }

        public override string GetName()
        {
            return "Motion Blur";
        }

        public void ResetHistory()
        {
            if (this.m_FrameBlendingFilter != null)
            {
                this.m_FrameBlendingFilter.Dispose();
            }

            this.m_FrameBlendingFilter = null;
        }

        public override DepthTextureMode GetCameraFlags()
        {
            return DepthTextureMode.Depth | DepthTextureMode.MotionVectors;
        }

        public override CameraEvent GetCameraEvent()
        {
            return CameraEvent.BeforeImageEffects;
        }

        public override void OnEnable()
        {
            this.m_FirstFrame = true;
        }

        public override void PopulateCommandBuffer(CommandBuffer cb)
        {
#if UNITY_EDITOR
            // Don't render motion blur preview when the editor is not playing as it can in some
            // cases results in ugly artifacts (i.e. when resizing the game view).
            if (!Application.isPlaying)
                return;
#endif

            // Skip rendering in the first frame as motion vectors won't be abvailable until the
            // next one
            if (this.m_FirstFrame)
            {
                this.m_FirstFrame = false;
                return;
            }

            var material = this.context.materialFactory.Get("Hidden/Post FX/Motion Blur");
            var blitMaterial = this.context.materialFactory.Get("Hidden/Post FX/Blit");
            var settings = this.model.settings;

            var fbFormat = this.context.isHdr
                ? RenderTextureFormat.DefaultHDR
                : RenderTextureFormat.Default;

            int tempRT = Uniforms._TempRT;
            cb.GetTemporaryRT(tempRT, this.context.width, this.context.height, 0, FilterMode.Point, fbFormat);

            if (settings.shutterAngle > 0f && settings.frameBlending > 0f)
            {
                // Motion blur + frame blending
                this.reconstructionFilter.ProcessImage(this.context, cb, ref settings, BuiltinRenderTextureType.CameraTarget, tempRT, material);
                this.frameBlendingFilter.BlendFrames(cb, settings.frameBlending, tempRT, BuiltinRenderTextureType.CameraTarget, material);
                this.frameBlendingFilter.PushFrame(cb, tempRT, this.context.width, this.context.height, material);
            }
            else if (settings.shutterAngle > 0f)
            {
                // No frame blending
                cb.SetGlobalTexture(Uniforms._MainTex, BuiltinRenderTextureType.CameraTarget);
                cb.Blit(BuiltinRenderTextureType.CameraTarget, tempRT, blitMaterial, 0);
                this.reconstructionFilter.ProcessImage(this.context, cb, ref settings, tempRT, BuiltinRenderTextureType.CameraTarget, material);
            }
            else if (settings.frameBlending > 0f)
            {
                // Frame blending only
                cb.SetGlobalTexture(Uniforms._MainTex, BuiltinRenderTextureType.CameraTarget);
                cb.Blit(BuiltinRenderTextureType.CameraTarget, tempRT, blitMaterial, 0);
                this.frameBlendingFilter.BlendFrames(cb, settings.frameBlending, tempRT, BuiltinRenderTextureType.CameraTarget, material);
                this.frameBlendingFilter.PushFrame(cb, tempRT, this.context.width, this.context.height, material);
            }

            // Cleaning up
            cb.ReleaseTemporaryRT(tempRT);
        }

        public override void OnDisable()
        {
            if (this.m_FrameBlendingFilter != null)
            {
                this.m_FrameBlendingFilter.Dispose();
            }
        }
    }
}
