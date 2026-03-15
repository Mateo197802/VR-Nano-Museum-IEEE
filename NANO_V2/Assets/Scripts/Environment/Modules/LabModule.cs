// ═══════════════════════════════════════════════════════════════════════════
// LabModule.cs — Laboratorio de Síntesis de Nanopartículas de Oro
// ═══════════════════════════════════════════════════════════════════════════
// Genera proceduralmente un laboratorio completo con:
//   • Mesas de trabajo con reactivos (botellas de vidrio con líquidos)
//   • Reactor de oro con tubo de vidrio y contenido dorado emisivo
//   • Cámara de extracción con panel de vidrio
//   • Campana de flujo laminar con luz interior
//   • Estación NanoDrops (espectrofotómetro)
//   • Fregadero de laboratorio
//   • Señal de bioseguridad
//   • Panel de manual interactivo (Canvas WorldSpace)
//   • Sistema de tareas guiadas (LabTaskManager)
//   • Bot guía con burbuja de texto que sigue al usuario
//   • Puerta automática de cristal (AutoDoor)
// ═══════════════════════════════════════════════════════════════════════════

using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace VRNanoProject.Environment
{
    public class LabModule : EnvironmentModule
    {
        [Header("Layout")]
        public bool    autoLayout       = true;
        public float   labCenterZ       = 32f;
        public float   benchRowOffsetX  = 10f;
        public int     benchesPerRow    = 2;
        public float   benchSpacingZ    = 7f;
        public Vector3 benchSize        = new Vector3(4f, 0.9f, 1.2f);
        public float   benchTopThickness = 0.08f;

        [Header("Stations")]
        public Vector3 reactorOffset    = new Vector3(0, 0, 2f);
        public Vector3 extractionOffset = new Vector3(0, 0, 8f);
        public Vector3 hoodOffset       = new Vector3(8f, 0, -2f);
        public Vector3 nanoDropOffset   = new Vector3(8f, 0, 4f);
        public Vector3 sinkOffset       = new Vector3(-8f, 0, 6f);
        public Vector3 ppeOffset        = new Vector3(0, 0, -10f);

        [Header("Lab Material Tiling")]
        public float metalTile   = 1.2f;
        public float surfaceTile = 1.1f;

        [Header("Manual UI")]
        public Vector3 manualPosition  = new Vector3(0f, 3.4f, 30f);
        public Vector3 manualRotation  = new Vector3(0f, 0f, 0f); // Antes estaba en 180, causando que el texto se viera al revés
        public Vector2 manualSize      = new Vector2(500f, 280f); // Reducido para que no sea inmenso
        public float   manualScale     = 0.008f; // Nueva variable para controlar el tamaño en WorldSpace
        public Color   manualPanelColor = new Color(0.05f, 0.08f, 0.1f, 0.85f);
        public Color   manualTextColor  = new Color(0.9f, 0.95f, 1f, 1f);
        public Font    manualFont;

        [Header("Bot")]
        public Vector3 botStartOffset   = new Vector3(1.6f, 0f, -1.2f);
        public float   botFollowDistance = 2.1f;
        public float   botMoveSpeed     = 2.2f;
        public float   botTurnSpeed     = 6f;

        [Header("Door")]
        public Vector3   doorOpenOffset   = new Vector3(0, 0, 1.6f);
        public float     doorOpenDistance  = 2.8f;
        public Transform userOverride;

        public override void Build()
        {
            var mats = master ? master.materials : null;
            var user = ResolveUser();
            float w  = master ? master.anchuraSalon  : 56f;
            float l  = master ? master.longitudSalon : 90f;
            float centerZ = autoLayout ? Mathf.Lerp(0f, l / 2f, 0.65f) : labCenterZ;

            var labRoot = new GameObject("Lab_Root");
            labRoot.transform.SetParent(transform, false);

            // ── Mesas de Trabajo ─────────────────────────────────────────
            float startZ = centerZ - (benchSpacingZ * (benchesPerRow - 1) * 0.5f);
            for (int row = 0; row < 2; row++)
            {
                float x = row == 0 ? -benchRowOffsetX : benchRowOffsetX;
                for (int i = 0; i < benchesPerRow; i++)
                {
                    float z     = startZ + (i * benchSpacingZ);
                    var   bench = CreateBench(new Vector3(x, 0, z), mats, labRoot.transform);
                    CreateReagents(bench, mats);
                }
            }

            // ── Estaciones ───────────────────────────────────────────────
            Vector3 reactorPos    = new Vector3(0, 0, centerZ) + reactorOffset;
            Vector3 extractionPos = new Vector3(0, 0, centerZ) + extractionOffset;
            Vector3 hoodPos       = new Vector3(0, 0, centerZ) + hoodOffset;
            Vector3 nanoPos       = new Vector3(0, 0, centerZ) + nanoDropOffset;
            Vector3 sinkPos       = new Vector3(0, 0, centerZ) + sinkOffset;

            CreateGoldReactor(reactorPos, mats, labRoot.transform);
            CreateExtractionChamber(extractionPos, mats, labRoot.transform);
            CreateFlowHood(hoodPos, mats, labRoot.transform);
            CreateNanoDrops(nanoPos, mats, labRoot.transform);
            CreateSink(sinkPos, mats, labRoot.transform);
            CreateBioSafetySign(new Vector3(-w / 2f + 2.2f, 2.8f, centerZ + 6f), mats, labRoot.transform);

            // ── Manual UI ────────────────────────────────────────────────
            Vector3 manualPos = manualPosition;
            if (autoLayout) manualPos = new Vector3(0f, 3.4f, centerZ + 6f);
            var manualText = CreateManual(labRoot.transform, manualPos, manualRotation);

            // ── Sistema de Tareas ────────────────────────────────────────
            var taskManagerGo = new GameObject("Lab_Task_Manager");
            taskManagerGo.transform.SetParent(labRoot.transform, false);
            var taskManager       = taskManagerGo.AddComponent<LabTaskManager>();
            taskManager.manualText = manualText;
            taskManager.title      = "Manual de Bioseguridad - Laboratorio AuNP";
            taskManager.userRoot   = user;
            taskManager.RefreshManual();

            // ── Triggers ─────────────────────────────────────────────────
            Transform tPpe        = CreateTaskTrigger(labRoot.transform, "Task_PPE",        new Vector3(0, 1f, centerZ) + ppeOffset, new Vector3(3f, 2.2f, 3f));
            Transform tBench      = CreateTaskTrigger(labRoot.transform, "Task_Bench",       new Vector3(-benchRowOffsetX, 1f, centerZ), new Vector3(3f, 2.2f, 3f));
            Transform tReactor    = CreateTaskTrigger(labRoot.transform, "Task_Reactor",     reactorPos    + new Vector3(0, 1f, 0), new Vector3(3f, 2.2f, 3f));
            Transform tNano       = CreateTaskTrigger(labRoot.transform, "Task_NanoDrops",   nanoPos       + new Vector3(0, 1f, 0), new Vector3(3f, 2.2f, 3f));
            Transform tExtraction = CreateTaskTrigger(labRoot.transform, "Task_Extraction",  extractionPos + new Vector3(0, 1f, 0), new Vector3(3f, 2.2f, 3f));
            Transform tSink       = CreateTaskTrigger(labRoot.transform, "Task_Sink",        sinkPos       + new Vector3(0, 1f, 0), new Vector3(3f, 2.2f, 3f));

            taskManager.RegisterTask(tPpe,        "Colocar EPP (bata y guantes)",   "Acercate al punto de EPP para iniciar.");
            taskManager.RegisterTask(tBench,      "Desinfectar mesa de trabajo",    "Activa la zona de mesa y limpia la superficie.");
            taskManager.RegisterTask(tReactor,    "Preparar reactivo de oro",       "Ingresa al reactor y prepara la solucion.");
            taskManager.RegisterTask(tNano,       "Calibrar Nanodrops",             "Verifica el equipo Nanodrops y calibra.");
            taskManager.RegisterTask(tExtraction, "Activar camara de extraccion",   "Enciende la camara de extraccion.");
            taskManager.RegisterTask(tSink,       "Lavado y descarte",              "Completa lavado de manos y descarte.");

            // ── Bot Guía ─────────────────────────────────────────────────
            var bot          = CreateLabBot(labRoot.transform, new Vector3(0, 0, centerZ) + botStartOffset, mats);
            var botFollower  = bot.AddComponent<LabBotFollower>();
            botFollower.user           = user;
            botFollower.taskManager    = taskManager;
            botFollower.followDistance = botFollowDistance;
            botFollower.moveSpeed     = botMoveSpeed;
            botFollower.turnSpeed     = botTurnSpeed;
            var bubble = bot.transform.Find("Bot_Bubble");
            if (bubble) botFollower.bubble = bubble;

            // ── Puerta Automática ────────────────────────────────────────
            SetupAutoDoor(user);
        }

        // ── Resolución del Usuario ───────────────────────────────────────

        private Transform ResolveUser()
        {
            if (userOverride) return userOverride;
            if (master)
            {
                var interaction = master.GetComponentInChildren<InteractionModule>();
                if (interaction && interaction.xrRigRoot) return interaction.xrRigRoot.transform;
            }
            var cam = Camera.main;
            return cam ? cam.transform : null;
        }

        private void SetupAutoDoor(Transform user)
        {
            if (!master) return;
            var door = FindChildByName(master.transform, "Puerta_Cristal_Lab");
            if (!door) return;
            var autoDoor = door.GetComponent<AutoDoor>() ?? door.AddComponent<AutoDoor>();
            autoDoor.user         = user;
            autoDoor.openOffset   = doorOpenOffset;
            autoDoor.openDistance  = doorOpenDistance;
        }

        // ── Constructores ────────────────────────────────────────────────

        private Transform CreateBench(Vector3 position, MuseumMaterials mats, Transform parent)
        {
            var root = new GameObject("Lab_Bench");
            root.transform.SetParent(parent, false);
            root.transform.localPosition = position;

            if (mats && mats.prefabLabBench)
            {
                var instance = Instantiate(mats.prefabLabBench, root.transform, false);
                instance.transform.localPosition = Vector3.zero;
                return root.transform;
            }

            var metal = CreateLabMetal(mats, 0.6f, 0.4f);
            var top   = CreateLabSurface(mats, 0.35f, 0.25f);

            float baseHeight = benchSize.y - benchTopThickness;
            CreatePrimitive(PrimitiveType.Cube, "Bench_Base", new Vector3(0, baseHeight / 2f, 0), new Vector3(benchSize.x, baseHeight, benchSize.z), metal, root.transform);
            CreatePrimitive(PrimitiveType.Cube, "Bench_Top",  new Vector3(0, benchSize.y - benchTopThickness / 2f, 0), new Vector3(benchSize.x, benchTopThickness, benchSize.z), top, root.transform);
            CreatePrimitive(PrimitiveType.Cube, "Bench_Drawer", new Vector3(0.9f, baseHeight * 0.4f, benchSize.z / 2f - 0.05f), new Vector3(0.6f, 0.35f, 0.08f), metal, root.transform);
            return root.transform;
        }

        private void CreateReagents(Transform bench, MuseumMaterials mats)
        {
            var glass    = CreateMaterial(mats ? mats.glassColor : new Color(0.5f, 0.8f, 1f, 0.35f), 0.9f, true);
            var gold     = mats ? mats.goldNanoColor : new Color(1f, 0.78f, 0.2f, 1f);
            var goldGlow = CreateEmissiveMaterial(gold, 0.8f, true, gold, 2.6f);
            var cyanGlow = CreateEmissiveMaterial(new Color(0.3f, 0.9f, 1f), 0.8f, true, new Color(0.3f, 0.9f, 1f), 2.2f);

            Vector3[] positions =
            {
                new Vector3(-0.8f, benchSize.y + 0.05f, -0.2f),
                new Vector3(-0.4f, benchSize.y + 0.05f,  0.2f),
                new Vector3( 0.2f, benchSize.y + 0.05f, -0.1f),
                new Vector3( 0.6f, benchSize.y + 0.05f,  0.25f)
            };

            for (int i = 0; i < positions.Length; i++)
            {
                if (mats && mats.prefabFlask)
                {
                    var instance = Instantiate(mats.prefabFlask, bench, false);
                    instance.transform.localPosition = positions[i];
                }
                else
                {
                    CreatePrimitive(PrimitiveType.Cylinder, "Reagent_Bottle", positions[i], new Vector3(0.18f, 0.28f, 0.18f), glass, bench);
                    CreatePrimitive(PrimitiveType.Cylinder, "Reagent_Liquid", positions[i] + new Vector3(0, -0.05f, 0), new Vector3(0.16f, 0.18f, 0.16f), i % 2 == 0 ? goldGlow : cyanGlow, bench, false, false);
                }
            }
        }

        private void CreateGoldReactor(Vector3 position, MuseumMaterials mats, Transform parent)
        {
            var metal    = CreateLabMetal(mats, 0.6f, 0.5f);
            var gold     = mats ? mats.goldNanoColor : new Color(1f, 0.78f, 0.2f, 1f);
            var goldGlow = CreateEmissiveMaterial(gold, 0.9f, true, gold, 3.2f);
            var glass    = CreateMaterial(mats ? mats.glassColor : new Color(0.5f, 0.8f, 1f, 0.35f), 0.9f, true);

            var root = new GameObject("Gold_Reactor");
            root.transform.SetParent(parent, false);
            root.transform.localPosition = position;

            CreatePrimitive(PrimitiveType.Cylinder, "Reactor_Base",     new Vector3(0, 0.4f, 0),  new Vector3(1.4f, 0.4f, 1.4f), metal, root.transform);
            CreatePrimitive(PrimitiveType.Cylinder, "Reactor_Tube",     new Vector3(0, 1.3f, 0),  new Vector3(0.9f, 0.9f, 0.9f), glass, root.transform, false, false);
            CreatePrimitive(PrimitiveType.Cylinder, "Reactor_Contents", new Vector3(0, 1.25f, 0), new Vector3(0.75f, 0.7f, 0.75f), goldGlow, root.transform, false, false);
            CreatePrimitive(PrimitiveType.Cube,     "Reactor_Panel",    new Vector3(0, 0.9f, 0.8f), new Vector3(0.9f, 0.6f, 0.1f), metal, root.transform);
        }

        private void CreateExtractionChamber(Vector3 position, MuseumMaterials mats, Transform parent)
        {
            var metal = CreateLabMetal(mats, 0.5f, 0.4f);
            var glass = CreateMaterial(mats ? mats.glassColor : new Color(0.5f, 0.8f, 1f, 0.35f), 0.9f, true);

            var root = new GameObject("Extraction_Chamber");
            root.transform.SetParent(parent, false);
            root.transform.localPosition = position;

            CreatePrimitive(PrimitiveType.Cube,     "Chamber_Base",  new Vector3(0, 0.8f, 0), new Vector3(2.4f, 1.6f, 1.6f), metal, root.transform);
            CreatePrimitive(PrimitiveType.Cube,     "Chamber_Glass", new Vector3(0, 1.1f, 0.81f), new Vector3(2.2f, 1.2f, 0.08f), glass, root.transform, false, false);
            CreatePrimitive(PrimitiveType.Cylinder, "Chamber_Core",  new Vector3(0, 1.1f, 0), new Vector3(0.7f, 0.6f, 0.7f), metal, root.transform);
        }

        private void CreateFlowHood(Vector3 position, MuseumMaterials mats, Transform parent)
        {
            var metal = CreateLabMetal(mats, 0.5f, 0.35f);
            var glass = CreateMaterial(mats ? mats.glassColor : new Color(0.5f, 0.8f, 1f, 0.35f), 0.9f, true);
            var glow  = CreateEmissiveMaterial(new Color(0.7f, 0.9f, 1f), 0.8f, false, new Color(0.7f, 0.9f, 1f), 2.5f);

            var root = new GameObject("Flow_Hood");
            root.transform.SetParent(parent, false);
            root.transform.localPosition = position;

            CreatePrimitive(PrimitiveType.Cube, "Hood_Base",  new Vector3(0, 1.1f, 0),    new Vector3(2.6f, 2.2f, 1.6f), metal, root.transform);
            CreatePrimitive(PrimitiveType.Cube, "Hood_Glass", new Vector3(0, 1.2f, 0.81f), new Vector3(2.4f, 1.4f, 0.08f), glass, root.transform, false, false);
            CreatePrimitive(PrimitiveType.Cube, "Hood_Light", new Vector3(0, 2.0f, 0),    new Vector3(2.2f, 0.1f, 1.2f), glow, root.transform, false, false);
        }

        private void CreateNanoDrops(Vector3 position, MuseumMaterials mats, Transform parent)
        {
            var root = new GameObject("NanoDrops_Station");
            root.transform.SetParent(parent, false);
            root.transform.localPosition = position;

            if (mats && mats.prefabMicroscope)
            {
                var instance = Instantiate(mats.prefabMicroscope, root.transform, false);
                instance.transform.localPosition = Vector3.zero;
                return;
            }

            var metal  = CreateLabMetal(mats, 0.5f, 0.4f);
            var accent = CreateLabSurface(mats, 0.4f, 0.3f);

            CreatePrimitive(PrimitiveType.Cube,     "NanoDrops_Base",   new Vector3(0, 0.4f, 0),       new Vector3(1.6f, 0.8f, 1.1f), metal, root.transform);
            CreatePrimitive(PrimitiveType.Cube,     "NanoDrops_Screen", new Vector3(0, 0.9f, -0.45f),  new Vector3(1.2f, 0.5f, 0.08f), accent, root.transform, false, false);
            CreatePrimitive(PrimitiveType.Cylinder, "NanoDrops_Optic",  new Vector3(0.45f, 0.95f, 0.2f), new Vector3(0.18f, 0.15f, 0.18f), metal, root.transform);
        }

        private void CreateSink(Vector3 position, MuseumMaterials mats, Transform parent)
        {
            var metal = CreateLabMetal(mats, 0.5f, 0.4f);
            var glass = CreateMaterial(mats ? mats.glassColor : new Color(0.5f, 0.8f, 1f, 0.35f), 0.9f, true);

            var root = new GameObject("Lab_Sink");
            root.transform.SetParent(parent, false);
            root.transform.localPosition = position;

            CreatePrimitive(PrimitiveType.Cube,     "Sink_Base",   new Vector3(0, 0.45f, 0),      new Vector3(2.2f, 0.9f, 1.2f), metal, root.transform);
            CreatePrimitive(PrimitiveType.Cylinder, "Sink_Basin",  new Vector3(-0.5f, 0.75f, 0),  new Vector3(0.6f, 0.2f, 0.6f), glass, root.transform, false, false);
            CreatePrimitive(PrimitiveType.Cylinder, "Sink_Faucet", new Vector3(0.6f, 1.0f, 0),    new Vector3(0.08f, 0.35f, 0.08f), metal, root.transform);
            CreatePrimitive(PrimitiveType.Cube,     "Sink_Spout",  new Vector3(0.6f, 1.25f, 0.25f), new Vector3(0.06f, 0.06f, 0.5f), metal, root.transform);
        }

        private void CreateBioSafetySign(Vector3 position, MuseumMaterials mats, Transform parent)
        {
            var panelMat = CreateMaterial(mats ? mats.bioSafetyColor : new Color(1f, 0.8f, 0.1f, 1f), 0.3f, false);
            CreatePrimitive(PrimitiveType.Cube, "BioSafety_Sign", position, new Vector3(1.6f, 0.8f, 0.08f), panelMat, parent, false, false);
            CreateTextMesh("BioSafety_Text", "BIOSEGURIDAD\nEPP OBLIGATORIO",
                position + new Vector3(0, 0, 0.06f), new Vector3(0, 180f, 0),
                0.04f, 60, Color.black, mats ? mats.displayFont : null, FontStyle.Bold, parent);
        }

        // ── Material Helpers ─────────────────────────────────────────────

        private Material CreateLabMetal(MuseumMaterials mats, float smoothness, float metallic)
        {
            var tex = mats ? mats.labMetal : null;
            Vector2? scale = tex ? new Vector2(metalTile, metalTile) : (Vector2?)null;
            return CreateMaterial(mats ? mats.labMetalColor : new Color(0.74f, 0.78f, 0.82f, 1f), smoothness, false, tex, scale, metallic);
        }

        private Material CreateLabSurface(MuseumMaterials mats, float smoothness, float metallic)
        {
            var tex = mats ? mats.labSurface : null;
            Vector2? scale = tex ? new Vector2(surfaceTile, surfaceTile) : (Vector2?)null;
            return CreateMaterial(mats ? mats.labAccentColor : new Color(0.2f, 0.55f, 0.85f, 1f), smoothness, false, tex, scale, metallic);
        }

        // ── Manual UI ────────────────────────────────────────────────────

        private Text CreateManual(Transform parent, Vector3 position, Vector3 rotation)
        {
            var canvasGo = new GameObject("Lab_Manual");
            canvasGo.transform.SetParent(parent, false);
            canvasGo.transform.localPosition = position;
            canvasGo.transform.localRotation = Quaternion.Euler(rotation);
            canvasGo.transform.localScale    = new Vector3(manualScale, manualScale, manualScale);

            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            var scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 30f; // Mejor calidad de texto
            canvasGo.AddComponent<GraphicRaycaster>();

            var panel = new GameObject("Panel");
            panel.transform.SetParent(canvasGo.transform, false);
            var panelRect = panel.AddComponent<RectTransform>();
            panelRect.sizeDelta = manualSize;
            panel.AddComponent<Image>().color = manualPanelColor;

            var textGo = new GameObject("Manual_Text");
            textGo.transform.SetParent(panel.transform, false);
            var text = textGo.AddComponent<Text>();
            text.font               = manualFont ? manualFont : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize           = 26;
            text.color              = manualTextColor;
            text.alignment          = TextAnchor.UpperLeft;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow   = VerticalWrapMode.Overflow;

            var textRect        = textGo.GetComponent<RectTransform>();
            textRect.anchorMin  = Vector2.zero;
            textRect.anchorMax  = Vector2.one;
            textRect.offsetMin  = new Vector2(24, 24);
            textRect.offsetMax  = new Vector2(-24, -24);

            return text;
        }

        private Transform CreateTaskTrigger(Transform parent, string name, Vector3 position, Vector3 size)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            var col       = go.AddComponent<BoxCollider>();
            col.isTrigger = true;
            col.size      = size;
            return go.transform;
        }

        // ── Bot Guía ─────────────────────────────────────────────────────

        private GameObject CreateLabBot(Transform parent, Vector3 position, MuseumMaterials mats)
        {
            var root = new GameObject("Lab_Bot");
            root.transform.SetParent(parent, false);
            root.transform.localPosition = position;

            var bodyMat   = CreateMaterial(new Color(0.12f, 0.14f, 0.18f, 1f), 0.6f, false, null, null, 0.2f);
            var coatMat   = CreateMaterial(Color.white, 0.3f, false, null, null, 0.1f);
            var accentMat = CreateMaterial(mats ? mats.labAccentColor : new Color(0.2f, 0.55f, 0.85f, 1f), 0.5f, false, null, null, 0.25f);

            CreatePrimitive(PrimitiveType.Capsule, "Bot_Body",  new Vector3(0, 0.9f, 0),           new Vector3(0.6f, 1.2f, 0.6f),  bodyMat,   root.transform);
            CreatePrimitive(PrimitiveType.Cube,    "Bot_Coat",  new Vector3(0, 0.7f, 0),           new Vector3(0.7f, 1.0f, 0.4f),  coatMat,   root.transform);
            CreatePrimitive(PrimitiveType.Sphere,  "Bot_Head",  new Vector3(0, 1.6f, 0),           new Vector3(0.4f, 0.4f, 0.4f),  bodyMat,   root.transform);
            CreatePrimitive(PrimitiveType.Cube,    "Bot_Badge", new Vector3(0.22f, 1.08f, 0.23f),  new Vector3(0.12f, 0.16f, 0.02f), accentMat, root.transform, false, false);

            CreateTextMesh("Bot_Bubble", "Siguiente: ...",
                new Vector3(0, 2.1f, 0), new Vector3(0, 180f, 0),
                0.04f, 48, Color.white, mats ? mats.bodyFont : null, FontStyle.Bold, root.transform,
                TextAnchor.LowerCenter, TextAlignment.Center, 1f, true,
                mats ? mats.holoEmissionColor : Color.cyan, mats ? mats.holoEmissionIntensity : 2.2f);
            return root;
        }

        private static GameObject FindChildByName(Transform root, string name)
        {
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
                if (child.name == name) return child.gameObject;
            return null;
        }
    }

    // ══════════════════════════════════════════════════════════════════════
    // Clases auxiliares del Laboratorio
    // ══════════════════════════════════════════════════════════════════════

    /// <summary>Puerta de cristal que se abre al acercarse el usuario.</summary>
    public class AutoDoor : MonoBehaviour
    {
        public Transform user;
        public Vector3   openOffset   = new Vector3(0, 0, 1.6f);
        public float     openDistance  = 2.8f;
        public float     openSpeed    = 2.5f;

        private Vector3 closedPos;
        private Vector3 openPos;

        private void Start()
        {
            closedPos = transform.localPosition;
            openPos   = closedPos + openOffset;
            if (!user && Camera.main) user = Camera.main.transform;
        }

        private void Update()
        {
            if (!user) return;
            float dist = Vector3.Distance(user.position, transform.position);
            Vector3 target = dist <= openDistance ? openPos : closedPos;
            transform.localPosition = Vector3.Lerp(transform.localPosition, target, Time.deltaTime * openSpeed);
        }
    }

    /// <summary>Gestor de tareas guiadas del laboratorio.</summary>
    public class LabTaskManager : MonoBehaviour
    {
        public Text            manualText;
        public string          title = "Manual de Bioseguridad";
        public Transform       userRoot;
        public List<LabTask>   tasks = new List<LabTask>();

        public void RegisterTask(Transform trigger, string label, string instruction)
        {
            var task = new LabTask { label = label, instruction = instruction };
            tasks.Add(task);
            var comp         = trigger.gameObject.AddComponent<LabTaskTrigger>();
            comp.manager     = this;
            comp.taskIndex   = tasks.Count - 1;
            comp.userRoot    = userRoot;
            RefreshManual();
        }

        public void CompleteTask(int index)
        {
            if (index < 0 || index >= tasks.Count || tasks[index].completed) return;
            tasks[index].completed = true;
            RefreshManual();
        }

        public string GetCurrentInstruction()
        {
            for (int i = 0; i < tasks.Count; i++)
                if (!tasks[i].completed) return tasks[i].instruction;
            return "Todas las tareas completadas.";
        }

        public void RefreshManual()
        {
            if (!manualText) return;
            var sb = new StringBuilder();
            sb.AppendLine(title);
            sb.AppendLine("--------------------------------");
            for (int i = 0; i < tasks.Count; i++)
                sb.AppendLine((tasks[i].completed ? "[x] " : "[ ] ") + tasks[i].label);
            sb.AppendLine();
            sb.AppendLine("Siguiente: " + GetCurrentInstruction());
            manualText.text = sb.ToString();
        }
    }

    public class LabTask { public string label; public string instruction; public bool completed; }

    /// <summary>Trigger que completa una tarea al entrar el usuario.</summary>
    public class LabTaskTrigger : MonoBehaviour
    {
        public LabTaskManager manager;
        public int            taskIndex;
        public Transform      userRoot;

        private void OnTriggerEnter(Collider other)
        {
            if (!manager) return;
            if (userRoot) { if (!other.transform.IsChildOf(userRoot)) return; }
            else { if (!other.CompareTag("Player") && !other.GetComponentInParent<Camera>()) return; }
            manager.CompleteTask(taskIndex);
        }
    }

    /// <summary>Bot guía que sigue al usuario y muestra instrucciones.</summary>
    public class LabBotFollower : MonoBehaviour
    {
        public Transform      user;
        public LabTaskManager  taskManager;
        public Transform      bubble;
        public float          followDistance = 2.1f;
        public float          moveSpeed     = 2.2f;
        public float          turnSpeed     = 6f;

        private void Update()
        {
            if (!user) return;
            Vector3 target = user.position - user.forward * followDistance;
            target.y = transform.position.y;
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * moveSpeed);

            Vector3 look = user.position;
            look.y = transform.position.y;
            Vector3 dir = (look - transform.position).normalized;
            if (dir.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * turnSpeed);

            if (bubble)
            {
                var toUser = (user.position - bubble.position).normalized;
                if (toUser.sqrMagnitude > 0.001f)
                    bubble.rotation = Quaternion.LookRotation(toUser);
            }
            if (taskManager)
            {
                var tm = bubble ? bubble.GetComponent<TextMesh>() : null;
                if (tm) tm.text = "Siguiente: " + taskManager.GetCurrentInstruction();
            }
        }
    }
}
