# 🧩 Scripts — Environment/Modules

## Propósito
Módulos procedurales que heredan de `EnvironmentModule`. Cada módulo genera una parte específica del museo de forma independiente y reutilizable.

## Módulos

| Módulo | Líneas | Qué Genera |
|---|---|---|
| `EnvironmentModule.cs` | Base | Clase abstracta con utilidades PBR, texto y layout |
| `ArchitectureModule.cs` | ~150 | Pisos mármol/alfombra, 4 paredes, muro divisorio con puerta cristal, columnas, teatro con cortinas |
| `ExhibitsModule.cs` | ~280 | Hologramas emisivos de científicos, marcos con foto, pergaminos con nanotemas, grids holográficos |
| `LightingModule.cs` | ~120 | GI tricolor ambient, key/fill lights, spot lamps VeryHigh, PostFX (Bloom, ACES, Vignette) |
| `FXModule.cs` | ~240 | Clusters de nanopartículas: 80 átomos con drift browniano, 10 orbitadores, aura pulsante, luz dinámica |
| `LabModule.cs` | ~450 | Mesas con reactivos, reactor de oro, cámara extracción, campana flujo, NanoDrops, fregadero, bot guía, tareas |
| `BannerModule.cs` | ~80 | Banner de tela con balanceo y texto emisivo |
| `InteractionModule.cs` | ~30 | Validación de presencia de XR Rig |
| `URPPostFXHelper.cs` | ~130 | Bloom, Tonemapping ACES, Vignette, Color Grading via reflection |

## Clases Auxiliares (Animadores)
- `NanoClusterAnimator` → Rotación de orbitadores + pulso de aura y luz
- `JitteryMotionV11` → Movimiento orgánico errático para clusters
- `NanoAtomDrift` → Drift browniano individual para átomos
- `SwayingBannerV11` → Balanceo suave del banner
- `AutoDoor` → Puerta que se abre al acercarse
- `LabBotFollower` → Bot que sigue al usuario y muestra instrucciones
- `LabTaskManager` → Sistema de tareas guiadas del laboratorio
- `LayoutUtils` → Alineamiento de posiciones con columnas
