// ═══════════════════════════════════════════════════════════════════════════
// MultiplaySpawner.cs — Spawner de Jugadores Cross-Platform
// ═══════════════════════════════════════════════════════════════════════════
// Instancia el prefab correcto según la plataforma (Desktop, VR, Mobile).
//
// IMPORTANTE: Este script requiere Photon Fusion 2 importado.
// Cuando importes Fusion, ve a:
//   Edit → Project Settings → Player → Scripting Define Symbols
//   Y agrega: FUSION_ENABLED
// ═══════════════════════════════════════════════════════════════════════════

using System;
using System.Collections.Generic;
using UnityEngine;

#if FUSION_ENABLED
using Fusion;
using Fusion.Sockets;
#endif

namespace NanoVR.Multiplayer
{
#if FUSION_ENABLED
    [RequireComponent(typeof(NetworkSessionManager))]
    public class MultiplaySpawner : MonoBehaviour, INetworkRunnerCallbacks
    {
        [Header("Player Prefabs (Avatares)")]
        [Tooltip("Cuerpo para estudiantes en PC/Laptop (Mouse/Teclado)")]
        public NetworkPrefabRef desktopPlayerPrefab;

        [Tooltip("Cuerpo para estudiantes con gafas VR (Manos + Cabeza)")]
        public NetworkPrefabRef vrPlayerPrefab;

        [Tooltip("Cuerpo para estudiantes en Celular (Joysticks)")]
        public NetworkPrefabRef mobilePlayerPrefab;

        [Header("Spawn Points")]
        public Vector3 spawnPosition = new Vector3(0, 1, 0);

        private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                Debug.Log($"[MultiplaySpawner] Nuevo jugador ({player}) unido. Generando Avatar...");

                NetworkPrefabRef selectedPrefab = desktopPlayerPrefab;

                #if UNITY_ANDROID || UNITY_IOS
                if (UnityEngine.XR.XRSettings.isDeviceActive)
                    selectedPrefab = vrPlayerPrefab;
                else
                    selectedPrefab = mobilePlayerPrefab;
                #endif

                NetworkObject networkPlayerObject = runner.Spawn(selectedPrefab, spawnPosition, Quaternion.identity, player);
                _spawnedCharacters.Add(player, networkPlayerObject);
            }
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
            {
                runner.Despawn(networkObject);
                _spawnedCharacters.Remove(player);
                Debug.Log($"[MultiplaySpawner] Jugador ({player}) salió.");
            }
        }

        // --- Métodos requeridos por INetworkRunnerCallbacks ---
        public void OnInput(NetworkRunner runner, NetworkInput input) { }
        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnShutdown(NetworkRunner runner, ShutdownReason info) { }
        public void OnConnectedToServer(NetworkRunner runner) { Debug.Log("[Photon] Conectado al servidor."); }
        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }
    }
#else
    public class MultiplaySpawner : MonoBehaviour
    {
        [Header("Player Prefabs (sin Fusion)")]
        [Tooltip("Prefab Desktop (se activará con Photon Fusion)")]
        public GameObject desktopPlayerPrefab;

        [Tooltip("Prefab VR (se activará con Photon Fusion)")]
        public GameObject vrPlayerPrefab;

        [Tooltip("Prefab Mobile (se activará con Photon Fusion)")]
        public GameObject mobilePlayerPrefab;

        [Header("Spawn Points")]
        public Vector3 spawnPosition = new Vector3(0, 1, 0);

        private void Start()
        {
            Debug.LogWarning("[MultiplaySpawner] Photon Fusion no está importado. " +
                "Importa Fusion 2 y agrega 'FUSION_ENABLED' a Scripting Define Symbols.");
        }
    }
#endif
}
