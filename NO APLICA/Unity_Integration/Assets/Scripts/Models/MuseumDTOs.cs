using System;

namespace NanoVR.Models
{
    [Serializable]
    public class ScientistDTO
    {
        public int id;
        public string name;
        public string description;
        public string image_url;
        public int position_index;
    }

    [Serializable]
    public class NanoTopicDTO
    {
        public int id;
        public string title;
        public string content;
        public string video_url;
        public int position_index;
    }
    
    // Wrapper para deserializar arrays en Unity (JsonUtility no soporta arrays puros en JSON en raíz)
    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            Wrapper<T> wrapper = UnityEngine.JsonUtility.FromJson<Wrapper<T>>("{\"Items\":" + json + "}");
            return wrapper.Items;
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Items;
        }
    }
}
