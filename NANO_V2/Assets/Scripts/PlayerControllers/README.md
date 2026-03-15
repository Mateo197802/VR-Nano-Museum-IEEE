# 🎮 Scripts — PlayerControllers

## Propósito
Controladores de jugador para cada plataforma soportada. Se instancian automáticamente según la plataforma detectada por `MultiplaySpawner`.

## Controladores

| Archivo | Plataforma | Controles |
|---|---|---|
| `DesktopController.cs` | PC / WebGL | WASD + Mouse Look |
| `VRController.cs` | Meta Quest | XR Rig (cabeza + 2 manos sincronizadas) |
| `MobileController.cs` | Celulares | Joystick virtual táctil (izq: mover, der: mirar) |
| `PlayerNameTag.cs` | Todas | Nombre flotante billboard sobre el avatar |

## Integración con Photon Fusion
- Todos los controladores están preparados para heredar de `NetworkBehaviour`.
- Los `// TODO:` en cada archivo indican las líneas exactas a descomentar al importar Fusion.
- Solo el jugador con `HasStateAuthority` controla su avatar; los demás ven copias interpoladas.

## Configuración de Prefabs
Para cada plataforma, crea un prefab con:
1. El controlador correspondiente.
2. Un `PlayerNameTag` como componente.
3. Un modelo visual (capsula, avatar, etc.).
