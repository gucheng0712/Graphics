using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor.Rendering
{
    /// <summary>
    /// <see cref="ProjectWindowCallback.EndNameEditAction"/> for <see cref="RenderPipelineGlobalSettings"/>
    /// </summary>
    public class RenderPipelineGlobalSettingsEndNameEditAction : ProjectWindowCallback.EndNameEditAction
    {
        private Type renderPipelineType { get; set; }
        private Type renderPipelineGlobalSettingsType { get; set; }
        private RenderPipelineGlobalSettings source { get; set; }

        /// <inheritdoc/>
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            RenderPipelineGlobalSettings assetCreated = RenderPipelineGlobalSettingsUtils.Create(renderPipelineGlobalSettingsType, pathName, source) as RenderPipelineGlobalSettings;

            if (renderPipelineType != null)
            {
                if (assetCreated != null)
                    EditorGraphicsSettings.RegisterRenderPipelineSettings(renderPipelineType, assetCreated);
                else
                    EditorGraphicsSettings.UnregisterRenderPipelineSettings(renderPipelineType);
            }

            ProjectWindowUtil.ShowCreatedAsset(assetCreated);
        }

        private static string s_DefaultAssetName = "{0}.asset";

        internal static RenderPipelineGlobalSettingsEndNameEditAction CreateEndNameEditAction(
            Type renderPipelineType, Type renderPipelineGlobalSettingsType, bool updateAssetOnGraphicsSettings, RenderPipelineGlobalSettings source = null)
        {
            var action = ScriptableObject.CreateInstance<RenderPipelineGlobalSettingsEndNameEditAction>();
            action.renderPipelineGlobalSettingsType = renderPipelineGlobalSettingsType;

            if (updateAssetOnGraphicsSettings)
                action.renderPipelineType = renderPipelineType;

            if (source != null)
                action.source = source;

            return action;
        }

        internal static void StartEndNameEditAction(RenderPipelineGlobalSettingsEndNameEditAction action, string pathName)
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                action.GetInstanceID(),
                action,
                pathName,
                CoreEditorStyles.globalSettingsIcon,
                null);
        }

        /// <summary>
        /// Creates a new <see cref="RenderPipelineGlobalSettings"/> of the given type for the given pipeline
        /// </summary>
        /// <typeparam name="TRenderPipeline"></typeparam>
        /// <typeparam name="TGlobalSettings"></typeparam>
        /// <param name="usePathFromCurrentSettings"></param>
        /// <param name="updateAssetOnGraphicsSettings"></param>
        public static void CreateNew<TRenderPipeline, TGlobalSettings>(bool usePathFromCurrentSettings = false, bool updateAssetOnGraphicsSettings = false)
            where TRenderPipeline : RenderPipeline
            where TGlobalSettings : RenderPipelineGlobalSettings
        {
            string pathName = string.Format(s_DefaultAssetName, typeof(TGlobalSettings).Name);

            if (usePathFromCurrentSettings)
            {
                var currentSettings = GraphicsSettings.GetSettingsForRenderPipeline(typeof(TRenderPipeline));
                if (currentSettings != null)
                    pathName = AssetDatabase.GenerateUniqueAssetPath(AssetDatabase.GetAssetPath(currentSettings));
            }

            StartEndNameEditAction(CreateEndNameEditAction(typeof(TRenderPipeline), typeof(TGlobalSettings), updateAssetOnGraphicsSettings), pathName);
        }

        internal static void CloneFrom<TRenderPipeline, TGlobalSettings>(RenderPipelineGlobalSettings source, bool updateAssetOnGraphicsSettings = false)
            where TRenderPipeline : RenderPipeline
            where TGlobalSettings : RenderPipelineGlobalSettings
        {
            var pathName = AssetDatabase.GenerateUniqueAssetPath(AssetDatabase.GetAssetPath(source));
            StartEndNameEditAction(CreateEndNameEditAction(typeof(TRenderPipeline), typeof(TGlobalSettings), updateAssetOnGraphicsSettings, source), pathName);
        }
    }
}
