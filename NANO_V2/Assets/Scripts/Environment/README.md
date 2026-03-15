# 🏛️ Scripts — Environment

## Propósito
Contiene el **sistema de generación procedural** del museo. Estos scripts construyen toda la geometría, iluminación y efectos visuales del entorno.

## Arquitectura

```
MuseumMaster (Orquestador)
├── MuseumContent  (ScriptableObject: datos de científicos + nanotemas)
├── MuseumMaterials (ScriptableObject: paleta visual PBR)
└── 7 Módulos (heredan de EnvironmentModule):
    ├── ArchitectureModule → Pisos, paredes, columnas, teatro
    ├── BannerModule       → Banner oscilante con texto emisivo
    ├── LightingModule     → GI tricolor, spots, PostFX URP
    ├── ExhibitsModule     → Hologramas + pergaminos + marcos
    ├── FXModule           → Nanopartículas animadas (orbit, pulse, drift)
    ├── LabModule          → Reactor de oro, mesas, bot guía, tareas
    └── InteractionModule  → Validación XR Rig
```

## Cómo Usar en Unity
1. Crear un **GameObject vacío** en la escena.
2. Agregar el componente **MuseumMaster**.
3. Click en **"Crear Assets por Defecto"** en el Inspector.
4. Click en **"▶ Generate"** para construir el museo.

## Archivos

| Archivo | Descripción |
|---|---|
| `MuseumMaster.cs` | Orquestador: auto-escala, genera, limpia, reconstruye |
| `MuseumContent.cs` | Datos de 6 científicos + 6 nanotemas |
| `MuseumMaterials.cs` | Texturas PBR, colores, fuentes |
| `Modules/` | 8 módulos procedurales (ver subcarpeta) |
| `Editor/` | Inspector personalizado + configuración gráfica |
