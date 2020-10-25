// <copyright file="PostProcessingBehaviour.cs" company="GreedyGuppyGames">
// Copyright (c) GreedyGuppyGames. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace UnityEngine.PostProcessing
{
    using DebugMode = BuiltinDebugViewsModel.Mode;

#if UNITY_5_4_OR_NEWER
    [ImageEffectAllowedInSceneView]
#endif
    [RequireComponent(typeof(Camera)), DisallowMultipleComponent, ExecuteInEditMode]
    [AddComponentMenu("Effects/Post-Processing Behaviour", -1)]
    public class PostProcessingBehaviour : MonoBehaviour
    {
        // Inspector fields
        public PostProcessingProfile profile;

        public Func<Vector2, Matrix4x4> jitteredMatrixFunc;

        // Internal helpers
        private Dictionary<Type, KeyValuePair<CameraEvent, CommandBuffer>> m_CommandBuffers;
        private List<PostProcessingComponentBase> m_Components;
        private Dictionary<PostProcessingComponentBase, bool> m_ComponentStates;
        private MaterialFactory m_MaterialFactory;
        private RenderTextureFactory m_RenderTextureFactory;
        private PostProcessingContext m_Context;
        private Camera m_Camera;
        private PostProcessingProfile m_PreviousProfile;
        private bool m_RenderingInSceneView = false;

        // Effect components
        private BuiltinDebugViewsComponent m_DebugViews;
        private AmbientOcclusionComponent m_AmbientOcclusion;
        private ScreenSpaceReflectionComponent m_ScreenSpaceReflection;
        private FogComponent m_FogComponent;
        private MotionBlurComponent m_MotionBlur;
        private TaaComponent m_Taa;
        private EyeAdaptationComponent m_EyeAdaptation;
        private DepthOfFieldComponent m_DepthOfField;
        private BloomComponent m_Bloom;
        private ChromaticAberrationComponent m_ChromaticAberration;
        private ColorGradingComponent m_ColorGrading;
        private UserLutComponent m_UserLut;
        private GrainComponent m_Grain;
        private VignetteComponent m_Vignette;
        private DitheringComponent m_Dithering;
        private FxaaComponent m_Fxaa;

        private void OnEnable()
        {
            this.m_CommandBuffers = new Dictionary<Type, KeyValuePair<CameraEvent, CommandBuffer>>();
            this.m_MaterialFactory = new MaterialFactory();
            this.m_RenderTextureFactory = new RenderTextureFactory();
            this.m_Context = new PostProcessingContext();

            // Keep a list of all post-fx for automation purposes
            this.m_Components = new List<PostProcessingComponentBase>();

            // Component list
            this.m_DebugViews = this.AddComponent(new BuiltinDebugViewsComponent());
            this.m_AmbientOcclusion = this.AddComponent(new AmbientOcclusionComponent());
            this.m_ScreenSpaceReflection = this.AddComponent(new ScreenSpaceReflectionComponent());
            this.m_FogComponent = this.AddComponent(new FogComponent());
            this.m_MotionBlur = this.AddComponent(new MotionBlurComponent());
            this.m_Taa = this.AddComponent(new TaaComponent());
            this.m_EyeAdaptation = this.AddComponent(new EyeAdaptationComponent());
            this.m_DepthOfField = this.AddComponent(new DepthOfFieldComponent());
            this.m_Bloom = this.AddComponent(new BloomComponent());
            this.m_ChromaticAberration = this.AddComponent(new ChromaticAberrationComponent());
            this.m_ColorGrading = this.AddComponent(new ColorGradingComponent());
            this.m_UserLut = this.AddComponent(new UserLutComponent());
            this.m_Grain = this.AddComponent(new GrainComponent());
            this.m_Vignette = this.AddComponent(new VignetteComponent());
            this.m_Dithering = this.AddComponent(new DitheringComponent());
            this.m_Fxaa = this.AddComponent(new FxaaComponent());

            // Prepare state observers
            this.m_ComponentStates = new Dictionary<PostProcessingComponentBase, bool>();

            foreach (var component in this.m_Components)
            {
                this.m_ComponentStates.Add(component, false);
            }

            this.useGUILayout = false;
        }

        private void OnPreCull()
        {
            // All the per-frame initialization logic has to be done in OnPreCull instead of Update
            // because [ImageEffectAllowedInSceneView] doesn't trigger Update events...

            this.m_Camera = this.GetComponent<Camera>();

            if (this.profile == null || this.m_Camera == null)
            {
                return;
            }

#if UNITY_EDITOR
            // Track the scene view camera to disable some effects we don't want to see in the
            // scene view
            // Currently disabled effects :
            //  - Temporal Antialiasing
            //  - Depth of Field
            //  - Motion blur
            this.m_RenderingInSceneView = UnityEditor.SceneView.currentDrawingSceneView != null
                && UnityEditor.SceneView.currentDrawingSceneView.camera == this.m_Camera;
#endif

            // Prepare context
            var context = this.m_Context.Reset();
            context.profile = this.profile;
            context.renderTextureFactory = this.m_RenderTextureFactory;
            context.materialFactory = this.m_MaterialFactory;
            context.camera = this.m_Camera;

            // Prepare components
            this.m_DebugViews.Init(context, this.profile.debugViews);
            this.m_AmbientOcclusion.Init(context, this.profile.ambientOcclusion);
            this.m_ScreenSpaceReflection.Init(context, this.profile.screenSpaceReflection);
            this.m_FogComponent.Init(context, this.profile.fog);
            this.m_MotionBlur.Init(context, this.profile.motionBlur);
            this.m_Taa.Init(context, this.profile.antialiasing);
            this.m_EyeAdaptation.Init(context, this.profile.eyeAdaptation);
            this.m_DepthOfField.Init(context, this.profile.depthOfField);
            this.m_Bloom.Init(context, this.profile.bloom);
            this.m_ChromaticAberration.Init(context, this.profile.chromaticAberration);
            this.m_ColorGrading.Init(context, this.profile.colorGrading);
            this.m_UserLut.Init(context, this.profile.userLut);
            this.m_Grain.Init(context, this.profile.grain);
            this.m_Vignette.Init(context, this.profile.vignette);
            this.m_Dithering.Init(context, this.profile.dithering);
            this.m_Fxaa.Init(context, this.profile.antialiasing);

            // Handles profile change and 'enable' state observers
            if (this.m_PreviousProfile != this.profile)
            {
                this.DisableComponents();
                this.m_PreviousProfile = this.profile;
            }

            this.CheckObservers();

            // Find out which camera flags are needed before rendering begins
            // Note that motion vectors will only be available one frame after being enabled
            var flags = DepthTextureMode.None;
            foreach (var component in this.m_Components)
            {
                if (component.active)
                {
                    flags |= component.GetCameraFlags();
                }
            }

            context.camera.depthTextureMode = flags;

            // Temporal antialiasing jittering, needs to happen before culling
            if (!this.m_RenderingInSceneView && this.m_Taa.active && !this.profile.debugViews.willInterrupt)
            {
                this.m_Taa.SetProjectionMatrix(this.jitteredMatrixFunc);
            }
        }

        private void OnPreRender()
        {
            if (this.profile == null)
            {
                return;
            }

            // Command buffer-based effects should be set-up here
            this.TryExecuteCommandBuffer(this.m_DebugViews);
            this.TryExecuteCommandBuffer(this.m_AmbientOcclusion);
            this.TryExecuteCommandBuffer(this.m_ScreenSpaceReflection);
            this.TryExecuteCommandBuffer(this.m_FogComponent);

            if (!this.m_RenderingInSceneView)
            {
                this.TryExecuteCommandBuffer(this.m_MotionBlur);
            }
        }

        private void OnPostRender()
        {
            if (this.profile == null || this.m_Camera == null)
            {
                return;
            }

            if (!this.m_RenderingInSceneView && this.m_Taa.active && !this.profile.debugViews.willInterrupt)
            {
                this.m_Context.camera.ResetProjectionMatrix();
            }
        }

        // Classic render target pipeline for RT-based effects
        // Note that any effect that happens after this stack will work in LDR
        [ImageEffectTransformsToLDR]
        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (this.profile == null || this.m_Camera == null)
            {
                Graphics.Blit(source, destination);
                return;
            }

            // Uber shader setup
            bool uberActive = false;
            bool fxaaActive = this.m_Fxaa.active;
            bool taaActive = this.m_Taa.active && !this.m_RenderingInSceneView;
            bool dofActive = this.m_DepthOfField.active && !this.m_RenderingInSceneView;

            var uberMaterial = this.m_MaterialFactory.Get("Hidden/Post FX/Uber Shader");
            uberMaterial.shaderKeywords = null;

            var src = source;
            var dst = destination;

            if (taaActive)
            {
                var tempRT = this.m_RenderTextureFactory.Get(src);
                this.m_Taa.Render(src, tempRT);
                src = tempRT;
            }

#if UNITY_EDITOR
            // Render to a dedicated target when monitors are enabled so they can show information
            // about the final render.
            // At runtime the output will always be the backbuffer or whatever render target is
            // currently set on the camera.
            if (this.profile.monitors.onFrameEndEditorOnly != null)
                dst = this.m_RenderTextureFactory.Get(src);
#endif

            Texture autoExposure = GraphicsUtils.whiteTexture;
            if (this.m_EyeAdaptation.active)
            {
                uberActive = true;
                autoExposure = this.m_EyeAdaptation.Prepare(src, uberMaterial);
            }

            uberMaterial.SetTexture("_AutoExposure", autoExposure);

            if (dofActive)
            {
                uberActive = true;
                this.m_DepthOfField.Prepare(src, uberMaterial, taaActive, this.m_Taa.jitterVector, this.m_Taa.model.settings.taaSettings.motionBlending);
            }

            if (this.m_Bloom.active)
            {
                uberActive = true;
                this.m_Bloom.Prepare(src, uberMaterial, autoExposure);
            }

            uberActive |= this.TryPrepareUberImageEffect(this.m_ChromaticAberration, uberMaterial);
            uberActive |= this.TryPrepareUberImageEffect(this.m_ColorGrading, uberMaterial);
            uberActive |= this.TryPrepareUberImageEffect(this.m_Vignette, uberMaterial);
            uberActive |= this.TryPrepareUberImageEffect(this.m_UserLut, uberMaterial);

            var fxaaMaterial = fxaaActive
                ? this.m_MaterialFactory.Get("Hidden/Post FX/FXAA")
                : null;

            if (fxaaActive)
            {
                fxaaMaterial.shaderKeywords = null;
                this.TryPrepareUberImageEffect(this.m_Grain, fxaaMaterial);
                this.TryPrepareUberImageEffect(this.m_Dithering, fxaaMaterial);

                if (uberActive)
                {
                    var output = this.m_RenderTextureFactory.Get(src);
                    Graphics.Blit(src, output, uberMaterial, 0);
                    src = output;
                }

                this.m_Fxaa.Render(src, dst);
            }
            else
            {
                uberActive |= this.TryPrepareUberImageEffect(this.m_Grain, uberMaterial);
                uberActive |= this.TryPrepareUberImageEffect(this.m_Dithering, uberMaterial);

                if (uberActive)
                {
                    if (!GraphicsUtils.isLinearColorSpace)
                    {
                        uberMaterial.EnableKeyword("UNITY_COLORSPACE_GAMMA");
                    }

                    Graphics.Blit(src, dst, uberMaterial, 0);
                }
            }

            if (!uberActive && !fxaaActive)
            {
                Graphics.Blit(src, dst);
            }

#if UNITY_EDITOR
            if (this.profile.monitors.onFrameEndEditorOnly != null)
            {
                Graphics.Blit(dst, destination);

                var oldRt = RenderTexture.active;
                this.profile.monitors.onFrameEndEditorOnly(dst);
                RenderTexture.active = oldRt;
            }
#endif

            this.m_RenderTextureFactory.ReleaseAll();
        }

        private void OnGUI()
        {
            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            if (this.profile == null || this.m_Camera == null)
            {
                return;
            }

            if (this.m_EyeAdaptation.active && this.profile.debugViews.IsModeActive(DebugMode.EyeAdaptation))
            {
                this.m_EyeAdaptation.OnGUI();
            }
            else if (this.m_ColorGrading.active && this.profile.debugViews.IsModeActive(DebugMode.LogLut))
            {
                this.m_ColorGrading.OnGUI();
            }
            else if (this.m_UserLut.active && this.profile.debugViews.IsModeActive(DebugMode.UserLut))
            {
                this.m_UserLut.OnGUI();
            }
        }

        private void OnDisable()
        {
            // Clear command buffers
            foreach (var cb in this.m_CommandBuffers.Values)
            {
                this.m_Camera.RemoveCommandBuffer(cb.Key, cb.Value);
                cb.Value.Dispose();
            }

            this.m_CommandBuffers.Clear();

            // Clear components
            if (this.profile != null)
            {
                this.DisableComponents();
            }

            this.m_Components.Clear();

            // Reset camera mode
            if (this.m_Camera != null)
            {
                this.m_Camera.depthTextureMode = DepthTextureMode.None;
            }

            // Factories
            this.m_MaterialFactory.Dispose();
            this.m_RenderTextureFactory.Dispose();
            GraphicsUtils.Dispose();
        }

        public void ResetTemporalEffects()
        {
            this.m_Taa.ResetHistory();
            this.m_MotionBlur.ResetHistory();
            this.m_EyeAdaptation.ResetHistory();
        }

        #region State management

        private List<PostProcessingComponentBase> m_ComponentsToEnable = new List<PostProcessingComponentBase>();
        private List<PostProcessingComponentBase> m_ComponentsToDisable = new List<PostProcessingComponentBase>();

        private void CheckObservers()
        {
            foreach (var cs in this.m_ComponentStates)
            {
                var component = cs.Key;
                var state = component.GetModel().enabled;

                if (state != cs.Value)
                {
                    if (state)
                    {
                        this.m_ComponentsToEnable.Add(component);
                    }
                    else
                    {
                        this.m_ComponentsToDisable.Add(component);
                    }
                }
            }

            for (int i = 0; i < this.m_ComponentsToDisable.Count; i++)
            {
                var c = this.m_ComponentsToDisable[i];
                this.m_ComponentStates[c] = false;
                c.OnDisable();
            }

            for (int i = 0; i < this.m_ComponentsToEnable.Count; i++)
            {
                var c = this.m_ComponentsToEnable[i];
                this.m_ComponentStates[c] = true;
                c.OnEnable();
            }

            this.m_ComponentsToDisable.Clear();
            this.m_ComponentsToEnable.Clear();
        }

        private void DisableComponents()
        {
            foreach (var component in this.m_Components)
            {
                var model = component.GetModel();
                if (model != null && model.enabled)
                {
                    component.OnDisable();
                }
            }
        }

        #endregion

        #region Command buffer handling & rendering helpers
        // Placeholders before the upcoming Scriptable Render Loop as command buffers will be
        // executed on the go so we won't need of all that stuff
        private CommandBuffer AddCommandBuffer<T>(CameraEvent evt, string name)
            where T : PostProcessingModel
        {
            var cb = new CommandBuffer { name = name };
            var kvp = new KeyValuePair<CameraEvent, CommandBuffer>(evt, cb);
            this.m_CommandBuffers.Add(typeof(T), kvp);
            this.m_Camera.AddCommandBuffer(evt, kvp.Value);
            return kvp.Value;
        }

        private void RemoveCommandBuffer<T>()
            where T : PostProcessingModel
        {
            KeyValuePair<CameraEvent, CommandBuffer> kvp;
            var type = typeof(T);

            if (!this.m_CommandBuffers.TryGetValue(type, out kvp))
            {
                return;
            }

            this.m_Camera.RemoveCommandBuffer(kvp.Key, kvp.Value);
            this.m_CommandBuffers.Remove(type);
            kvp.Value.Dispose();
        }

        private CommandBuffer GetCommandBuffer<T>(CameraEvent evt, string name)
            where T : PostProcessingModel
        {
            CommandBuffer cb;
            KeyValuePair<CameraEvent, CommandBuffer> kvp;

            if (!this.m_CommandBuffers.TryGetValue(typeof(T), out kvp))
            {
                cb = this.AddCommandBuffer<T>(evt, name);
            }
            else if (kvp.Key != evt)
            {
                this.RemoveCommandBuffer<T>();
                cb = this.AddCommandBuffer<T>(evt, name);
            }
            else cb = kvp.Value;

            return cb;
        }

        private void TryExecuteCommandBuffer<T>(PostProcessingComponentCommandBuffer<T> component)
            where T : PostProcessingModel
        {
            if (component.active)
            {
                var cb = this.GetCommandBuffer<T>(component.GetCameraEvent(), component.GetName());
                cb.Clear();
                component.PopulateCommandBuffer(cb);
            }
            else this.RemoveCommandBuffer<T>();
        }

        private bool TryPrepareUberImageEffect<T>(PostProcessingComponentRenderTexture<T> component, Material material)
            where T : PostProcessingModel
        {
            if (!component.active)
            {
                return false;
            }

            component.Prepare(material);
            return true;
        }

        private T AddComponent<T>(T component)
            where T : PostProcessingComponentBase
        {
            this.m_Components.Add(component);
            return component;
        }

        #endregion
    }
}
