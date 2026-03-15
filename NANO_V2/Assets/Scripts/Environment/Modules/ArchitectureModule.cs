// ═══════════════════════════════════════════════════════════════════════════
// ArchitectureModule.cs — Estructura Física del Museo
// ═══════════════════════════════════════════════════════════════════════════
// Genera proceduralmente: pisos (alfombra + lab), 4 paredes de mármol,
// muro divisorio con puerta de cristal, 12 columnas decorativas y un
// teatro completo con escenario, escalones, arco proscenio y cortinas.
// ═══════════════════════════════════════════════════════════════════════════

using UnityEngine;

namespace VRNanoProject.Environment
{
    public class ArchitectureModule : EnvironmentModule
    {
        [Header("Layout")]
        public float dividerZ         = 18f;
        public float dividerGap       = 9f;
        public float columnStartZ     = -34f;
        public float columnSpacing    = 10f;
        public int   columnPairs      = 6;
        public float wallThickness    = 0.8f;
        public float dividerThickness = 0.6f;

        [Header("Floor")]
        public float carpetTile = 3.2f;
        public float labTile    = 2.4f;

        [Header("Theater")]
        public bool  enableTheater         = true;
        public float stageDepth            = 10f;
        public float stageWidth            = 26f;
        public float stageHeight           = 0.6f;
        public float stageZOffset          = 12f;
        public int   stageSteps            = 3;
        public float stepDepth             = 1.2f;
        public float prosceniumHeight      = 6.2f;
        public float prosceniumThickness   = 0.7f;
        public float prosceniumInset       = 2.5f;

        public override void Build()
        {
            float w    = master ? master.anchuraSalon  : 40f;
            float l    = master ? master.longitudSalon : 60f;
            float h    = master ? master.alturaParedes : 6f;
            var   mats = master ? master.materials     : null;

            // ── Pisos ────────────────────────────────────────────────────
            float carpetStartZ  = -l / 2f;
            float carpetEndZ    = dividerZ;
            float carpetLength  = Mathf.Max(10f, carpetEndZ - carpetStartZ);
            float carpetCenterZ = (carpetStartZ + carpetEndZ) * 0.5f;

            float labStartZ  = dividerZ;
            float labEndZ    = l / 2f;
            float labLength  = Mathf.Max(8f, labEndZ - labStartZ);
            float labCenterZ = (labStartZ + labEndZ) * 0.5f;

            Color carpetTint = mats ? mats.carpetTint : new Color(0.12f, 0.1f, 0.12f, 1f);
            CreateFloor("Suelo_Alfombra_Museo", new Vector3(0, 0, carpetCenterZ),
                new Vector3(w, 1, carpetLength), mats ? mats.alfombra : null, carpetTile, 0.18f, carpetTint);

            Texture2D labFloorTex = mats && mats.labFloor ? mats.labFloor : (mats ? mats.marmol : null);
            CreateFloor("Suelo_Lab", new Vector3(0, 0, labCenterZ),
                new Vector3(w, 1, labLength), labFloorTex, labTile, 0.5f, Color.white);

            // ── Paredes ──────────────────────────────────────────────────
            float tilingLateral = l / 5f;
            float tilingFrontal = w / 5f;

            CreateWall("Pared_Fondo",     new Vector3(0, h / 2f,  l / 2f),  new Vector3(w, h, wallThickness), mats ? mats.marmol : null, tilingFrontal);
            CreateWall("Pared_Entrada",   new Vector3(0, h / 2f, -l / 2f),  new Vector3(w, h, wallThickness), mats ? mats.marmol : null, tilingFrontal);
            CreateWall("Pared_Izquierda", new Vector3(-w / 2f, h / 2f, 0),  new Vector3(wallThickness, h, l), mats ? mats.marmol : null, tilingLateral);
            CreateWall("Pared_Derecha",   new Vector3( w / 2f, h / 2f, 0),  new Vector3(wallThickness, h, l), mats ? mats.marmol : null, tilingLateral);

            // ── Muro Divisorio + Puerta ──────────────────────────────────
            float pWidth = (w - dividerGap) / 2f;
            CreateWall("Muro_Div_Izq",  new Vector3(-(w / 2f - pWidth / 2f), h / 2f, dividerZ), new Vector3(pWidth, h, dividerThickness), mats ? mats.marmol : null, pWidth / 5f);
            CreateWall("Muro_Div_Der",  new Vector3( (w / 2f - pWidth / 2f), h / 2f, dividerZ), new Vector3(pWidth, h, dividerThickness), mats ? mats.marmol : null, pWidth / 5f);
            CreateWall("Muro_Dintel",   new Vector3(0, h - 0.75f, dividerZ), new Vector3(dividerGap, 1.5f, dividerThickness), mats ? mats.marmol : null, 1f);

            var doorMat = CreateMaterial(mats ? mats.glassColor : new Color(0.3f, 0.8f, 1f, 0.3f), 1f, true);
            CreatePrimitive(PrimitiveType.Cube, "Puerta_Cristal_Lab", new Vector3(0, 2.25f, dividerZ), new Vector3(dividerGap, 4.5f, 0.1f), doorMat);

            // ── Columnas ─────────────────────────────────────────────────
            for (int i = 0; i < columnPairs; i++)
            {
                float columnZ = columnStartZ + (i * columnSpacing);
                CreateColumn(new Vector3(-w / 2f + 1.25f, h / 2f, columnZ), mats ? mats.marmol : null);
                CreateColumn(new Vector3( w / 2f - 1.25f, h / 2f, columnZ), mats ? mats.marmol : null);
            }

            // ── Teatro ───────────────────────────────────────────────────
            if (enableTheater) CreateTheater(w, h, mats);
        }

