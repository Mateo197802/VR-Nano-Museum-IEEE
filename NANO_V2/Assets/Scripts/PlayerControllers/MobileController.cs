// ═══════════════════════════════════════════════════════════════════════════
// MobileController.cs — Joystick Virtual para Celulares
// ═══════════════════════════════════════════════════════════════════════════
// Implementa controles táctiles para la experiencia en celulares:
//   • Joystick izquierdo: movimiento (caminar)
//   • Joystick derecho: rotación de cámara (ver alrededor)
//   • Sensibilidad configurable
//   • Deadzone para evitar movimiento accidental
// ═══════════════════════════════════════════════════════════════════════════

using UnityEngine;
// TODO: Descomentar cuando Photon Fusion esté importado:
// using Fusion;

namespace VRNanoProject.PlayerControllers
{
    /// <summary>
    /// Controlador para dispositivos móviles con joystick virtual en pantalla.
    /// </summary>
    public class MobileController : MonoBehaviour // TODO: Cambiar a NetworkBehaviour
    {
        [Header("Movement")]
        [Tooltip("Velocidad de caminar (metros/segundo).")]
        public float moveSpeed      = 3.5f;

        [Tooltip("Zona muerta del joystick (0-1).")]
        public float deadzone       = 0.15f;

        [Header("Camera")]
        [Tooltip("Sensibilidad de rotación horizontal.")]
        public float lookSensitivity = 2.5f;

        [Tooltip("Límite de rotación vertical.")]
        public float pitchClamp      = 60f;

        [Header("Joystick UI")]
        [Tooltip("Radio del joystick en píxeles.")]
        public float joystickRadius  = 80f;

        // ── Estado interno ────────────────────────────────────────────────
        private Camera     cam;
        private float      pitch;
        private CharacterController cc;

        // Joystick tracking
        private int   leftTouchId  = -1;
        private int   rightTouchId = -1;
        private Vector2 leftStart;
        private Vector2 rightStart;
        private Vector2 leftDelta;
        private Vector2 rightDelta;

        private bool IsLocal => true; // TODO: HasStateAuthority con Fusion

        private void Start()
        {
            cam = GetComponentInChildren<Camera>();
            cc  = GetComponent<CharacterController>();
            if (!cc)
            {
                cc        = gameObject.AddComponent<CharacterController>();
                cc.height = 1.8f;
                cc.center = new Vector3(0, 0.9f, 0);
                cc.radius = 0.3f;
            }
        }

        private void Update()
        {
            if (!IsLocal) return;
            ProcessTouches();
            ApplyMovement();
            ApplyRotation();
        }

        private void ProcessTouches()
        {
            leftDelta  = Vector2.zero;
            rightDelta = Vector2.zero;

            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                bool isLeftHalf = touch.position.x < Screen.width * 0.5f;

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        if (isLeftHalf && leftTouchId < 0)
                        {
                            leftTouchId = touch.fingerId;
                            leftStart   = touch.position;
                        }
                        else if (!isLeftHalf && rightTouchId < 0)
                        {
                            rightTouchId = touch.fingerId;
                            rightStart   = touch.position;
                        }
                        break;

                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        if (touch.fingerId == leftTouchId)
                        {
                            leftDelta = (touch.position - leftStart) / joystickRadius;
                            if (leftDelta.magnitude > 1f) leftDelta = leftDelta.normalized;
                        }
                        else if (touch.fingerId == rightTouchId)
                        {
                            rightDelta = (touch.position - rightStart) / joystickRadius;
                            if (rightDelta.magnitude > 1f) rightDelta = rightDelta.normalized;
                        }
                        break;

                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        if (touch.fingerId == leftTouchId)  leftTouchId  = -1;
                        if (touch.fingerId == rightTouchId) rightTouchId = -1;
                        break;
                }
            }
        }

        private void ApplyMovement()
        {
            if (leftDelta.magnitude < deadzone) return;

            Vector3 forward = transform.forward;
            Vector3 right   = transform.right;
            forward.y = 0; forward.Normalize();
            right.y   = 0; right.Normalize();

            Vector3 move = (forward * leftDelta.y + right * leftDelta.x) * moveSpeed;
            move.y = Physics.gravity.y;

            if (cc) cc.Move(move * Time.deltaTime);
        }

        private void ApplyRotation()
        {
            if (rightDelta.magnitude < deadzone) return;

            float yaw = rightDelta.x * lookSensitivity;
            transform.Rotate(0, yaw, 0);

            pitch -= rightDelta.y * lookSensitivity;
            pitch  = Mathf.Clamp(pitch, -pitchClamp, pitchClamp);

            if (cam)
                cam.transform.localRotation = Quaternion.Euler(pitch, 0, 0);
        }

        // ── Debug Gizmo ──────────────────────────────────────────────────

        private void OnGUI()
        {
            if (!IsLocal || Input.touchCount == 0) return;

            // Joystick visual feedback (debug)
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (leftTouchId >= 0)
                DrawJoystick(leftStart, leftDelta, Color.green);
            if (rightTouchId >= 0)
                DrawJoystick(rightStart, rightDelta, Color.cyan);
            #endif
        }

        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        private void DrawJoystick(Vector2 center, Vector2 delta, Color color)
        {
            var oldColor = GUI.color;
            GUI.color = new Color(color.r, color.g, color.b, 0.3f);
            float r = joystickRadius;
            Vector2 screenCenter = new Vector2(center.x, Screen.height - center.y);
            GUI.Box(new Rect(screenCenter.x - r, screenCenter.y - r, r * 2, r * 2), "");
            GUI.color = new Color(color.r, color.g, color.b, 0.7f);
            Vector2 knob = screenCenter + new Vector2(delta.x * r, -delta.y * r);
            GUI.Box(new Rect(knob.x - 15, knob.y - 15, 30, 30), "");
            GUI.color = oldColor;
        }
        #endif
    }
}
