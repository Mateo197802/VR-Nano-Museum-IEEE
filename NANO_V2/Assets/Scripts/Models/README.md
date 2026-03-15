# 📦 Scripts — Models

## Propósito
Estructuras de datos (DTOs) para la serialización/deserialización de contenido del museo.

## Archivos

| Archivo | Descripción |
|---|---|
| `MuseumDTOs.cs` | Data Transfer Objects: `MuseumContentDTO`, `ScientistDTO`, `NanoTopicDTO` |

## Estructura del JSON

```json
{
  "bannerText": "LABORATORIO DE NANOTECNOLOGIA IEEE",
  "scientists": [
    {
      "id": 1,
      "shortName": "Feynman",
      "fullName": "Richard Feynman",
      "bio": "Biografía...",
      "photoUrl": ""
    }
  ],
  "nanoTopics": [
    {
      "id": 1,
      "title": "NANOMEDICINA",
      "description": "Descripción..."
    }
  ]
}
```

Estos DTOs son usados por `ContentLoader.cs` y `APIClient.cs` para mapear JSON a `MuseumContentData`.
