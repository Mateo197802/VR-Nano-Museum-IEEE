# 📡 Scripts — Services

## Propósito
Servicios de carga de datos para el museo. Dos opciones:

| Servicio | Fuente | Uso |
|---|---|---|
| `ContentLoader.cs` | `StreamingAssets/museum_content.json` | **Principal**: Lee JSON local, funciona en todas las plataformas |
| `APIClient.cs` | Servidor Node.js REST | **Opcional**: Solo si se reactiva el backend dinámico |

## ContentLoader (Recomendado)
Lee el archivo `museum_content.json` desde `StreamingAssets` usando `UnityWebRequest` (compatible con WebGL y Android).

**Cómo editar contenido:**
1. Abrir `Assets/StreamingAssets/museum_content.json`.
2. Modificar científicos, nanotemas o texto del banner.
3. ¡Sin recompilar! Los cambios se cargan al iniciar.

## APIClient (Opcional)
Si se necesita un panel web para que administradores editen contenido en tiempo real:
1. Reactivar el backend de `NO APLICA/Backend/`.
2. Cambiar `useJsonContent = false` en MuseumMaster.
3. Configurar la URL del servidor.
