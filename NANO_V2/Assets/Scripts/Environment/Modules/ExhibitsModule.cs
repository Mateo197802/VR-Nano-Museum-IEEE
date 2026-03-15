// ═══════════════════════════════════════════════════════════════════════════
// ExhibitsModule.cs — Exhibiciones del Museo (Hologramas + Pergaminos)
// ═══════════════════════════════════════════════════════════════════════════
// Genera un par de exhibiciones por cada científico/nanotema:
//   • Lado izquierdo: pedestal + hologramas emisivos + marco con foto
//   • Lado derecho: pedestal cilíndrico + pergamino con título y cuerpo
// Se alinea automáticamente a las columnas de ArchitectureModule.
// ═══════════════════════════════════════════════════════════════════════════

using UnityEngine;

namespace VRNanoProject.Environment
{
    public class ExhibitsModule : EnvironmentModule
    {
        [Header("Layout")]
        public bool  autoLayout          = true;
        public int   exhibitCount        = 0;
        public bool  useAllContent       = true;
        public float leftX               = -12f;
        public float rightX              = 12f;
        public float startZ              = -20.5f;
        public float spacing             = 9f;
        public float sidePadding         = 6.5f;
        public float startPadding        = 12f;
        public float endPadding          = 28f;
        public float textInset           = 0.6f;
        public float frameInset          = 0.7f;
        public bool  clampToMuseumZone   = true;
        public float museumStartPadding  = 8f;
        public float museumEndPadding    = 2.5f;
        public bool  alignToColumns      = true;

        [Header("Text Sizes")]
        public float nameTextSize       = 0.055f;
        public float bioTextSize        = 0.038f;
        public float parchmentTextSize  = 0.045f;
        public int   nameFontSize       = 110;
        public int   bioFontSize        = 80;
        public int   parchmentFontSize  = 88;
        public float bioLineSpacing     = 0.9f;
        public float parchmentLineSpacing = 0.95f;
        public int   bioMaxCharsPerLine = 26;
        public int   bioMaxLines        = 6;
        public float bioYOffset         = 0f;
        public float bioEmissionBoost   = 1.2f;

        [Header("Parchment Layout")]
        public Vector3 parchmentBaseSize       = new Vector3(0.22f, 4.9f, 4.6f);
        public Vector3 parchmentPaperSize      = new Vector3(0.02f, 4.4f, 4.1f);
        public Vector3 parchmentTextOffset     = new Vector3(-0.14f, 0f, 0f);
        public float   parchmentTitleTextSize  = 0.052f;
        public int     parchmentTitleFontSize  = 96;
        public float   parchmentTitleYOffset   = 1.2f;
        public float   parchmentBodyYOffset    = -0.15f;
        public int     parchmentMaxCharsPerLine = 24;
        public int     parchmentMaxBodyLines   = 6;
        public int     parchmentMaxTitleLines  = 2;

        [Header("Hologram Light")]
        public Vector3 bioGlowOffset    = new Vector3(0f, -0.45f, 0.08f);
        public float   bioGlowWidth     = 2.2f;
        public float   bioGlowHeight    = 0.65f;
        public float   bioLightRange    = 2.6f;
        public float   bioLightIntensity = 3.0f;

