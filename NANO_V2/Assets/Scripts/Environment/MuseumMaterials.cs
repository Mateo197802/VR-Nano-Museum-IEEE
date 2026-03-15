// ═══════════════════════════════════════════════════════════════════════════
// MuseumMaterials.cs — ScriptableObject: Paleta Visual del Museo
// ═══════════════════════════════════════════════════════════════════════════
// Centraliza TODAS las texturas, colores y fuentes del entorno.
// Los módulos nunca hardcodean colores; siempre consultan este asset.
// Uso: Crear asset vía menú VRNanoProject → Museum Materials.
// ═══════════════════════════════════════════════════════════════════════════

using UnityEngine;

namespace VRNanoProject.Environment
{
    /// <summary>
    /// ScriptableObject que centraliza la identidad visual del museo:
    /// texturas PBR, colores holográficos, fuentes tipográficas y
    /// parámetros de emisión para efectos de nanopartículas.
    /// </summary>
    [CreateAssetMenu(menuName = "VRNanoProject/Museum Materials", fileName = "MuseumMaterials")]
    public class MuseumMaterials : ScriptableObject
    {
        // ── Texturas PBR del Museo ───────────────────────────────────────
        [Header("Texturas del Museo")]
        [Tooltip("Textura de mármol para paredes, columnas y pisos.")]
        public Texture2D marmol;

        [Tooltip("Textura de alfombra para la zona de exhibición.")]
        public Texture2D alfombra;

        [Tooltip("Textura de madera para marcos y escenario.")]
        public Texture2D madera;

        // ── Texturas PBR del Laboratorio ─────────────────────────────────
        [Header("Texturas del Laboratorio")]
        [Tooltip("Metal cepillado para equipos de lab.")]
        public Texture2D labMetal;

        [Tooltip("Superficie de trabajo del lab.")]
        public Texture2D labSurface;

        [Tooltip("Piso especial del laboratorio.")]
        public Texture2D labFloor;

        // ── Tipografía ───────────────────────────────────────────────────
        [Header("Fuentes Tipográficas")]
        [Tooltip("Fuente para títulos y nombres (recomendado: Oswald Bold).")]
        public Font displayFont;

        [Tooltip("Fuente para texto descriptivo (recomendado: Roboto Regular).")]
        public Font bodyFont;

        // ── Paleta de Colores ────────────────────────────────────────────
        [Header("Colores — Museo")]
        public Color carpetTint         = new Color(0.12f, 0.10f, 0.12f, 1.0f);
        public Color stageCurtainColor  = new Color(0.35f, 0.05f, 0.08f, 1.0f);
        public Color stageTrimColor     = new Color(0.82f, 0.74f, 0.60f, 1.0f);

        [Header("Colores — Laboratorio")]
        public Color labMetalColor      = new Color(0.74f, 0.78f, 0.82f, 1.0f);
        public Color labAccentColor     = new Color(0.15f, 0.60f, 0.90f, 1.0f);
        public Color goldNanoColor      = new Color(1.00f, 0.78f, 0.20f, 1.0f);
        public Color bioSafetyColor     = new Color(1.00f, 0.80f, 0.10f, 1.0f);

        [Header("Colores — Cristal y Hologramas")]
        public Color glassColor            = new Color(0.30f, 0.80f, 1.00f, 0.30f);
        public Color holoGridColor         = new Color(0.00f, 0.70f, 1.00f, 0.40f);
        public Color holoEmissionColor     = Color.cyan;
        public float holoEmissionIntensity = 2.5f;
        public Color holoTextColor         = new Color(0.75f, 0.95f, 1.00f, 1.0f);
        public Color holoBioLightColor     = new Color(0.20f, 0.80f, 1.00f, 1.0f);

        [Header("Colores — Superficies")]
        public Color paperColor     = new Color(0.97f, 0.97f, 0.93f, 1.0f);
        public Color bannerColor    = Color.black;
        public Color bannerTextColor = Color.white;
        public Color lightColor     = new Color(1.00f, 1.00f, 0.90f, 1.0f);

        [Header("Colores — Nanopartículas")]
        public Color nanoAuraColor  = new Color(0.30f, 0.80f, 1.00f, 0.18f);
        public Color nanoLightColor = new Color(0.30f, 0.90f, 1.00f, 1.00f);

        // ── Prefabs de Laboratorio ───────────────────────────────────────
        [Header("Modelos 3D Importados (Prefabs)")]
        [Tooltip("Prefab para la mesa de trabajo (ej. Chemistry Table)")]
        public GameObject prefabLabBench;
        
        [Tooltip("Prefab de microscopio o equipo de medición")]
        public GameObject prefabMicroscope;
        
        [Tooltip("Prefab de matraz, beaker o tubo de ensayo")]
        public GameObject prefabFlask;
        
        [Tooltip("Prefab decorativo (ej. Pipetas, Centrífuga)")]
        public GameObject prefabLabProp;
    }
}
