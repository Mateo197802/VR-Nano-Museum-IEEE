# 🌐 Scripts — Multiplayer

## Propósito
Capa de red que permite que múltiples estudiantes se conecten simultáneamente al museo VR usando **Photon Fusion 2**.

## Archivos

| Archivo | Descripción |
|---|---|
| `NetworkSessionManager.cs` | Conecta automáticamente a una sala Photon al iniciar |
| `MultiplaySpawner.cs` | Instancia prefabs de jugador según la plataforma detectada (VR, Desktop, Mobile) |

## Requisitos
1. **Photon Fusion 2**: Importar desde [Photon Engine](https://www.photonengine.com/fusion).
2. **App ID**: Configurar en `Assets/Photon/Fusion/Resources/PhotonAppSettings.asset`.

## Flujo de Conexión
```
Start() → NetworkSessionManager → StartGameAsync()
    → Se conecta a sala "NanoMuseum"
    → MultiplaySpawner.Spawned() detecta plataforma
    → Instancia prefab correcto (Desktop/VR/Mobile)
```