        public override void Build()
        {
            float w    = master ? master.anchuraSalon  : 40f;
            float l    = master ? master.longitudSalon : 60f;
            var   mats = master ? master.materials     : null;
            var   data = MuseumContentDefaults.Get(master ? master.content : null);

            int available = Mathf.Min(data.scientists.Length, data.nanoTopics.Length);
            if (available == 0) return;

            int displayCount = available;
            if (!useAllContent)
                displayCount = exhibitCount > 0 ? Mathf.Min(exhibitCount, available) : available;

            // ── Calcular Posiciones ──────────────────────────────────────
            float localLeftX   = leftX;
            float localRightX  = rightX;
            float localStartZ  = startZ;
            float localSpacing = spacing;

            if (autoLayout)
            {
                localLeftX  = -w / 2f + sidePadding;
                localRightX =  w / 2f - sidePadding;
                float zoneStart = -l / 2f + startPadding;
                float zoneEnd   =  l / 2f - endPadding;

                float dividerZ = 0f;
                var arch = master ? master.GetComponentInChildren<ArchitectureModule>() : null;
                dividerZ = arch ? arch.dividerZ : l * 0.2f;

                if (clampToMuseumZone)
                {
                    zoneStart = -l / 2f + museumStartPadding;
                    zoneEnd   = dividerZ - museumEndPadding;
                }
                if (zoneEnd < zoneStart) zoneEnd = zoneStart;

                float[] aligned = null;
                if (alignToColumns && arch && displayCount > 0)
                    TryGetColumnAlignedPositions(displayCount, zoneStart, zoneEnd, arch, out aligned);

                if (aligned != null)
                {
                    localStartZ  = aligned[0];
                    localSpacing = aligned.Length > 1
                        ? (aligned[aligned.Length - 1] - aligned[0]) / (aligned.Length - 1) : 0f;
                }
                else
                {
                    localStartZ  = zoneStart;
                    localSpacing = displayCount > 1 ? (zoneEnd - zoneStart) / (displayCount - 1) : 0f;
                }
            }

            // ── Posiciones finales ───────────────────────────────────────
            float[] zPositions = null;
            if (autoLayout && alignToColumns)
            {
                var arch = master ? master.GetComponentInChildren<ArchitectureModule>() : null;
                float lZoneStart = -l / 2f + (clampToMuseumZone ? museumStartPadding : startPadding);
                float lZoneEnd   = (arch ? arch.dividerZ : l * 0.2f) - (clampToMuseumZone ? museumEndPadding : endPadding);
                TryGetColumnAlignedPositions(displayCount, lZoneStart, lZoneEnd, arch, out zPositions);
            }

            // ── Generar Exhibiciones ─────────────────────────────────────
            for (int i = 0; i < displayCount; i++)
            {
                float z = zPositions != null ? zPositions[i] : (localStartZ + (i * localSpacing));
                var scientist = data.scientists[i];
                var topic     = data.nanoTopics[i];

                // Lado izquierdo: científico
                CreateStand(new Vector3(localLeftX, 0.6f, z), "Stand_Cientifico", false, mats);
                CreateHoloText(scientist.fullName, new Vector3(localLeftX + textInset, 3.5f, z), new Vector3(0, -90, 0), nameTextSize, true, mats);
                Vector3 bioPos = new Vector3(localLeftX + textInset, 2.45f + bioYOffset, z);
                CreateHoloText(scientist.bio, bioPos, new Vector3(0, -90, 0), bioTextSize, false, mats);
                CreateBioGlow(bioPos + bioGlowOffset, new Vector3(0, -90, 0), mats);
                CreateHoloGrid(new Vector3(localLeftX + textInset, 1.25f, z), mats);
                CreateFrame(new Vector3(-w / 2f + frameInset, 3.55f, z), scientist, mats, true);

                // Lado derecho: nanotema
                CreateStand(new Vector3(localRightX, 0.6f, z), "Stand_Nanoparticula", true, mats);
                CreateParchment(new Vector3(w / 2f - frameInset, 3.55f, z), topic, mats);
            }
        }

        public override System.Collections.Generic.List<string> Validate()
        {
            var warnings = new System.Collections.Generic.List<string>();
            var data = MuseumContentDefaults.Get(master ? master.content : null);
            if (data.scientists.Length == 0) warnings.Add("ExhibitsModule: no scientists configured.");
            if (data.nanoTopics.Length == 0) warnings.Add("ExhibitsModule: no nano topics configured.");
            return warnings;
        }

        // ── Constructores Privados ───────────────────────────────────────

        private void CreateStand(Vector3 position, string name, bool isCylinder, MuseumMaterials mats)
        {
            Vector3 scale = isCylinder ? new Vector3(2.8f, 0.6f, 2.8f) : new Vector3(2.4f, 1.25f, 3.6f);
            var mat = CreateMaterial(Color.white, 0.45f, false, mats ? mats.marmol : null);
            CreatePrimitive(isCylinder ? PrimitiveType.Cylinder : PrimitiveType.Cube, name, position, scale, mat);
        }

        private void CreateHoloText(string text, Vector3 position, Vector3 rotation, float size, bool bold, MuseumMaterials mats)
        {
            var textColor         = mats ? mats.holoTextColor     : Color.white;
            var emissionColor     = mats ? mats.holoEmissionColor : Color.cyan;
            float emissionIntens  = mats ? mats.holoEmissionIntensity : 2.5f;
            Font font             = bold ? (mats ? mats.displayFont : null) : (mats ? mats.bodyFont : null);
            int  fSize            = bold ? nameFontSize : bioFontSize;
            float lSpacing        = bold ? 1f : bioLineSpacing;
            string finalText      = bold ? text : FormatWrappedText(text, bioMaxCharsPerLine, bioMaxLines);

            CreateTextMesh("Texto_Holo", finalText, position, rotation, size, fSize,
                textColor, font, bold ? FontStyle.Bold : FontStyle.Normal, transform,
                TextAnchor.MiddleCenter, TextAlignment.Center, lSpacing,
                true, emissionColor, emissionIntens * (bold ? 1f : bioEmissionBoost));
        }

