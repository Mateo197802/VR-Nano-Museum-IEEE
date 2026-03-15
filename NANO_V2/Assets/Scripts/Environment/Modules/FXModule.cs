// ═══════════════════════════════════════════════════════════════════════════
// FXModule.cs — Efectos Visuales: Nanopartículas Animadas
// ═══════════════════════════════════════════════════════════════════════════
// Genera clusters 3D de nanopartículas con:
//   • Núcleo emisivo con brillo pulsante
//   • Aura transparente que respira
//   • 80 átomos flotantes con drift browniano
//   • 10 orbitadores con rotación multi-eje
//   • Punto de luz coloreado
//   • Movimiento jittery del cluster completo
// ═══════════════════════════════════════════════════════════════════════════

using UnityEngine;

namespace VRNanoProject.Environment
{
    public class FXModule : EnvironmentModule
    {
        [Header("Layout")]
        public bool  autoLayout         = true;
        public float rightX             = 12f;
        public float y                  = 2.9f;
        public float startZ             = -20.5f;
        public float spacing            = 9f;
        public int   count              = 0;
        public bool  useAllContent      = true;
        public float sidePadding        = 6.5f;
        public float startPadding       = 12f;
        public float endPadding         = 28f;
        public bool  clampToMuseumZone  = true;
        public float museumStartPadding = 8f;
        public float museumEndPadding   = 2.5f;
        public bool  alignToColumns     = true;

        [Header("Nanoparticles")]
        public int   atomsPerCluster    = 80;
        public int   orbitersPerCluster = 10;
        public float clusterRadius      = 1.6f;
        public float coreRadius         = 0.32f;
        public Vector2 atomScaleRange   = new Vector2(0.06f, 0.22f);
        public float emissionIntensity  = 6.5f;
        public float auraScale          = 2.4f;
        public float auraEmission       = 1.6f;
        public float lightRange         = 6.5f;
        public float lightIntensity     = 4.2f;
        public float orbitSpeed         = 20f;
        public float tiltSpeed          = 12f;
        public float pulseSpeed         = 1.2f;
        public float pulseAmount        = 0.08f;
        public bool  animateAtoms       = true;
        public float atomDriftRadius    = 0.06f;
        public float atomDriftSpeed     = 1.6f;
        public float clusterMoveRadius  = 0.9f;
        public float clusterMoveSpeed   = 2.4f;
        public float clusterBobHeight   = 0.18f;
        public float clusterBobSpeed    = 1.2f;

        public override void Build()
        {
            var mats = master ? master.materials : null;
            var data = MuseumContentDefaults.Get(master ? master.content : null);

            Color[] palette =
            {
                new Color(0.35f, 0.9f, 1f),
                new Color(0.95f, 0.3f, 0.65f),
                new Color(0.35f, 1f, 0.65f),
                new Color(1f, 0.55f, 0.25f),
                new Color(0.6f, 0.45f, 1f),
                new Color(0.9f, 0.95f, 0.3f)
            };

            float w = master ? master.anchuraSalon  : 40f;
            float l = master ? master.longitudSalon : 60f;
            float localRightX  = autoLayout ? (w / 2f - sidePadding) : rightX;
            float localStartZ  = startZ;
            float localSpacing = spacing;
            int available      = data.nanoTopics != null ? data.nanoTopics.Length : 0;
            int localCount     = available;

            if (!useAllContent)
            {
                localCount = count > 0 ? count : available;
                if (available > 0) localCount = Mathf.Min(localCount, available);
            }
            if (localCount <= 0) return;

            if (autoLayout)
            {
                float zoneStart = -l / 2f + startPadding;
                float zoneEnd   =  l / 2f - endPadding;
                var arch        = master ? master.GetComponentInChildren<ArchitectureModule>() : null;
                float dividerZ  = arch ? arch.dividerZ : l * 0.2f;

                if (clampToMuseumZone)
                {
                    zoneStart = -l / 2f + museumStartPadding;
                    zoneEnd   = dividerZ - museumEndPadding;
                }
                if (zoneEnd < zoneStart) zoneEnd = zoneStart;

                float[] aligned = null;
                if (alignToColumns && arch && localCount > 0)
                    LayoutUtils.TryGetColumnAlignedPositions(localCount, zoneStart, zoneEnd, arch, out aligned);

                if (aligned != null)
                {
                    localStartZ  = aligned[0];
                    localSpacing = aligned.Length > 1 ? (aligned[aligned.Length - 1] - aligned[0]) / (aligned.Length - 1) : 0f;
                }
                else
                {
                    localStartZ  = zoneStart;
                    localSpacing = localCount > 1 ? (zoneEnd - zoneStart) / (localCount - 1) : 0f;
                }
            }

            float[] zPositions = null;
            if (autoLayout && alignToColumns)
            {
                var arch = master ? master.GetComponentInChildren<ArchitectureModule>() : null;
                float lZoneStart = -l / 2f + (clampToMuseumZone ? museumStartPadding : startPadding);
                float lZoneEnd   = (arch ? arch.dividerZ : l * 0.2f) - (clampToMuseumZone ? museumEndPadding : endPadding);
                LayoutUtils.TryGetColumnAlignedPositions(localCount, lZoneStart, lZoneEnd, arch, out zPositions);
            }

            for (int i = 0; i < localCount; i++)
            {
                float z     = zPositions != null ? zPositions[i] : (localStartZ + (i * localSpacing));
                var   color = palette[i % palette.Length];
                CreateCluster(new Vector3(localRightX, y, z), color, mats);
            }
        }

