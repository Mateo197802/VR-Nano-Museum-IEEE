// ═══════════════════════════════════════════════════════════════════════════
// ContentLoader.cs — Cargador de Contenido desde StreamingAssets
// ═══════════════════════════════════════════════════════════════════════════
// Lee museum_content.json de StreamingAssets (funciona en TODAS las
// plataformas: Editor, WebGL, Android/Meta Quest, iOS).
// El JSON es editable sin recompilar — ideal para cambiar contenido
// rápidamente antes de una demostración.
// ═══════════════════════════════════════════════════════════════════════════

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using VRNanoProject.Environment;

namespace VRNanoProject.Services
{
    /// <summary>
    /// Carga contenido del museo desde un archivo JSON local en
    /// StreamingAssets. Compatible con todas las plataformas objetivo.
    /// </summary>
    public class ContentLoader : MonoBehaviour
    {
        [Header("Archivo")]
        [Tooltip("Nombre del archivo JSON dentro de StreamingAssets.")]
        public string fileName = "museum_content.json";

        /// <summary>Se dispara cuando el contenido fue cargado.</summary>
        public event Action<MuseumContentData> OnContentLoaded;

        /// <summary>Se dispara cuando hay un error de lectura.</summary>
        public event Action<string> OnError;

        /// <summary>Inicia la carga asíncrona del contenido.</summary>
        public void StartLoad() => StartCoroutine(LoadContent());

        private IEnumerator LoadContent()
        {
            // StreamingAssets requiere UnityWebRequest en WebGL/Android
            string path = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
            Debug.Log($"[ContentLoader] Loading content from {path}");

            UnityWebRequest req = UnityWebRequest.Get(path);
            req.timeout = 5;
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                string error = req.error;
                Debug.LogWarning($"[ContentLoader] Failed to load {fileName}: {error}. Using defaults.");
                req.Dispose();
                OnError?.Invoke(error);
                OnContentLoaded?.Invoke(MuseumContentDefaults.Default);
                yield break;
            }

            string json = req.downloadHandler.text;
            req.Dispose();

            MuseumContentDTO dto;
            try
            {
                dto = JsonUtility.FromJson<MuseumContentDTO>(json);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ContentLoader] JSON parse error: {ex.Message}");
                OnContentLoaded?.Invoke(MuseumContentDefaults.Default);
                yield break;
            }

            var data = MapDTO(dto);
            Debug.Log($"[ContentLoader] Loaded {data.scientists.Length} scientists, {data.nanoTopics.Length} nano-topics.");
            OnContentLoaded?.Invoke(data);
        }

        private MuseumContentData MapDTO(MuseumContentDTO dto)
        {
            var defaults = MuseumContentDefaults.Default;
            return new MuseumContentData
            {
                bannerText = string.IsNullOrWhiteSpace(dto.bannerText)
                    ? defaults.bannerText : dto.bannerText,
                scientists = MapScientists(dto.scientists, defaults.scientists),
                nanoTopics = MapTopics(dto.nanoTopics, defaults.nanoTopics)
            };
        }

        private ScientistEntry[] MapScientists(ScientistDTO[] dtos, ScientistEntry[] fallback)
        {
            if (dtos == null || dtos.Length == 0) return fallback;
            var result = new ScientistEntry[dtos.Length];
            for (int i = 0; i < dtos.Length; i++)
                result[i] = new ScientistEntry
                {
                    shortName = dtos[i].shortName,
                    fullName  = dtos[i].fullName,
                    bio       = dtos[i].bio
                };
            return result;
        }

        private NanoTopic[] MapTopics(NanoTopicDTO[] dtos, NanoTopic[] fallback)
        {
            if (dtos == null || dtos.Length == 0) return fallback;
            var result = new NanoTopic[dtos.Length];
            for (int i = 0; i < dtos.Length; i++)
                result[i] = new NanoTopic
                {
                    title       = dtos[i].title,
                    description = dtos[i].description
                };
            return result;
        }
    }
}