        private void CreateHoloGrid(Vector3 position, MuseumMaterials mats)
        {
            var gridColor     = mats ? mats.holoGridColor     : new Color(0f, 0.7f, 1f, 0.4f);
            var emissionColor = mats ? mats.holoEmissionColor : Color.cyan;
            float emIntens    = mats ? (mats.holoEmissionIntensity + 0.5f) : 3f;
            var mat = CreateEmissiveMaterial(gridColor, 1f, true, emissionColor, emIntens);
            CreatePrimitive(PrimitiveType.Plane, "Grid_Holo", position, new Vector3(0.4f, 1, 0.4f), mat, null, false, false);
        }

        private void CreateBioGlow(Vector3 position, Vector3 rotation, MuseumMaterials mats)
        {
            var glowColor = mats ? mats.holoBioLightColor : new Color(0.2f, 0.8f, 1f, 1f);
            var mat = CreateEmissiveMaterial(glowColor, 0.9f, true, glowColor, 2.8f);
            var quad = CreatePrimitive(PrimitiveType.Quad, "Bio_Glow", position,
                new Vector3(bioGlowWidth, bioGlowHeight, 1f), mat, null, false, false);
            quad.transform.localRotation = Quaternion.Euler(rotation);

            var lightGo = new GameObject("Bio_Light");
            lightGo.transform.SetParent(quad.transform, false);
            lightGo.transform.localPosition = new Vector3(0, 0, -0.08f);
            var light       = lightGo.AddComponent<Light>();
            light.type      = LightType.Spot;
            light.range     = bioLightRange;
            light.intensity = bioLightIntensity;
            light.spotAngle = 70f;
            light.color     = glowColor;
            light.shadows   = LightShadows.None;
        }

        private void CreateFrame(Vector3 position, ScientistEntry scientist, MuseumMaterials mats, bool leftSide)
        {
            const float frameThickness = 0.18f;
            const float photoThickness = 0.02f;
            var frameMat = CreateMaterial(Color.white, 0.7f, false, mats ? mats.madera : null);
            CreatePrimitive(PrimitiveType.Cube, "Marco", position, new Vector3(frameThickness, 3.2f, 2.5f), frameMat);

            var photo = scientist.photo ? scientist.photo : CreatePlaceholderPhoto(scientist.shortName);
            var photoMat = CreateMaterial(photo ? Color.white : Color.black, 0.1f, false, photo);
            float faceOffset = (frameThickness * 0.5f) - (photoThickness * 0.5f) - 0.002f;
            float dir = leftSide ? 1f : -1f;
            var photoPos = position + new Vector3(dir * faceOffset, 0, 0);
            CreatePrimitive(PrimitiveType.Cube, "Foto_" + scientist.shortName, photoPos,
                new Vector3(photoThickness, 2.8f, 2.1f), photoMat, null, false, false);
        }

        private void CreateParchment(Vector3 position, NanoTopic topic, MuseumMaterials mats)
        {
            var woodMat  = CreateMaterial(Color.white, 0.5f, false, mats ? mats.madera : null);
            CreatePrimitive(PrimitiveType.Cube, "Parchment_Base", position, parchmentBaseSize, woodMat);

            var paperMat = CreateMaterial(mats ? mats.paperColor : new Color(0.97f, 0.97f, 0.93f), 0.05f, false);
            CreatePrimitive(PrimitiveType.Cube, "Parchment_Paper", position - new Vector3(0.12f, 0, 0), parchmentPaperSize, paperMat);

            var titleFont = mats ? mats.displayFont : null;
            var bodyFont  = mats ? mats.bodyFont    : null;

            var titleTm = CreateTextMesh("Parchment_Title", topic.title,
                position + parchmentTextOffset + new Vector3(0, parchmentTitleYOffset, 0),
                new Vector3(0, 90, 0), parchmentTitleTextSize, parchmentTitleFontSize,
                Color.black, titleFont, FontStyle.Bold, transform);
            AdjustTextScale(titleTm, parchmentTitleTextSize, parchmentMaxCharsPerLine, parchmentMaxTitleLines);

            var bodyTm = CreateTextMesh("Parchment_Body", topic.description,
                position + parchmentTextOffset + new Vector3(0, parchmentBodyYOffset, 0),
                new Vector3(0, 90, 0), parchmentTextSize, parchmentFontSize,
                Color.black, bodyFont, FontStyle.Normal, transform,
                TextAnchor.MiddleCenter, TextAlignment.Center, parchmentLineSpacing);
            AdjustTextScale(bodyTm, parchmentTextSize, parchmentMaxCharsPerLine, parchmentMaxBodyLines);
        }