        private void CreateCluster(Vector3 position, Color color, MuseumMaterials mats)
        {
            var container = new GameObject("Nano_Cluster");
            container.transform.SetParent(transform, false);
            container.transform.localPosition = position;
            container.isStatic = false;

            var jitter          = container.AddComponent<JitteryMotionV11>();
            jitter.radius       = clusterMoveRadius;
            jitter.retargetTime = 0.9f;
            jitter.moveSpeed    = clusterMoveSpeed;
            jitter.rotationSpeed = 28f;
            jitter.bobHeight    = clusterBobHeight;
            jitter.bobSpeed     = clusterBobSpeed;
            jitter.seed         = Random.Range(0f, 10f);

            // Núcleo
            var coreMat = CreateEmissiveMaterial(color, 0.9f, false, color, emissionIntensity, null, null, 0.35f);
            CreatePrimitive(PrimitiveType.Sphere, "Core", Vector3.zero, Vector3.one * coreRadius, coreMat, container.transform);

            // Aura
            var auraColor = mats ? mats.nanoAuraColor : new Color(color.r, color.g, color.b, 0.18f);
            var auraMat   = CreateEmissiveMaterial(auraColor, 0.9f, true, color, auraEmission);
            var aura      = CreatePrimitive(PrimitiveType.Sphere, "Aura", Vector3.zero, Vector3.one * auraScale, auraMat, container.transform, false, false);

            // Átomos
            var atomMat = CreateEmissiveMaterial(color, 0.85f, true, color, emissionIntensity * 0.9f);
            for (int i = 0; i < atomsPerCluster; i++)
            {
                var atom = CreatePrimitive(PrimitiveType.Sphere, "Atom", Vector3.zero, Vector3.one, atomMat, container.transform, false, false);
                atom.isStatic = false;
                atom.transform.localPosition = Random.insideUnitSphere * clusterRadius;
                float scale = Random.Range(atomScaleRange.x, atomScaleRange.y);
                atom.transform.localScale = Vector3.one * scale;
                if (animateAtoms)
                {
                    var drift          = atom.AddComponent<NanoAtomDrift>();
                    drift.radius       = atomDriftRadius * Random.Range(0.6f, 1.2f);
                    drift.retargetTime = Random.Range(0.6f, 1.6f);
                    drift.moveSpeed    = atomDriftSpeed * Random.Range(0.8f, 1.4f);
                }
            }

            // Orbitadores
            var orbitRoot = new GameObject("Orbiters");
            orbitRoot.transform.SetParent(container.transform, false);
            var orbiterMat = CreateEmissiveMaterial(Color.white, 0.9f, true, color, emissionIntensity * 1.2f);
            for (int i = 0; i < orbitersPerCluster; i++)
            {
                var orb = CreatePrimitive(PrimitiveType.Sphere, "Orbiter", Vector3.zero, Vector3.one, orbiterMat, orbitRoot.transform, false, false);
                orb.isStatic = false;
                orb.transform.localPosition = Random.onUnitSphere * (clusterRadius + 0.4f);
                orb.transform.localScale    = Vector3.one * Random.Range(0.05f, 0.12f);
            }

            // Luz puntual
            var lightGo = new GameObject("Nano_Light");
            lightGo.transform.SetParent(container.transform, false);
            var light       = lightGo.AddComponent<Light>();
            light.type      = LightType.Point;
            light.range     = lightRange;
            light.intensity = lightIntensity;
            light.color     = mats ? mats.nanoLightColor : color;
            light.shadows   = LightShadows.None;

            // Animador
            var anim          = container.AddComponent<NanoClusterAnimator>();
            anim.orbitersRoot = orbitRoot.transform;
            anim.aura         = aura.transform;
            anim.clusterLight = light;
            anim.orbitSpeed   = orbitSpeed;
            anim.tiltSpeed    = tiltSpeed;
            anim.pulseSpeed   = pulseSpeed;
            anim.pulseAmount  = pulseAmount;
        }
    }

