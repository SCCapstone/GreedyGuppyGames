// <copyright file="UserLutComponent.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

namespace UnityEngine.PostProcessing
{
    public sealed class UserLutComponent : PostProcessingComponentRenderTexture<UserLutModel>
    {
        private static class Uniforms
        {
            internal static readonly int _UserLut = Shader.PropertyToID("_UserLut");
            internal static readonly int _UserLut_Params = Shader.PropertyToID("_UserLut_Params");
        }

        public override bool active
        {
            get
            {
                var settings = this.model.settings;
                return this.model.enabled
                       && settings.lut != null
                       && settings.contribution > 0f
                       && settings.lut.height == (int)Mathf.Sqrt(settings.lut.width)
                       && !this.context.interrupted;
            }
        }

        public override void Prepare(Material uberMaterial)
        {
            var settings = this.model.settings;
            uberMaterial.EnableKeyword("USER_LUT");
            uberMaterial.SetTexture(Uniforms._UserLut, settings.lut);
            uberMaterial.SetVector(Uniforms._UserLut_Params, new Vector4(1f / settings.lut.width, 1f / settings.lut.height, settings.lut.height - 1f, settings.contribution));
        }

        public void OnGUI()
        {
            var settings = this.model.settings;
            var rect = new Rect(this.context.viewport.x * Screen.width + 8f, 8f, settings.lut.width, settings.lut.height);
            GUI.DrawTexture(rect, settings.lut);
        }
    }
}
