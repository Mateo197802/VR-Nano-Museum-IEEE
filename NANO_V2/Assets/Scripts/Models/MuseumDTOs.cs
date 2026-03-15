// ═══════════════════════════════════════════════════════════════════════════
// MuseumDTOs.cs — Data Transfer Objects
// ═══════════════════════════════════════════════════════════════════════════
// Estructuras serializables que mapean directamente al JSON del API/archivo.
// Usadas por APIClient y ContentLoader para deserializar datos.
// ═══════════════════════════════════════════════════════════════════════════

using System;

namespace VRNanoProject.Services
{
    /// <summary>Contenedor raíz del JSON de contenido del museo.</summary>
    [Serializable]
    public class MuseumContentDTO
    {
        public string          bannerText;
        public ScientistDTO[]  scientists;
        public NanoTopicDTO[]  nanoTopics;
    }

    /// <summary>Datos de un científico recibidos del API/JSON.</summary>
    [Serializable]
    public class ScientistDTO
    {
        public int    id;
        public string shortName;
        public string fullName;
        public string bio;
        public string photoUrl;
    }

    /// <summary>Datos de un nanotema recibidos del API/JSON.</summary>
    [Serializable]
    public class NanoTopicDTO
    {
        public int    id;
        public string title;
        public string description;
    }
}
