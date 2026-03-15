// ═══════════════════════════════════════════════════════════════════════════
// LightingModule.cs — Iluminación Profesional del Museo
// ═══════════════════════════════════════════════════════════════════════════
// Configura: Ambient GI tricolor, luz direccional key, fill light cálida,
// spots focalizados en paredes con sombras VeryHigh, y post-processing
// URP (Bloom, ACES Tonemapping, Vignette, Color Grading) vía helper.
// ═══════════════════════════════════════════════════════════════════════════

using UnityEngine;
using UnityEngine.Rendering;

namespace VRNanoProject.Environment
{
    public class LightingModule : EnvironmentModule
    {
        [Header("Layout")]
        public bool  autoLayout   = true;
        public float sidePadding  = 6.5f;
        public float startPadding = 12f;
        public float endPadding   = 18f;
        public int   count        = 6;
        public float y            = 6.4f;

        [Header("Global Light")]
        public bool    createDirectional     = true;
        public Vector3 directionalRotation   = new Vector3(50f, -30f, 0f);
        public float   directionalIntensity  = 0.9f;
        public Color   directionalColor      = new Color(1f, 0.98f, 0.92f);

        [Header("Ambient")]
        public float ambientIntensity = 1.1f;
        public Color ambientSky       = new Color(0.40f, 0.45f, 0.50f);
        public Color ambientEquator   = new Color(0.22f, 0.24f, 0.28f);
        public Color ambientGround    = new Color(0.10f, 0.10f, 0.12f);

        [Header("Spot Lights")]
        public float range            = 25f;
        public float intensity        = 5f;
        public float spotAngle        = 60f;
        public bool  enableShadows    = true;
        public float shadowStrength   = 0.85f;
        public float shadowBias       = 0.06f;
        public float shadowNormalBias  = 0.35f;

        [Header("Post-Processing")]
        [Tooltip("Crea un volumen global de Post-Processing (requiere URP)")]
        public bool  enablePostProcessing = true;
        public float bloomIntensity       = 1.4f;
        public float bloomThreshold       = 0.9f;
        public float vignetteIntensity    = 0.35f;

        public override void Build()
        {
            var mats    = master ? master.materials : null;
            var lampMat = CreateMaterial(new Color(0.05f, 0.05f, 0.05f), 0.92f, false, metallic: 0.95f);

            float w = master ? master.anchuraSalon  : 40f;
            float l = master ? master.longitudSalon : 60f;
            float localLeftX  = autoLayout ? (-w / 2f + sidePadding) : -16f;
            float localRightX = autoLayout ? ( w / 2f - sidePadding) :  16f;
            float localStartZ, localSpacing;

            if (autoLayout)
                ComputeLinearLayout(count, l, startPadding, endPadding, out localStartZ, out localSpacing);
            else
            {
                localStartZ  = -20.5f;
                localSpacing = 9f;
            }

            // ── Ambient GI ───────────────────────────────────────────────
            RenderSettings.ambientMode         = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor     = ambientSky;
            RenderSettings.ambientEquatorColor = ambientEquator;
            RenderSettings.ambientGroundColor  = ambientGround;
            RenderSettings.ambientIntensity    = ambientIntensity;

            // ── Directional Key Light ────────────────────────────────────
            if (createDirectional)
            {
                var dirGo = new GameObject("Key_Light");
                dirGo.transform.SetParent(transform, false);
                dirGo.transform.localRotation = Quaternion.Euler(directionalRotation);
                var dl              = dirGo.AddComponent<Light>();
                dl.type             = LightType.Directional;
                dl.intensity        = directionalIntensity;
                dl.color            = mats ? mats.lightColor : directionalColor;
                dl.shadows          = enableShadows ? LightShadows.Soft : LightShadows.None;
                dl.shadowStrength   = shadowStrength;
                dl.shadowBias       = shadowBias;
                dl.shadowNormalBias = shadowNormalBias;
                dl.shadowResolution = LightShadowResolution.VeryHigh;
            }

            // ── Warm Fill Bounce ─────────────────────────────────────────
            {
                var fill = new GameObject("Fill_Light");
                fill.transform.SetParent(transform, false);
                fill.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                var fl       = fill.AddComponent<Light>();
                fl.type      = LightType.Directional;
                fl.intensity = 0.18f;
                fl.color     = new Color(1f, 0.95f, 0.85f);
                fl.shadows   = LightShadows.None;
            }

            // ── Spot Lamps ───────────────────────────────────────────────
            for (int i = 0; i < count; i++)
            {
                float z = localStartZ + (i * localSpacing);
                CreateLamp(new Vector3(localLeftX,  y, z), new Vector3(25,  90, 0), lampMat, mats);
                CreateLamp(new Vector3(localRightX, y, z), new Vector3(25, -90, 0), lampMat, mats);
            }

            // ── Post-Processing ──────────────────────────────────────────
            if (enablePostProcessing)
                URPPostFXHelper.ApplyGlobalVolume(transform, bloomThreshold, bloomIntensity, vignetteIntensity);
        }

        private void CreateLamp(Vector3 position, Vector3 rotation, Material lampMat, MuseumMaterials mats)
        {
            var lamp = CreatePrimitive(PrimitiveType.Cylinder, "Lamp",
                position, new Vector3(0.3f, 0.6f, 0.3f), lampMat);
            lamp.transform.localRotation = Quaternion.Euler(rotation);

            var lgo = new GameObject("Spot");
            lgo.transform.SetParent(lamp.transform, false);
            lgo.transform.localPosition = Vector3.zero;

            var light              = lgo.AddComponent<Light>();
            light.type             = LightType.Spot;
            light.range            = range;
            light.intensity        = intensity;
            light.spotAngle        = spotAngle;
            light.innerSpotAngle   = spotAngle * 0.6f;
            light.color            = mats ? mats.lightColor : new Color(1f, 1f, 0.9f);
            light.shadows          = enableShadows ? LightShadows.Soft : LightShadows.None;
            light.shadowStrength   = shadowStrength;
            light.shadowBias       = shadowBias;
            light.shadowNormalBias = shadowNormalBias;
            light.shadowResolution = LightShadowResolution.VeryHigh;
        }
    }
}
