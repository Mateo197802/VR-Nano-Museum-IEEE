using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using NanoVR.Models;
using NanoVR.Services;

namespace NanoVR.Environment
{
    [RequireComponent(typeof(APIClient))]
    public class MuseumMaster : MonoBehaviour
    {
        [Header("API Reference")]
        private APIClient apiClient;

        [Header("State")]
        public bool isInitialized = false;

        [Header("Data Received")]
        public ScientistDTO[] loadedScientists;
        public NanoTopicDTO[] loadedNanoTopics;

        private void Awake()
        {
            apiClient = GetComponent<APIClient>();
        }

        private async void Start()
        {
            Debug.Log("[MuseumMaster] Contactando al servidor para construir museo...");
            // Procedimiento asíncrono de carga
            await LoadMuseumData();
            
            if (isInitialized)
            {
                Debug.Log("[MuseumMaster] Datos descargados con éxito. Procediendo a generar geometría...");
                BuildProceduralMuseum();
            }
            else
            {
                Debug.LogError("[MuseumMaster] Fallo al inicializar el museo: Servidor offline o error de red.");
                // Opcional: Cargar geometría con datos de respaldo o Scene Offline
            }
        }

        private async Task LoadMuseumData()
        {
            // Descargar en paralelo para mayor velocidad
            var scientistsTask = apiClient.GetScientistsAsync();
            var topicsTask = apiClient.GetNanoTopicsAsync();

            await Task.WhenAll(scientistsTask, topicsTask);

            loadedScientists = scientistsTask.Result;
            loadedNanoTopics = topicsTask.Result;

            if (loadedScientists != null && loadedNanoTopics != null)
            {
                isInitialized = true;
            }
        }

        private void BuildProceduralMuseum()
        {
            // Aquí iría el código existente tuyo adaptado que instancia los Prefabs.
            // Ejemplo conceptual:
            
            int scientistCount = loadedScientists.Length;
            Debug.Log($"Instanciando {scientistCount} pedestales holográficos.");
            
            foreach (var scientist in loadedScientists)
            {
                // Instantiate(ScientistPrefab, NextPosition(), Quaternion.identity);
                // Configurar textos, audios o imagenes en base al scientist object.
                Debug.Log($"Cargando aponente: {scientist.name}");
            }

            foreach (var topic in loadedNanoTopics)
            {
                // Instantiate(TopicPanelPrefab, NextPosition(), Quaternion.identity);
                Debug.Log($"Cargando tópico: {topic.title}");
            }
        }
    }
}
