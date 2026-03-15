using UnityEngine;
using UnityEditor;
using VRNanoProject.Environment;

#if UNITY_EDITOR
namespace NanoVR.Environment.Editor
{
    public class AutoAssignPBRAssets : EditorWindow
    {
        [MenuItem("NanoVR/Auto-Asignar Texturas y Modelos Pro")]
        public static void AssignAssets()
        {
            Debug.Log("[AutoAssign] Buscando assets descargados del Asset Store...");

            // 1. Buscar MuseumMaterials
            string[] matGuids = AssetDatabase.FindAssets("t:MuseumMaterials");
            if (matGuids.Length == 0)
            {
                Debug.LogError("No se encontró MuseumMaterials. Genera los assets primero en el MuseumMaster.");
                return;
            }
            
            MuseumMaterials materials = AssetDatabase.LoadAssetAtPath<MuseumMaterials>(AssetDatabase.GUIDToAssetPath(matGuids[0]));
            if (materials == null) return;

            bool matUpdated = false;

            // --- ASIGNAR TEXTURAS ---
            // 1. Mármol
            Texture2D marmolTex = FindTexture("Marble");
            if (marmolTex != null) { materials.marmol = marmolTex; matUpdated = true; Debug.Log("✅ Mármol asignado: " + marmolTex.name); }

            // 2. Madera
            Texture2D maderaTex = FindTexture("Wood");
            if (maderaTex != null) { materials.madera = maderaTex; matUpdated = true; Debug.Log("✅ Madera asignada: " + maderaTex.name); }

            // 3. Metal
            Texture2D metalTex = FindTexture("Metal");
            if (metalTex != null) { materials.labMetal = metalTex; matUpdated = true; Debug.Log("✅ Metal asignado: " + metalTex.name); }

            // 4. Tela/Alfombra
            Texture2D telaTex = FindTexture("Fabric");
            if (telaTex == null) telaTex = FindTexture("Carpet"); // Fallback
            if (telaTex != null) { materials.alfombra = telaTex; matUpdated = true; Debug.Log("✅ Tela asignada: " + telaTex.name); }

            // 5. Piso Lab
            Texture2D tileTex = FindTexture("Tile");
            if (tileTex == null) tileTex = FindTexture("Concrete"); // Fallback
            if (tileTex != null) { materials.labFloor = tileTex; matUpdated = true; Debug.Log("✅ Piso lab asignado: " + tileTex.name); }

            // --- ASIGNAR PREFABS (Lab Packs) ---
            GameObject bench = FindPrefab("Chemistry_Table"); // Del Pack "Chemistry Lab Items Pack" o "Free Laboratory Pack"
            if (bench == null) bench = FindPrefab("Table");
            if (bench != null) { materials.prefabLabBench = bench; matUpdated = true; Debug.Log("✅ Mesa asignada: " + bench.name); }

            GameObject microscope = FindPrefab("Microscope"); 
            if (microscope != null) { materials.prefabMicroscope = microscope; matUpdated = true; Debug.Log("✅ Microscopio asignado: " + microscope.name); }

            GameObject flask = FindPrefab("Flask"); 
            if (flask == null) flask = FindPrefab("Beaker");
            if (flask != null) { materials.prefabFlask = flask; matUpdated = true; Debug.Log("✅ Matraz asignado: " + flask.name); }

            GameObject prop = FindPrefab("Centrifuge"); 
            if (prop == null) prop = FindPrefab("Bunsen");
            if (prop == null) prop = FindPrefab("TestTubeRack");
            if (prop != null) { materials.prefabLabProp = prop; matUpdated = true; Debug.Log("✅ Prop de laboratorio asignado: " + prop.name); }

            if (matUpdated)
            {
                EditorUtility.SetDirty(materials);
                AssetDatabase.SaveAssets();

                // Intentar regenerar el museo automáticamente
                MuseumMaster master = GameObject.FindObjectOfType<MuseumMaster>();
                if (master != null) 
                {
                    master.Rebuild();
                    Debug.Log("🏗️ [AutoAssign] Museo reconstruido automáticamente con las nuevas texturas y correcciones.");
                }

                Debug.Log("[AutoAssign] Texturas PBR asignadas con éxito.");
            }
            else
            {
                Debug.LogWarning("[AutoAssign] No se encontraron las texturas. Asegúrate de haber presionado 'Import' en el Package Manager para los assets descargados.");
            }
        }

        private static GameObject FindPrefab(string keyword)
        {
            string[] guids = AssetDatabase.FindAssets(keyword + " t:Prefab");
            if (guids.Length > 0)
                return AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guids[0]));
            return null;
        }

        private static Texture2D FindTexture(string keyword)
        {
            string[] guids = AssetDatabase.FindAssets(keyword + " t:Texture2D");
            if (guids.Length > 0)
            {
                // Buscar la primera que parezca Diffuse/Albedo/BaseColor/Color
                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid).ToLower();
                    if (path.Contains("albedo") || path.Contains("diffuse") || path.Contains("basecolor") || path.Contains("color") || path.Contains("diff"))
                    {
                        return AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guid));
                    }
                }
                // Si no hay especifica, devolver la primera
                return AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(guids[0]));
            }
            return null;
        }
    }
}
#endif