        // ── Helpers ──────────────────────────────────────────────────────

        private void CreateFloor(string name, Vector3 position, Vector3 size,
            Texture2D texture, float tile, float smoothness, Color tint)
        {
            var mat = CreateMaterial(tint, smoothness, false, texture,
                new Vector2(tile * (size.x / 10f), tile * (size.z / 10f)));
            CreatePrimitive(PrimitiveType.Plane, name, position,
                new Vector3(size.x / 10f, 1, size.z / 10f), mat);
        }

        private void CreateWall(string name, Vector3 position, Vector3 size,
            Texture2D texture, float tile)
        {
            var mat = CreateMaterial(Color.white, 0.15f, false, texture, new Vector2(tile, 1f));
            CreatePrimitive(PrimitiveType.Cube, name, position, size, mat);
        }

        private void CreateColumn(Vector3 position, Texture2D texture)
        {
            var mat = CreateMaterial(Color.white, 0.25f, false, texture, new Vector2(1f, 1f));
            CreatePrimitive(PrimitiveType.Cube, "Columna", position,
                new Vector3(2.5f, master ? master.alturaParedes : 6f, 2.5f), mat);
        }

        private void CreateTheater(float width, float height, MuseumMaterials mats)
        {
            float stageW = Mathf.Min(stageWidth, width - 8f);
            float stageZ = dividerZ + stageZOffset;
            float stageY = stageHeight / 2f;

            var stageMat = CreateMaterial(
                mats ? mats.stageTrimColor : new Color(0.82f, 0.74f, 0.6f, 1f),
                0.35f, false, mats ? mats.madera : null, null, 0.1f);
            CreatePrimitive(PrimitiveType.Cube, "Escenario_Plataforma",
                new Vector3(0, stageY, stageZ), new Vector3(stageW, stageHeight, stageDepth), stageMat);

            // Escalones
            float stepHeight = Mathf.Max(0.12f, stageHeight / (stageSteps + 1f));
            for (int i = 0; i < stageSteps; i++)
            {
                float stepZ = stageZ - (stageDepth / 2f) - (i + 0.5f) * stepDepth;
                float stepY = (i + 1) * stepHeight * 0.5f;
                float stepW = stageW - (i * 1.4f);
                CreatePrimitive(PrimitiveType.Cube, "Escenario_Escalon",
                    new Vector3(0, stepY, stepZ), new Vector3(stepW, stepHeight, stepDepth), stageMat);
            }

            // Arco proscenio
            float archHeight = Mathf.Min(prosceniumHeight, height - 0.8f);
            float archZ      = stageZ + (stageDepth / 2f) - (prosceniumThickness / 2f);
            float archW      = stageW + (prosceniumInset * 2f);

            var curtainMat = CreateMaterial(
                mats ? mats.stageCurtainColor : new Color(0.35f, 0.05f, 0.08f, 1f),
                0.2f, false, null, null, 0.05f);
            CreatePrimitive(PrimitiveType.Cube, "Escenario_Cortina",
                new Vector3(0, archHeight * 0.55f, archZ - 0.1f),
                new Vector3(archW - 2f, archHeight * 0.9f, 0.15f), curtainMat, null, false, false);

            var archMat = CreateMaterial(
                mats ? mats.stageTrimColor : new Color(0.82f, 0.74f, 0.6f, 1f),
                0.4f, false, mats ? mats.madera : null, null, 0.15f);
            float postX = archW / 2f - 0.6f;
            CreatePrimitive(PrimitiveType.Cube, "Escenario_Arco_Izq",
                new Vector3(-postX, archHeight / 2f, archZ), new Vector3(1.1f, archHeight, prosceniumThickness), archMat);
            CreatePrimitive(PrimitiveType.Cube, "Escenario_Arco_Der",
                new Vector3( postX, archHeight / 2f, archZ), new Vector3(1.1f, archHeight, prosceniumThickness), archMat);
            CreatePrimitive(PrimitiveType.Cube, "Escenario_Arco_Superior",
                new Vector3(0, archHeight - 0.4f, archZ), new Vector3(archW, 0.8f, prosceniumThickness), archMat);
        }
    }
}