        // ── Utilidades de Texto ──────────────────────────────────────────

        private void AdjustTextScale(TextMesh tm, float baseSize, int maxCharsPerLine, int maxLines)
        {
            if (!tm) return;
            string text = tm.text ?? string.Empty;
            if (text.Length == 0) return;
            string[] lines = text.Split('\n');
            int longest = 0;
            for (int i = 0; i < lines.Length; i++)
                if (lines[i].Length > longest) longest = lines[i].Length;

            float scale = 1f;
            if (maxCharsPerLine > 0 && longest > maxCharsPerLine)
                scale = Mathf.Min(scale, (float)maxCharsPerLine / longest);
            if (maxLines > 0 && lines.Length > maxLines)
                scale = Mathf.Min(scale, (float)maxLines / lines.Length);
            tm.characterSize = baseSize * scale;
        }

        private string FormatWrappedText(string text, int maxCharsPerLine, int maxLines)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;
            if (maxCharsPerLine <= 0) return text;

            string normalized = text.Replace("\r\n", "\n").Replace("\r", "\n");
            var words = normalized.Split(' ');
            var lines = new System.Collections.Generic.List<string>();
            string current = string.Empty;

            foreach (var word in words)
            {
                if (word.Contains("\n"))
                {
                    var parts = word.Split('\n');
                    for (int i = 0; i < parts.Length; i++)
                    {
                        if (parts[i].Length > 0)
                        {
                            if ((current.Length + parts[i].Length + (current.Length > 0 ? 1 : 0)) > maxCharsPerLine)
                            {
                                if (current.Length > 0) lines.Add(current);
                                current = parts[i];
                            }
                            else
                            {
                                current = current.Length == 0 ? parts[i] : current + " " + parts[i];
                            }
                        }
                        if (i < parts.Length - 1)
                        {
                            if (current.Length > 0) lines.Add(current);
                            current = string.Empty;
                        }
                    }
                    continue;
                }
                if ((current.Length + word.Length + (current.Length > 0 ? 1 : 0)) > maxCharsPerLine)
                {
                    if (current.Length > 0) lines.Add(current);
                    current = word;
                }
                else
                {
                    current = current.Length == 0 ? word : current + " " + word;
                }
            }
            if (current.Length > 0) lines.Add(current);
            if (maxLines > 0 && lines.Count > maxLines)
                lines = lines.GetRange(0, maxLines);
            return string.Join("\n", lines);
        }

        // ── Placeholders ─────────────────────────────────────────────────

        private static readonly System.Collections.Generic.Dictionary<string, Texture2D> PlaceholderCache =
            new System.Collections.Generic.Dictionary<string, Texture2D>();

        private Texture2D CreatePlaceholderPhoto(string key)
        {
            key = string.IsNullOrWhiteSpace(key) ? "Scientist" : key;
            if (PlaceholderCache.TryGetValue(key, out var cached)) return cached;

            const int size = 256;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.wrapMode   = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;

            Color a = new Color(0.1f, 0.2f, 0.3f, 1f);
            Color b = new Color(0.35f, 0.55f, 0.7f, 1f);
            for (int y = 0; y < size; y++)
            {
                float t   = y / (float)(size - 1);
                Color row = Color.Lerp(a, b, t);
                for (int x = 0; x < size; x++)
                {
                    float stripe = ((x / 16) % 2 == 0) ? 0.9f : 1f;
                    tex.SetPixel(x, y, row * stripe);
                }
            }
            tex.Apply();
            tex.name = "Placeholder_" + key;
            PlaceholderCache[key] = tex;
            return tex;
        }

        // ── Column Alignment ─────────────────────────────────────────────

        private void TryGetColumnAlignedPositions(int count, float zoneStart, float zoneEnd,
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
}
