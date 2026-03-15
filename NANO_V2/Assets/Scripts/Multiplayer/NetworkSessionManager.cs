// ═══════════════════════════════════════════════════════════════════════════
// NetworkSessionManager.cs — Gestor de Sesión Photon Fusion
// ═══════════════════════════════════════════════════════════════════════════
// Conecta automáticamente a una sala de Photon Fusion al iniciar el juego.
//
// IMPORTANTE: Este script requiere Photon Fusion 2 importado.
// Cuando importes Fusion, ve a:
//   Edit → Project Settings → Player → Scripting Define Symbols
//   Y agrega: FUSION_ENABLED
// ═══════════════════════════════════════════════════════════════════════════

using UnityEngine;

#if FUSION_ENABLED
using Fusion;
using UnityEngine.SceneManagement;
#endif

namespace NanoVR.Multiplayer
{
    public class NetworkSessionManager : MonoBehaviour
    {
#if FUSION_ENABLED
        [Header("Photon Fusion Configuration")]
        [Tooltip("The runner object that synchronizes the session.")]
        public NetworkRunner runnerPrefab;

        private NetworkRunner _runner;

        private void Start()
        {
            StartGame(GameMode.AutoHostOrClient);
        }

        async void StartGame(GameMode mode)
        {
            if (_runner == null)
            {
                _runner = Instantiate(runnerPrefab);
                _runner.AddCallbacks(GetComponent<MultiplaySpawner>());
            }

            Debug.Log($"[NetworkSessionManager] Iniciando conexión en modo {mode}...");

            var startGameArgs = new StartGameArgs()
            {
                GameMode = mode,
                SessionName = "MuseoNanoFeria",
                Scene = SceneManager.GetActiveScene().buildIndex,
                SceneManager = _runner.gameObject.AddComponent<NetworkSceneManagerDefault>()
            };

            await _runner.StartGame(startGameArgs);
            Debug.Log("[NetworkSessionManager] Conexión establecida a la sala virtual.");
        }
#else
        private void Start()
        {
            Debug.LogWarning("[NetworkSessionManager] Photon Fusion no está importado. " +
                "Importa Fusion 2 y agrega 'FUSION_ENABLED' a Scripting Define Symbols.");
        }
#endif
    }
}
