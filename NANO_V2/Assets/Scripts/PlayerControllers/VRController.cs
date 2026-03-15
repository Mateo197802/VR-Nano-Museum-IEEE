// ═══════════════════════════════════════════════════════════════════════════
// VRController.cs — Controlador de Jugador para Meta Quest (VR)
// ═══════════════════════════════════════════════════════════════════════════
// Wrapper de red para sincronizar posición de cabeza y manos del XR Rig
// a través de Photon Fusion. Solo el jugador con State Authority mueve
// su XR Rig; los demás ven un avatar sincronizado.
//
// REQUISITOS:
//   • XR Interaction Toolkit importado en el proyecto
//   • Photon Fusion 2 importado
//   • XR Rig asignado como hijo del prefab de jugador VR
// ═══════════════════════════════════════════════════════════════════════════

using UnityEngine;
// TODO: Descomentar cuando Photon Fusion esté importado:
// using Fusion;

namespace VRNanoProject.PlayerControllers
{
    /// <summary>
    /// Controlador de jugador VR que sincroniza la posición del XR Rig
    /// con otros jugadores mediante Photon Fusion NetworkBehaviour.
    /// </summary>
    public class VRController : MonoBehaviour // TODO: Cambiar a NetworkBehaviour cuando Fusion esté importado
    {
        [Header("XR Rig References")]
        [Tooltip("Transform de la cámara principal (Head) del XR Rig.")]
        public Transform head;

        [Tooltip("Transform del controlador de mano izquierda.")]
        public Transform leftHand;

        [Tooltip("Transform del controlador de mano derecha.")]
        public Transform rightHand;

        [Header("Avatar Visuals")]
        [Tooltip("Representación visual de la cabeza para otros jugadores.")]
        public Transform avatarHead;

        [Tooltip("Representación visual de la mano izquierda para otros.")]
        public Transform avatarLeftHand;

        [Tooltip("Representación visual de la mano derecha para otros.")]
        public Transform avatarRightHand;

        [Header("Network Settings")]
        [Tooltip("Frecuencia de interpolación para suavizar movimiento.")]
        public float interpolationSpeed = 15f;

        // ── Variables de red sincronizadas ────────────────────────────────
        // TODO: Usar [Networked] cuando Fusion esté importado:
        // [Networked] private Vector3 NetHeadPos { get; set; }
        // [Networked] private Quaternion NetHeadRot { get; set; }
        // [Networked] private Vector3 NetLeftPos { get; set; }
        // [Networked] private Quaternion NetLeftRot { get; set; }
        // [Networked] private Vector3 NetRightPos { get; set; }
        // [Networked] private Quaternion NetRightRot { get; set; }

        private Vector3    netHeadPos;
        private Quaternion netHeadRot;
        private Vector3    netLeftPos;
        private Quaternion netLeftRot;
        private Vector3    netRightPos;
        private Quaternion netRightRot;

        private bool IsLocal => true; // TODO: Cambiar a HasStateAuthority con Fusion

        private void Update()
        {
            if (IsLocal)
            {
                // Leer transforms del XR Rig real
                if (head)
                {
                    netHeadPos = head.position;
                    netHeadRot = head.rotation;
                }
                if (leftHand)
                {
                    netLeftPos = leftHand.position;
                    netLeftRot = leftHand.rotation;
                }
                if (rightHand)
                {
                    netRightPos = rightHand.position;
                    netRightRot = rightHand.rotation;
                }
            }
            else
            {
                // Interpolar avatares remotos suavemente
                float t = Time.deltaTime * interpolationSpeed;
                if (avatarHead)
                {
                    avatarHead.position = Vector3.Lerp(avatarHead.position, netHeadPos, t);
                    avatarHead.rotation = Quaternion.Slerp(avatarHead.rotation, netHeadRot, t);
                }
                if (avatarLeftHand)
                {
                    avatarLeftHand.position = Vector3.Lerp(avatarLeftHand.position, netLeftPos, t);
                    avatarLeftHand.rotation = Quaternion.Slerp(avatarLeftHand.rotation, netLeftRot, t);
                }
                if (avatarRightHand)
                {
                    avatarRightHand.position = Vector3.Lerp(avatarRightHand.position, netRightPos, t);
                    avatarRightHand.rotation = Quaternion.Slerp(avatarRightHand.rotation, netRightRot, t);
                }
            }
        }
    }
}
