// ═══════════════════════════════════════════════════════════════════════════
// MuseumMaster.cs — Orquestador Central del Museo
// ═══════════════════════════════════════════════════════════════════════════
// Controla el ciclo de vida completo de la generación procedural:
//   1. Carga contenido (ScriptableObject o JSON vía ContentLoader)
//   2. Aplica auto-escalado según cantidad de científicos
//   3. Instancia, configura y ejecuta 7 módulos en orden
//   4. Provee validación y reconstrucción desde el editor
// ═══════════════════════════════════════════════════════════════════════════

using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRNanoProject.Environment
{
    /// <summary>
    /// Orquestador maestro del museo VR. Genera el entorno completo
    /// a partir de módulos <see cref="EnvironmentModule"/>.
    /// </summary>
    public class MuseumMaster : MonoBehaviour
    {
        // ── Datos de Entrada ─────────────────────────────────────────────
        [Header("Content")]
        [Tooltip("ScriptableObject con datos de científicos y nanotemas.")]
        public MuseumContent  content;

        [Tooltip("ScriptableObject con la paleta visual del museo.")]
        public MuseumMaterials materials;

        // ── Dimensiones (auto-escalables) ────────────────────────────────
        [Header("Dimensiones del Salón")]
        [Tooltip("Ancho total del salón (metros).")]
        public float anchuraSalon  = 40f;

        [Tooltip("Longitud total del salón (metros).")]
        public float longitudSalon = 60f;

        [Tooltip("Altura de las paredes (metros).")]
        public float alturaParedes = 6f;

        // ── Auto-escala ──────────────────────────────────────────────────
        [Header("Auto Scale")]
        [Tooltip("Escalar automáticamente según la cantidad de contenido.")]
        public bool  autoScale        = true;
        public float minLength        = 32f;
        public float maxLength        = 220f;
        public float lengthPerEntry   = 10f;
        public float baseLength       = 50f;
        public float widthPerEntry    = 3f;
        public float baseWidth        = 36f;
        public float minWidth         = 32f;
        public float maxWidth         = 80f;
        public float heightPerEntry   = 0.4f;
        public float baseHeight       = 5.2f;
        public float minHeight        = 4.8f;
        public float maxHeight        = 10f;

        // ── Lifecycle ────────────────────────────────────────────────────
        [Header("Lifecycle")]
        [Tooltip("Generar automáticamente al iniciar el juego.")]
        public bool buildOnPlay = true;

        [Header("Content Loader")]
        [Tooltip("Usar JSON de StreamingAssets en vez de ScriptableObject.")]
        public bool useJsonContent = false;

        // ── Módulos ──────────────────────────────────────────────────────
        [HideInInspector] public List<EnvironmentModule> modules = new List<EnvironmentModule>();

        // ── Lifecycle ────────────────────────────────────────────────────

        private void Start()
        {
            if (buildOnPlay)
            {
                if (useJsonContent)
                    StartCoroutine(BuildFromJson());
                else
                    Generate();
            }
        }

        private System.Collections.IEnumerator BuildFromJson()
        {
            var loader = gameObject.GetComponent<Services.ContentLoader>()
                ?? gameObject.AddComponent<Services.ContentLoader>();

            bool loaded = false;
            loader.OnContentLoaded += data =>
            {
                if (content)
                {
                    content.bannerText = data.bannerText;
                    content.scientists = data.scientists;
                    content.nanoTopics = data.nanoTopics;
                }
                loaded = true;
            };
            loader.StartLoad();

            float timeout = Time.time + 5f;
            while (!loaded && Time.time < timeout)
                yield return null;

            Generate();
        }

        // ── Generación ───────────────────────────────────────────────────

        /// <summary>Genera el museo completo con todos los módulos.</summary>
        public void Generate()
        {
            ApplyAutoScale();
            Clear();

            // Orden de ejecución importa: Architecture > Banner > Lighting > Exhibits > FX > Lab > Interaction
            EnsureModule<ArchitectureModule>();
            EnsureModule<BannerModule>();
            EnsureModule<LightingModule>();
            EnsureModule<ExhibitsModule>();
            EnsureModule<FXModule>();
            EnsureModule<LabModule>();
            EnsureModule<InteractionModule>();

            foreach (var module in modules)
            {
                module.Configure(this);
                module.Build();
            }

            Debug.Log($"[MuseumMaster] Museo generado: {anchuraSalon}m x {longitudSalon}m x {alturaParedes}m — {modules.Count} módulos.");
        }

        /// <summary>Destruye todo el contenido generado.</summary>
        public void Clear()
        {
            foreach (var module in modules)
            {
                if (module) module.Clear();
            }
        }

        /// <summary>Limpia y regenera el museo.</summary>
        public void Rebuild()
        {
            Clear();
            Generate();
        }

        // ── Auto-escala ──────────────────────────────────────────────────

        /// <summary>Ajusta las dimensiones del salón según la cantidad de contenido.</summary>
        public void ApplyAutoScale()
        {
            if (!autoScale) return;
            var data  = MuseumContentDefaults.Get(content);
            int count = Mathf.Max(
                data.scientists != null ? data.scientists.Length : 0,
                data.nanoTopics != null ? data.nanoTopics.Length : 0);

            longitudSalon = Mathf.Clamp(baseLength + (count * lengthPerEntry), minLength, maxLength);
            anchuraSalon  = Mathf.Clamp(baseWidth  + (count * widthPerEntry),  minWidth,  maxWidth);
            alturaParedes = Mathf.Clamp(baseHeight + (count * heightPerEntry), minHeight, maxHeight);
        }

        // ── Módulo Helper ────────────────────────────────────────────────

        private T EnsureModule<T>() where T : EnvironmentModule
        {
            var existing = GetComponentInChildren<T>();
            if (existing)
            {
                if (!modules.Contains(existing))
                    modules.Add(existing);
                return existing;
            }

            var go = new GameObject(typeof(T).Name);
            go.transform.SetParent(transform, false);
            var module = go.AddComponent<T>();
            modules.Add(module);
            return module;
        }

        // ── Validación ───────────────────────────────────────────────────

        /// <summary>Retorna warnings de todos los módulos.</summary>
        public List<string> Validate()
        {
            var warnings = new List<string>();
            if (!content)   warnings.Add("MuseumMaster: MuseumContent asset not assigned.");
            if (!materials) warnings.Add("MuseumMaster: MuseumMaterials asset not assigned.");
            foreach (var module in modules)
            {
                if (module) warnings.AddRange(module.Validate());
            }
            return warnings;
        }
    }
}
