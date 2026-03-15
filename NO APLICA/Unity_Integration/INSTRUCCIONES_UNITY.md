# Integración Cliente Unity (ENTORNO VR V2)

Para pasar del esquema antiguo (basado estáticamente en ScritableObjects) a la nueva Arquitectura de Microservicios Dinámica, por favor sigue rigurosamente estos pasos dentro del editor de Unity.

## Paso 1: Importar los Scripts
1. Dentro del explorador de Windows, copia la carpeta `Assets` que se encuentra en este directorio `Unity_Integration`.
2. Pégala directamente dentro de la carpeta raíz de tu proyecto de Unity actual (`ENTORNO_NANO_VR` o `NANO_V2`).
3. Unity recargará y los detectará automáticamente.

## Paso 2: Configurar APIClient
1. En Unity, selecciona el GameObject que usas típicamente como **GameManager** o **MuseumMaster** en tu jerarquía principal (Scene Root).
2. Arrástrale el script `APIClient.cs`.
3. Notarás un nuevo componente. En el Inspector del script, asegúrate de que el URL base sea: `http://localhost:3000/api` (asumiendo que correrás la experiencia y el servidor web localmente por ahora).

## Paso 3: Actualizar el Orquestador
1. Abre tu antiguo script de `MuseumMaster.cs` (si el tuyo tiene un nombre similar y tiene tu lógica de instanciación).
2. Compara tu script con la plantilla `MuseumMaster.cs` adjunta en esta carpeta.
3. Asegúrate de que el método `Awake` y `Start` utilicen las corrutinas asíncronas (`async void Start()` o `IEnumerator`) para **obligar a Unity a detener cualquier generación geométrica** hasta que `apiClient.GetScientistsAsync()` y `apiClient.GetNanoTopicsAsync()` finalicen.
4. *¡Listo!* Cuando le des "Play", verás por consola cómo descarga los nombres modificados desde la web. Mágicamente, sin recompilar, cada vez que cambies los textos desde tu web, también cambiarán en la experiencia VR.
