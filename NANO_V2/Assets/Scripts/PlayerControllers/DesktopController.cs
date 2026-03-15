// ═══════════════════════════════════════════════════════════════════════════
// DesktopController.cs — Controlador de Jugador para PC/WebGL
// ═══════════════════════════════════════════════════════════════════════════
// Movimiento WASD + Mouse Look para escritorio.
//
// IMPORTANTE: Cuando importes Photon Fusion 2, agrega 'FUSION_ENABLED'
// a Edit → Project Settings → Player → Scripting Define Symbols.
// ═══════════════════════════════════════════════════════════════════════════

using UnityEngine;

#if FUSION_ENABLED
using Fusion;
#endif

namespace NanoVR.PlayerControllers
{
    public class DesktopController :
#if FUSION_ENABLED
        NetworkBehaviour
#else
        MonoBehaviour
#endif
    {
        [Header("Movement Settings")]
        public float moveSpeed        = 5f;
        public float mouseSensitivity = 2f;

        [Header("References")]
        public Camera playerCamera;
        private CharacterController _characterController;
        private float pitch = 0f;

#if FUSION_ENABLED
        public override void Spawned()
        {
            _characterController = GetComponent<CharacterController>();

            if (!HasStateAuthority)
            {
                if (playerCamera != null)
                {
                    playerCamera.enabled = false;
                    playerCamera.GetComponent<AudioListener>().enabled = false;
                }
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible   = false;
            }
        }
#else
        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
            if (!_characterController)
            {
                _characterController        = gameObject.AddComponent<CharacterController>();
                _characterController.height = 1.8f;
                _characterController.center = new Vector3(0, 0.9f, 0);
                _characterController.radius = 0.3f;
            }
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;
        }
#endif

        private void Update()
        {
#if FUSION_ENABLED
            if (!HasStateAuthority) return;
#endif
            HandleMouseLook();
            HandleMovement();
        }

        private void HandleMouseLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            transform.Rotate(Vector3.up * mouseX);

            pitch -= mouseY;
            pitch  = Mathf.Clamp(pitch, -85f, 85f);

            if (playerCamera != null)
                playerCamera.transform.localEulerAngles = new Vector3(pitch, 0f, 0f);
        }

        private void HandleMovement()
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;
            if (_characterController)
                _characterController.Move(move * moveSpeed * Time.deltaTime);
        }
    }
}
