// <copyright file="PostProcessingContext.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

namespace UnityEngine.PostProcessing
{
    public class PostProcessingContext
    {
        public PostProcessingProfile profile;
        public Camera camera;

        public MaterialFactory materialFactory;
        public RenderTextureFactory renderTextureFactory;

        public bool interrupted { get; private set; }

        public void Interrupt()
        {
            this.interrupted = true;
        }

        public PostProcessingContext Reset()
        {
            this.profile = null;
            this.camera = null;
            this.materialFactory = null;
            this.renderTextureFactory = null;
            this.interrupted = false;
            return this;
        }

        #region Helpers
        public bool isGBufferAvailable
        {
            get { return this.camera.actualRenderingPath == RenderingPath.DeferredShading; }
        }

        public bool isHdr
        {
            // No UNITY_5_6_OR_NEWER defined in early betas of 5.6
#if UNITY_5_6 || UNITY_5_6_OR_NEWER
            get { return this.camera.allowHDR; }
#else
            get { return camera.hdr; }
#endif
        }

        public int width
        {
            get { return this.camera.pixelWidth; }
        }

        public int height
        {
            get { return this.camera.pixelHeight; }
        }

        public Rect viewport
        {
            get { return this.camera.rect; } // Normalized coordinates
        }
        #endregion
    }
}