    // ── Animadores de Nanopartículas ──────────────────────────────────────

    /// <summary>Anima la rotación de orbitadores, el pulso del aura y la luz.</summary>
    public class NanoClusterAnimator : MonoBehaviour
    {
        public Transform orbitersRoot;
        public Transform aura;
        public Light     clusterLight;
        public float     orbitSpeed  = 20f;
        public float     tiltSpeed   = 12f;
        public float     pulseSpeed  = 1.2f;
        public float     pulseAmount = 0.08f;
        public float     lightPulse  = 0.15f;

        private Vector3 auraBaseScale = Vector3.one;
        private float   baseLightIntensity;

        private void Start()
        {
            if (aura)         auraBaseScale       = aura.localScale;
            if (clusterLight) baseLightIntensity   = clusterLight.intensity;
        }

        private void Update()
        {
            if (orbitersRoot)
            {
                orbitersRoot.Rotate(Vector3.up,    orbitSpeed * Time.deltaTime, Space.Self);
                orbitersRoot.Rotate(Vector3.right, tiltSpeed  * Time.deltaTime, Space.Self);
            }
            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
            if (aura)         aura.localScale        = auraBaseScale * pulse;
            if (clusterLight) clusterLight.intensity = baseLightIntensity * (1f + Mathf.Sin(Time.time * (pulseSpeed * 1.3f)) * lightPulse);
        }
    }

    /// <summary>Utilidad compartida de alineamiento con columnas.</summary>
    internal static class LayoutUtils
    {
        internal static void TryGetColumnAlignedPositions(int count, float zoneStart, float zoneEnd,
            ArchitectureModule arch, out float[] positions)
        {
            positions = null;
            if (!arch || arch.columnPairs < 2 || count <= 0) return;

            float colSpacing = arch.columnSpacing;
            float first      = arch.columnStartZ + (colSpacing * 0.5f);
            int   slotCount  = Mathf.Max(1, arch.columnPairs - 1);
            var   slots      = new System.Collections.Generic.List<float>(slotCount);

            for (int i = 0; i < slotCount; i++)
            {
                float z = first + (i * colSpacing);
                if (z >= zoneStart && z <= zoneEnd) slots.Add(z);
            }
            if (slots.Count == 0 || count > slots.Count) return;

            positions = new float[count];
            if (count == 1) { positions[0] = slots[slots.Count / 2]; return; }

            float step = (slots.Count - 1f) / (count - 1f);
            for (int i = 0; i < count; i++)
            {
                int idx = Mathf.Clamp(Mathf.RoundToInt(i * step), 0, slots.Count - 1);
                positions[i] = slots[idx];
            }
        }
    }

    /// <summary>Movimiento orgánico jittery para clusters de nanopartículas.</summary>
    public class JitteryMotionV11 : MonoBehaviour
    {
        public float radius        = 0.6f;
        public float retargetTime  = 1.2f;
        public float moveSpeed     = 1.5f;
        public float rotationSpeed = 40f;
        public float bobHeight     = 0.12f;
        public float bobSpeed      = 1.2f;
        public float seed          = 0f;

        private Vector3 origin;
        private Vector3 targetOffset;
        private float   timer;

        private void Start()  { origin = transform.localPosition; PickNewTarget(); }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= retargetTime || (transform.localPosition - (origin + targetOffset)).sqrMagnitude < 0.01f)
                PickNewTarget();

            var targetPos = origin + targetOffset;
            var basePos   = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * moveSpeed);
            float bob     = Mathf.Sin(Time.time * bobSpeed + seed) * bobHeight;
            basePos.y     = origin.y + targetOffset.y + bob;
            transform.localPosition = basePos;
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
        }

        private void PickNewTarget() { timer = 0f; targetOffset = Random.insideUnitSphere * radius; }
    }

    /// <summary>Drift browniano individual para cada átomo.</summary>
    public class NanoAtomDrift : MonoBehaviour
    {
        public float radius       = 0.05f;
        public float retargetTime = 1.2f;
        public float moveSpeed    = 1.5f;

        private Vector3 baseLocalPos;
        private Vector3 targetOffset;
        private float   timer;

        private void Start()  { baseLocalPos = transform.localPosition; PickNewTarget(); }

        private void Update()
        {
            timer += Time.deltaTime;
            if (timer >= retargetTime || (transform.localPosition - (baseLocalPos + targetOffset)).sqrMagnitude < 0.0025f)
                PickNewTarget();
            transform.localPosition = Vector3.Lerp(transform.localPosition, baseLocalPos + targetOffset, Time.deltaTime * moveSpeed);
        }

        private void PickNewTarget() { timer = 0f; targetOffset = Random.insideUnitSphere * radius; }
    }
}
