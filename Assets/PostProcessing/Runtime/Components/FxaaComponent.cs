// <copyright file="FxaaComponent.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

namespace UnityEngine.PostProcessing
{
    public sealed class FxaaComponent : PostProcessingComponentRenderTexture<AntialiasingModel>
    {
        private static class Uniforms
        {
            internal static readonly int _QualitySettings = Shader.PropertyToID("_QualitySettings");
            internal static readonly int _ConsoleSettings = Shader.PropertyToID("_ConsoleSettings");
        }

        public override bool active
        {
            get
            {
                return this.model.enabled
                       && this.model.settings.method == AntialiasingModel.Method.Fxaa
                       && !this.context.interrupted;
            }
        }

        public void Render(RenderTexture source, RenderTexture destination)
        {
            var settings = this.model.settings.fxaaSettings;
            var material = this.context.materialFactory.Get("Hidden/Post FX/FXAA");
            var qualitySettings = AntialiasingModel.FxaaQualitySettings.presets[(int)settings.preset];
            var consoleSettings = AntialiasingModel.FxaaConsoleSettings.presets[(int)settings.preset];

            material.SetVector(Uniforms._QualitySettings,
                new Vector3(
                    qualitySettings.subpixelAliasingRemovalAmount,
                    qualitySettings.edgeDetectionThreshold,
                    qualitySettings.minimumRequiredLuminance
                    )
                );

            material.SetVector(Uniforms._ConsoleSettings,
                new Vector4(
                    consoleSettings.subpixelSpreadAmount,
                    consoleSettings.edgeSharpnessAmount,
                    consoleSettings.edgeDetectionThreshold,
                    consoleSettings.minimumRequiredLuminance
                    )
                );

            Graphics.Blit(source, destination, material, 0);
        }
    }
}
