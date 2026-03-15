using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using NanoVR.Models;

namespace NanoVR.Services
{
    public class APIClient : MonoBehaviour
    {
        [Header("Backend Configuration")]
        [Tooltip("The Base URL to the Node.js server. E.g. http://localhost:3000/api")]
        public string baseURL = "http://localhost:3000/api";

        [Header("Timeout Setup")]
        public int timeoutSeconds = 10;

        /// <summary>
        /// Obtiene la lista asincrónica de Científicos interactivos desde la API REST.
        /// </summary>
        public async Task<ScientistDTO[]> GetScientistsAsync()
        {
            string url = $"{baseURL}/scientists";
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                www.timeout = timeoutSeconds;
                var operation = www.SendWebRequest();

                while (!operation.isDone) {
                    await Task.Yield();
                }

#if UNITY_2020_3_OR_NEWER
                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
#else
                if (www.isNetworkError || www.isHttpError)
#endif
                {
                    Debug.LogError($"[APIClient] Error fetching Scientists: {www.error}");
                    return null;
                }

                string jsonResponse = www.downloadHandler.text;
                // Parseando el JSON array con Helper
                ScientistDTO[] data = JsonHelper.FromJson<ScientistDTO>(jsonResponse);
                return data;
            }
        }

        /// <summary>
        /// Obtiene la lista asincrónica de Temas Nano desde la API REST.
        /// </summary>
        public async Task<NanoTopicDTO[]> GetNanoTopicsAsync()
        {
            string url = $"{baseURL}/nanotopics";
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                www.timeout = timeoutSeconds;
                var operation = www.SendWebRequest();

                while (!operation.isDone) {
                    await Task.Yield();
                }

#if UNITY_2020_3_OR_NEWER
                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
#else
                if (www.isNetworkError || www.isHttpError)
#endif
                {
                    Debug.LogError($"[APIClient] Error fetching NanoTopics: {www.error}");
                    return null;
                }

                string jsonResponse = www.downloadHandler.text;
                NanoTopicDTO[] data = JsonHelper.FromJson<NanoTopicDTO>(jsonResponse);
                return data;
            }
        }
    }
}
