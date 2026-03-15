// ═══════════════════════════════════════════════════════════════════════════
// PlayerNameTag.cs — Nombre Flotante sobre Avatares
// ═══════════════════════════════════════════════════════════════════════════
// TextMesh que muestra el nombre del jugador y siempre mira hacia la
// cámara local. Se oculta automáticamente para el jugador propio.
// ═══════════════════════════════════════════════════════════════════════════

using UnityEngine;
// TODO: Descomentar cuando Photon Fusion esté importado:
// using Fusion;

namespace VRNanoProject.PlayerControllers
{
    /// <summary>
    /// Nombre flotante con billboard effect que siempre mira a la cámara.
    /// Se adjunta como hijo del prefab de jugador.
    /// </summary>
    public class PlayerNameTag : MonoBehaviour // TODO: Cambiar a NetworkBehaviour
    {
        [Header("Appearance")]
        [Tooltip("Tamaño del texto.")]
        public float characterSize = 0.04f;

        [Tooltip("Tamaño de fuente.")]
        public int fontSize = 48;

        [Tooltip("Color del texto.")]
        public Color textColor = Color.white;

        [Tooltip("Color de emisión para visibilidad nocturna.")]
        public Color emissionColor = new Color(0.6f, 0.85f, 1f, 1f);

        [Tooltip("Intensidad de emisión.")]
        public float emissionIntensity = 1.5f;

        [Header("Position")]
        [Tooltip("Offset vertical sobre el jugador.")]
        public float yOffset = 2.3f;

        // ── Red ──────────────────────────────────────────────────────────
        // TODO: Usar [Networked] con Fusion:
        // [Networked, Capacity(32)] public string PlayerName { get; set; }
        private string playerName = "Estudiante";

        private TextMesh textMesh;
        private Camera   mainCam;

        private bool IsLocal => true; // TODO: HasStateAuthority con Fusion

        private void Start()
        {
            mainCam = Camera.main;
            CreateTextMesh();

            // Ocultar para el jugador local
            if (IsLocal && textMesh)
                textMesh.gameObject.SetActive(false);
        }

        private void LateUpdate()
        {
            if (!textMesh || !mainCam) return;

            // Billboard: siempre mira a la cámara
            Vector3 lookDir = mainCam.transform.position - textMesh.transform.position;
            if (lookDir.sqrMagnitude > 0.001f)
                textMesh.transform.rotation = Quaternion.LookRotation(lookDir);

            // Actualizar posición
            textMesh.transform.position = transform.position + Vector3.up * yOffset;
        }

        /// <summary>Establece el nombre del jugador.</summary>
        public void SetName(string name)
        {
            playerName = name;
            if (textMesh) textMesh.text = name;
        }

        private void CreateTextMesh()
        {
            var go = new GameObject("NameTag");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = Vector3.up * yOffset;

            textMesh               = go.AddComponent<TextMesh>();
            textMesh.text          = playerName;
            textMesh.characterSize = characterSize;
            textMesh.fontSize      = fontSize;
            textMesh.color         = textColor;
            textMesh.anchor        = TextAnchor.MiddleCenter;
            textMesh.alignment     = TextAlignment.Center;
            textMesh.fontStyle     = FontStyle.Bold;

            var renderer = textMesh.GetComponent<Renderer>();
            if (renderer)
            {
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows    = false;
                var mat = renderer.material;
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", emissionColor * emissionIntensity);
            }
        }
    }
}
