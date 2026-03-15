// ═══════════════════════════════════════════════════════════════════════════
// EnvironmentModule.cs — Base Abstracta de Módulos del Museo
// ═══════════════════════════════════════════════════════════════════════════
// Todos los módulos (Architecture, Exhibits, Lab, etc.) heredan de esta
// clase. Provee utilidades compartidas: creación de primitivas con
// materiales PBR (URP/Standard), TextMesh con emisión, y layout lineal.
// ═══════════════════════════════════════════════════════════════════════════

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace VRNanoProject.Environment
{
    /// <summary>
    /// Clase base abstracta para módulos procedurales del museo.
    /// Cada módulo recibe una referencia al <see cref="MuseumMaster"/>
    /// y genera su geometría mediante <see cref="Build"/>.
    /// </summary>
    public abstract class EnvironmentModule : MonoBehaviour
    {
        [HideInInspector] public MuseumMaster master;

        // ── Ciclo de vida ───────────────────────────────────────────────

        /// <summary>Recibe la referencia al orquestador.</summary>
        public virtual void Configure(MuseumMaster owner)
        {
            master = owner;
        }

        /// <summary>Genera toda la geometría del módulo.</summary>
        public abstract void Build();

        /// <summary>Destruye toda la geometría hija.</summary>
        public virtual void Clear()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i).gameObject;
                if (Application.isPlaying)
                    Destroy(child);
                else
                    DestroyImmediate(child);
            }
        }

        /// <summary>Devuelve advertencias de configuración.</summary>
        public virtual List<string> Validate()
        {
            return new List<string>();
        }

        // ── Utilidades de Construcción ───────────────────────────────────

        /// <summary>
        /// Crea una primitiva 3D con material PBR, posición y escala.
        /// Soporta tanto URP como Standard pipeline automáticamente.
        /// </summary>
        protected GameObject CreatePrimitive(
            PrimitiveType type,
            string        name,
            Vector3       position,
            Vector3       scale,
            Material      material,
            Transform     parent        = null,
            bool          castShadows   = true,
            bool          receiveShadows = true)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent ? parent : transform, false);
            go.transform.localPosition = position;
            go.transform.localScale    = scale;

            var renderer = go.GetComponent<Renderer>();
            if (renderer)
            {
                if (material) renderer.sharedMaterial = material;
                renderer.shadowCastingMode = castShadows
                    ? ShadowCastingMode.On
                    : ShadowCastingMode.Off;
                renderer.receiveShadows = receiveShadows;
            }
            return go;
        }

        /// <summary>
        /// Crea un material PBR compatible con URP Lit o Standard.
        /// Soporta transparencia, metalicidad y tiling de texturas.
        /// </summary>
        protected Material CreateMaterial(
            Color     color,
            float     smoothness,
            bool      transparent,
            Texture2D texture      = null,
            Vector2?  textureScale = null,
            float     metallic     = 0f)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (!shader) shader = Shader.Find("Standard");

            var mat = new Material(shader);

            // Colores base (ambos pipelines)
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            if (mat.HasProperty("_Color"))     mat.SetColor("_Color",     color);

            // PBR parámetros
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", smoothness);
            if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", smoothness);
            if (mat.HasProperty("_Metallic"))   mat.SetFloat("_Metallic",   metallic);

            // Textura con tiling
            if (texture)
            {
                mat.mainTexture = texture;
                if (textureScale.HasValue)
                    mat.mainTextureScale = textureScale.Value;
            }

            // Transparencia
            if (transparent)
            {
                if (mat.HasProperty("_Surface"))
                    mat.SetFloat("_Surface", 1f);
                mat.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite",   0);
                mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
                mat.renderQueue = 3000;
            }

            return mat;
        }

        /// <summary>Crea un material PBR con canal de emisión activo.</summary>
        protected Material CreateEmissiveMaterial(
            Color     color,
            float     smoothness,
            bool      transparent,
            Color     emissionColor,
            float     emissionIntensity,
            Texture2D texture      = null,
            Vector2?  textureScale = null,
            float     metallic     = 0f)
        {
            var mat = CreateMaterial(color, smoothness, transparent, texture, textureScale, metallic);
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", emissionColor * emissionIntensity);
            return mat;
        }

        /// <summary>
        /// Crea un TextMesh 3D con soporte para emisión luminosa,
        /// fuente personalizada y alineación configurable.
        /// </summary>
        protected TextMesh CreateTextMesh(
            string        name,
            string        text,
            Vector3       position,
            Vector3       rotation,
            float         characterSize,
            int           fontSize,
            Color         color,
            Font          font,
            FontStyle     fontStyle,
            Transform     parent             = null,
            TextAnchor    anchor             = TextAnchor.MiddleCenter,
            TextAlignment alignment          = TextAlignment.Center,
            float         lineSpacing        = 1f,
            bool          emissive           = false,
            Color         emissionColor      = default,
            float         emissionIntensity  = 1f)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent ? parent : transform, false);
            go.transform.localPosition = position;
            go.transform.localRotation = Quaternion.Euler(rotation);

            var tm          = go.AddComponent<TextMesh>();
            tm.text         = text;
            tm.color        = color;
            tm.fontSize     = fontSize;
            tm.characterSize = characterSize;
            tm.anchor       = anchor;
            tm.alignment    = alignment;
            tm.fontStyle    = fontStyle;
            tm.lineSpacing  = lineSpacing;

            if (font)
            {
                tm.font = font;
                var textRenderer = tm.GetComponent<Renderer>();
                if (textRenderer) textRenderer.sharedMaterial = font.material;
            }

            var renderer = tm.GetComponent<Renderer>();
            if (renderer)
            {
                renderer.shadowCastingMode = ShadowCastingMode.Off;
                renderer.receiveShadows    = false;
                if (emissive)
                {
                    var mat = renderer.material;
                    mat.EnableKeyword("_EMISSION");
                    mat.SetColor("_EmissionColor", emissionColor * emissionIntensity);
                }
            }

            return tm;
        }

        /// <summary>
        /// Calcula posiciones equidistantes a lo largo de un eje Z,
        /// respetando márgenes de inicio y final.
        /// </summary>
        protected void ComputeLinearLayout(
            int   count,
            float length,
            float startPadding,
            float endPadding,
            out float startZ,
            out float spacing)
        {
            float usable = Mathf.Max(0f, length - startPadding - endPadding);
            startZ  = -length / 2f + startPadding;
            spacing = count > 1 ? usable / (count - 1) : 0f;
        }
    }
}
