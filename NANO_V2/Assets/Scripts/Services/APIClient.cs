// ═══════════════════════════════════════════════════════════════════════════
// APIClient.cs — Cliente REST para Backend (Opcional)
// ═══════════════════════════════════════════════════════════════════════════
// Descarga contenido del museo desde un servidor Node.js local.
// Si el servidor no responde, usa datos por defecto hardcoded.
// Este script es OPCIONAL — solo se usa si se reactiva el backend.
// ═══════════════════════════════════════════════════════════════════════════

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using VRNanoProject.Environment;

namespace VRNanoProject.Services
{
    /// <summary>
    /// Cliente REST que descarga contenido del museo desde un API.
    /// Adjuntar a un GameObject persistente y llamar <see cref="StartFetch"/>.
    /// Falls back silenciosamente a datos por defecto si el servidor está caído.
    /// </summary>
    public class APIClient : MonoBehaviour
    {
        [Header("Server")]
        [Tooltip("URL base del servidor backend. Configurar con variable de entorno si es necesario.")]
        public string serverUrl = "http://localhost:3000";

        /// <summary>Se dispara cuando el contenido fue cargado exitosamente.</summary>
        public event Action<MuseumContentData> OnContentLoaded;

        /// <summary>Se dispara cuando hay un error de conexión.</summary>
        public event Action<string> OnError;

        public void StartFetch() => StartCoroutine(FetchMuseumContent());

        private IEnumerator FetchMuseumContent()
        {
            string url = $"{serverUrl}/api/museum-content";
            Debug.Log($"[APIClient] Fetching museum data from {url}");

            UnityWebRequest req = UnityWebRequest.Get(url);
            req.timeout = 5;
            yield return req.SendWebRequest();

            bool   success = req.result == UnityWebRequest.Result.Success;
            string error   = req.error;
            string rawJson = success ? req.downloadHandler.text : null;
            req.Dispose();

            if (!success)
            {
                Debug.LogWarning($"[APIClient] Server unreachable ({error}). Using built-in defaults.");
                OnError?.Invoke(error);
                OnContentLoaded?.Invoke(MuseumContentDefaults.Default);
                yield break;
            }

            Debug.Log($"[APIClient] Received {rawJson.Length} bytes.");

            MuseumContentDTO dto;
            try { dto = JsonUtility.FromJson<MuseumContentDTO>(rawJson); }
            catch (Exception ex)
            {
                Debug.LogError($"[APIClient] JSON parse error: {ex.Message}");
                OnContentLoaded?.Invoke(MuseumContentDefaults.Default);
                yield break;
            }

            var data = new MuseumContentData
            {
                bannerText = string.IsNullOrWhiteSpace(dto.bannerText)
                    ? MuseumContentDefaults.Default.bannerText
                    : dto.bannerText,
                scientists = MapScientists(dto.scientists),
                nanoTopics = MapTopics(dto.nanoTopics)
            };

            Debug.Log($"[APIClient] Loaded {data.scientists.Length} scientists, {data.nanoTopics.Length} nano-topics.");
            OnContentLoaded?.Invoke(data);
        }

        private ScientistEntry[] MapScientists(ScientistDTO[] dtos)
        {
            if (dtos == null || dtos.Length == 0) return MuseumContentDefaults.Default.scientists;
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

        private NanoTopic[] MapTopics(NanoTopicDTO[] dtos)
        {
            if (dtos == null || dtos.Length == 0) return MuseumContentDefaults.Default.nanoTopics;
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
