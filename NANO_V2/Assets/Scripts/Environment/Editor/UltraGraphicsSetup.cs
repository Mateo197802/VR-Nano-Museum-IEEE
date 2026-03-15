// ═══════════════════════════════════════════════════════════════════════════
// UltraGraphicsSetup.cs — Configuración Gráfica de Alta Fidelidad
// ═══════════════════════════════════════════════════════════════════════════
// Se ejecuta automáticamente al cargar el editor y fuerza:
// • Sombras VeryHigh con 4 cascadas
// • Filtrado anisotrópico habilitado
// • 8 pixel lights
// • GI baked
// • URP Asset con textura de profundidad y opaca
// Accesible via menú: VR Nano → Forzar Gráficos Ultra
// ═══════════════════════════════════════════════════════════════════════════

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace VRNanoProject.Environment.Editor
{
    [InitializeOnLoad]
    public static class UltraGraphicsSetup
    {
        static UltraGraphicsSetup()
        {
            EditorApplication.delayCall += ConfigurarAltaFidelidad;
        }

        [MenuItem("VR Nano/Forzar Gráficos Ultra (PBR y Sombras)")]
        public static void ConfigurarAltaFidelidad()
        {
            // ── Sombras Globales ──────────────────────────────────────────
            QualitySettings.shadows              = ShadowQuality.All;
            QualitySettings.shadowResolution     = ShadowResolution.VeryHigh;
            QualitySettings.shadowDistance        = 75f;
            QualitySettings.shadowCascades        = 4;
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
            QualitySettings.softParticles        = true;
            QualitySettings.pixelLightCount      = 8;
            QualitySettings.vSyncCount           = 1;

            // ── GI ───────────────────────────────────────────────────────
            Lightmapping.giWorkflowMode = Lightmapping.GIWorkflowMode.Iterative;
            Lightmapping.bakedGI        = true;
            Lightmapping.realtimeGI     = false;

            // ── URP Asset via Reflection ─────────────────────────────────
            var pipeline = GraphicsSettings.currentRenderPipeline;
            if (pipeline != null)
            {
                var type = pipeline.GetType();
                TrySet(pipeline, type, "shadowDistance",              75f);
                TrySet(pipeline, type, "shadowCascadeCount",         4);
                TrySet(pipeline, type, "supportsCameraDepthTexture", true);
                TrySet(pipeline, type, "supportsCameraOpaqueTexture", true);

                EditorUtility.SetDirty(pipeline);
                AssetDatabase.SaveAssets();
                Debug.Log("<b>[VR Nano]</b> URP Asset ajustado a fidelidad máxima.");
            }
            else
            {
                Debug.LogWarning("<b>[VR Nano]</b> No se detectó un URP Asset activo.");
            }

            Debug.Log("<b>[VR Nano]</b> Gráficos Ultra aplicados correctamente.");
        }

        private static void TrySet(object target, System.Type type, string prop, object value)
        {
            var pi = type.GetProperty(prop,
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            if (pi != null && pi.CanWrite) pi.SetValue(target, value);
        }
    }
}
#endif
