// ═══════════════════════════════════════════════════════════════════════════
// BannerModule.cs — Banner Oscilante de Bienvenida
// ═══════════════════════════════════════════════════════════════════════════
// Genera un banner de tela con texto emisivo, cuerdas decorativas y
// animación de balanceo suave (SwayingBannerV11).
// ═══════════════════════════════════════════════════════════════════════════

using UnityEngine;

namespace VRNanoProject.Environment
{
    public class BannerModule : EnvironmentModule
    {
        [Header("Layout")]
        public bool    autoLayout  = true;
        public Vector3 position    = new Vector3(0, 5.2f, 9.6f);
        public Vector3 clothScale  = new Vector3(14f, 3.2f, 0.15f);
        public float   ropeOffset  = 6.5f;
        public float   ropeHeight  = 1.0f;

        [Header("Text")]
        public Vector3 textOffset  = new Vector3(0, -0.4f, 0.1f);
        public float   textSize    = 0.08f;
        public int     fontSize    = 100;
        public float   textEmission = 1.2f;

        public override void Build()
        {
            var data = MuseumContentDefaults.Get(master ? master.content : null);
            var mats = master ? master.materials : null;

            var container = new GameObject("Banner_Final");
            container.transform.SetParent(transform, false);
            var targetPos = position;
            if (autoLayout && master)
            {
                float posY = Mathf.Max(4.8f, master.alturaParedes - 0.9f);
                float posZ = Mathf.Min(master.longitudSalon * 0.2f, 14f);
                targetPos  = new Vector3(0, posY, posZ);
            }
            container.transform.localPosition = targetPos;
            container.AddComponent<SwayingBannerV11>();

            // Tela
            var clothMat = CreateMaterial(mats ? mats.bannerColor : Color.black, 0.1f, false);
            CreatePrimitive(PrimitiveType.Cube, "Banner_Cloth", new Vector3(0, -0.4f, 0), clothScale, clothMat, container.transform);

            // Cuerdas
            CreateRope(new Vector3( ropeOffset, ropeHeight, 0), container.transform);
            CreateRope(new Vector3(-ropeOffset, ropeHeight, 0), container.transform);

            // Texto emisivo
            var textColor = mats ? mats.bannerTextColor : Color.white;
            var font      = mats ? mats.displayFont     : null;
            CreateTextMesh("Banner_Text", data.bannerText, textOffset, Vector3.zero,
                textSize, fontSize, textColor, font, FontStyle.Bold,
                container.transform, TextAnchor.MiddleCenter, TextAlignment.Center,
                1f, true, textColor, textEmission);
        }

        public override System.Collections.Generic.List<string> Validate()
        {
            var warnings = new System.Collections.Generic.List<string>();
            var data = MuseumContentDefaults.Get(master ? master.content : null);
            if (string.IsNullOrWhiteSpace(data.bannerText))
                warnings.Add("BannerModule: banner text is empty.");
            return warnings;
        }

        private void CreateRope(Vector3 pos, Transform parent)
        {
            var mat = CreateMaterial(Color.gray, 0.1f, false);
            CreatePrimitive(PrimitiveType.Cylinder, "Rope", pos, new Vector3(0.1f, 1.0f, 0.1f), mat, parent);
        }
    }

    /// <summary>Animación de balanceo suave para el banner.</summary>
    public class SwayingBannerV11 : MonoBehaviour
    {
        public float swaySpeed = 1.2f;
        public float swayAngle = 8f;

        private void Update()
        {
            transform.rotation = Quaternion.Euler(Mathf.Sin(Time.time * swaySpeed) * swayAngle, 0, 0);
        }
    }
}
