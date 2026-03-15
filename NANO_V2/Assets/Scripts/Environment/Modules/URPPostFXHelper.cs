// ═══════════════════════════════════════════════════════════════════════════
// URPPostFXHelper.cs — Post-Processing URP via Reflection
// ═══════════════════════════════════════════════════════════════════════════
// Aplica Bloom, ACES Tonemapping, Vignette y Color Grading sin requerir
// referencia directa al assembly de URP — todo resuelto en runtime.
// Si URP no está instalado, el script falla silenciosamente.
// ═══════════════════════════════════════════════════════════════════════════

using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace VRNanoProject.Environment
{
    /// <summary>
    /// Aplica un volumen global de post-procesamiento URP vía reflexión,
    /// permitiendo que el script compile incluso sin el paquete URP.
    /// </summary>
    public static class URPPostFXHelper
    {
        public static void ApplyGlobalVolume(
            Transform parent,
            float bloomThreshold    = 0.9f,
            float bloomIntensity    = 1.4f,
            float vignetteIntensity = 0.35f)
        {
            Type volumeType = Type.GetType("UnityEngine.Rendering.Volume, Unity.RenderPipelines.Core.Runtime");
            if (volumeType == null)
            {
                Debug.LogWarning("[VR Nano] URP Volume type not found. Post-Processing skipped.");
                return;
            }

            var go     = new GameObject("Global_PostFX_Volume");
            go.transform.SetParent(parent, false);
            var volume = go.AddComponent(volumeType);

            volumeType.GetProperty("isGlobal")?.SetValue(volume, true);
            volumeType.GetProperty("priority")?.SetValue(volume, 10f);

            Type profileType = Type.GetType("UnityEngine.Rendering.VolumeProfile, Unity.RenderPipelines.Core.Runtime");
            if (profileType == null) return;

            var profile = ScriptableObject.CreateInstance(profileType) as ScriptableObject;
            if (profile == null) return;

            AddBloom(profile, profileType, bloomThreshold, bloomIntensity);
            AddTonemapping(profile, profileType);
            AddVignette(profile, profileType, vignetteIntensity);
            AddColorAdjustments(profile, profileType);

            volumeType.GetProperty("sharedProfile")?.SetValue(volume, profile);
            Debug.Log("[VR Nano] PostFX: Bloom + ACES + Vignette + Color Grading applied.");
        }

        private static void AddBloom(ScriptableObject profile, Type profileType, float threshold, float intensity)
        {
            var bloom = TryAdd(profile, profileType, "UnityEngine.Rendering.Universal.Bloom, Unity.RenderPipelines.Universal.Runtime");
            if (bloom == null) return;
            SetOverride(bloom, "threshold", threshold);
            SetOverride(bloom, "intensity", intensity);
            SetOverride(bloom, "scatter",   0.7f);
        }

        private static void AddTonemapping(ScriptableObject profile, Type profileType)
        {
            var tone = TryAdd(profile, profileType, "UnityEngine.Rendering.Universal.Tonemapping, Unity.RenderPipelines.Universal.Runtime");
            if (tone == null) return;
            var modeField = tone.GetType().GetField("mode");
            if (modeField != null)
            {
                var param = modeField.GetValue(tone);
                if (param != null)
                {
                    param.GetType()
                        .GetMethod("Override", new[] { param.GetType().GetGenericArguments()[0] })
                        ?.Invoke(param, new object[] { 1 }); // ACES = 1
                }
            }
        }

        private static void AddVignette(ScriptableObject profile, Type profileType, float vigIntensity)
        {
            var vig = TryAdd(profile, profileType, "UnityEngine.Rendering.Universal.Vignette, Unity.RenderPipelines.Universal.Runtime");
            if (vig == null) return;
            SetOverride(vig, "intensity",  vigIntensity);
            SetOverride(vig, "smoothness", 0.4f);
        }

        private static void AddColorAdjustments(ScriptableObject profile, Type profileType)
        {
            var ca = TryAdd(profile, profileType, "UnityEngine.Rendering.Universal.ColorAdjustments, Unity.RenderPipelines.Universal.Runtime");
            if (ca == null) return;
            SetOverride(ca, "postExposure", 0.2f);
            SetOverride(ca, "contrast",     12f);
            SetOverride(ca, "saturation",   8f);
        }

        private static object TryAdd(ScriptableObject profile, Type profileType, string typeName)
        {
            Type effectType = Type.GetType(typeName);
            if (effectType == null) return null;

            var genMethod = profileType.GetMethod("TryAdd");
            if (genMethod != null)
            {
                try
                {
                    var specific = genMethod.MakeGenericMethod(effectType);
                    var args     = new object[] { null };
                    specific.Invoke(profile, args);
                    if (args[0] != null) return args[0];
                }
                catch { /* Method signature mismatch — fall through */ }
            }

            var tryAddNonGeneric = profileType.GetMethod("TryAdd", new[] { typeof(Type), typeof(bool) });
            if (tryAddNonGeneric != null)
                tryAddNonGeneric.Invoke(profile, new object[] { effectType, false });

            Type vcType = Type.GetType("UnityEngine.Rendering.VolumeComponent, Unity.RenderPipelines.Core.Runtime");
            if (vcType == null) return null;
            var getMethod = profileType.GetMethod("TryGet", new[] { typeof(Type), vcType.MakeByRefType() });
            if (getMethod == null) return null;
            object[] getArgs = { effectType, null };
            getMethod.Invoke(profile, getArgs);
            return getArgs[1];
        }

        private static void SetOverride(object component, string fieldName, object value)
        {
            if (component == null) return;
            var field = component.GetType().GetField(fieldName);
            if (field == null) return;
            var param = field.GetValue(component);
            if (param == null) return;
            var method = param.GetType().GetMethod("Override");
            if (method == null) return;
            try { method.Invoke(param, new[] { value }); }
            catch { /* type mismatch between URP versions — silently skip */ }
        }
    }
}
