// ═══════════════════════════════════════════════════════════════════════════
// MuseumMasterEditor.cs — Inspector Personalizado del Museo
// ═══════════════════════════════════════════════════════════════════════════
// Agrega botones de Generate/Clear/Rebuild al Inspector de MuseumMaster,
// muestra warnings de validación, y permite crear ScriptableObject assets.
// ═══════════════════════════════════════════════════════════════════════════

#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace VRNanoProject.Environment
{
    [CustomEditor(typeof(MuseumMaster))]
    public class MuseumMasterEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();

            var master = (MuseumMaster)target;

            // ── Botones de Generación ────────────────────────────────────
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Generación", EditorStyles.boldLabel);

            if (GUILayout.Button("▶ Generate"))
                RunAction(master, "Generate Museum", master.Generate);

            if (GUILayout.Button("✕ Clear"))
                RunAction(master, "Clear Museum", master.Clear);

            if (GUILayout.Button("↻ Rebuild"))
                RunAction(master, "Rebuild Museum", master.Rebuild);

            EditorGUILayout.EndVertical();

            // ── Validación ───────────────────────────────────────────────
            EditorGUILayout.Space();
            var warnings = master.Validate();
            if (warnings.Count > 0)
            {
                foreach (var warning in warnings)
                    EditorGUILayout.HelpBox(warning, MessageType.Warning);
            }

            // ── Crear Assets por Defecto ─────────────────────────────────
            EditorGUILayout.Space();
            if (GUILayout.Button("Crear Assets por Defecto"))
                CreateDefaultAssets(master);

            // ── Content Tools ────────────────────────────────────────────
            EditorGUILayout.Space();
            DrawContentTools(master);
        }

        private static void RunAction(MuseumMaster master, string actionName, Action action)
        {
            Undo.RegisterFullObjectHierarchyUndo(master.gameObject, actionName);
            action();
            EditorUtility.SetDirty(master);
            EditorSceneManager.MarkSceneDirty(master.gameObject.scene);
        }

        private static void CreateDefaultAssets(MuseumMaster master)
        {
            const string rootFolder = "Assets/Scripts/Environment";
            const string dataFolder = "Assets/Scripts/Environment/Data";

            if (!AssetDatabase.IsValidFolder(dataFolder))
            {
                if (!AssetDatabase.IsValidFolder(rootFolder))
                    AssetDatabase.CreateFolder("Assets/Scripts", "Environment");
                AssetDatabase.CreateFolder(rootFolder, "Data");
            }

            if (!master.content)
            {
                var asset = ScriptableObject.CreateInstance<MuseumContent>();
                string path = AssetDatabase.GenerateUniqueAssetPath(dataFolder + "/MuseumContent.asset");
                AssetDatabase.CreateAsset(asset, path);
                master.content = asset;
            }

            if (!master.materials)
            {
                var asset = ScriptableObject.CreateInstance<MuseumMaterials>();
                string path = AssetDatabase.GenerateUniqueAssetPath(dataFolder + "/MuseumMaterials.asset");
                AssetDatabase.CreateAsset(asset, path);
                master.materials = asset;
            }

            EditorUtility.SetDirty(master);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void DrawContentTools(MuseumMaster master)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Content Tools", EditorStyles.boldLabel);

            if (!master.content)
            {
                EditorGUILayout.HelpBox("Asigna un MuseumContent asset para editar contenido.", MessageType.Info);
                EditorGUILayout.EndVertical();
                return;
            }

            var content = master.content;
            int scientistCount = content.scientists != null ? content.scientists.Length : 0;
            int nanoCount      = content.nanoTopics != null ? content.nanoTopics.Length : 0;
            EditorGUILayout.LabelField("Científicos", scientistCount.ToString());
            EditorGUILayout.LabelField("Nano Topics", nanoCount.ToString());

            if (GUILayout.Button("+ Científico"))
            {
                AddScientist(content);
                master.ApplyAutoScale();
                EditorUtility.SetDirty(master);
                RunAction(master, "Rebuild Museum", master.Rebuild);
            }

            if (GUILayout.Button("+ Nano Topic"))
            {
                AddNanoTopic(content);
                master.ApplyAutoScale();
                EditorUtility.SetDirty(master);
                RunAction(master, "Rebuild Museum", master.Rebuild);
            }

            if (GUILayout.Button("+ Par (Científico + Nano)"))
            {
                AddScientist(content);
                AddNanoTopic(content);
                master.ApplyAutoScale();
                EditorUtility.SetDirty(master);
                RunAction(master, "Rebuild Museum", master.Rebuild);
            }

            EditorGUILayout.EndVertical();
        }

        private static void AddScientist(MuseumContent content)
        {
            Undo.RecordObject(content, "Add Scientist");
            var list = content.scientists ?? Array.Empty<ScientistEntry>();
            Array.Resize(ref list, list.Length + 1);
            list[list.Length - 1] = new ScientistEntry
            {
                shortName = "Nuevo",
                fullName  = "Nuevo Científico",
                bio       = "Biografía del científico.\nAgrega logros y contexto."
            };
            content.scientists = list;
            EditorUtility.SetDirty(content);
            AssetDatabase.SaveAssets();
        }

        private static void AddNanoTopic(MuseumContent content)
        {
            Undo.RecordObject(content, "Add Nano Topic");
            var list = content.nanoTopics ?? Array.Empty<NanoTopic>();
            Array.Resize(ref list, list.Length + 1);
            list[list.Length - 1] = new NanoTopic
            {
                title       = "NUEVA NANOPARTICULA",
                description = "Describe el fenómeno,\naplicaciones y riesgo."
            };
            content.nanoTopics = list;
            EditorUtility.SetDirty(content);
            AssetDatabase.SaveAssets();
        }
    }
}
#endif
