# 1. Instalación de Photon Fusion en Unity
Para que los scripts de "Multiplayer" que te acabo de dejar funcionen, tu motor gráfico Unity necesita la librería de redes. Haz esto:

1. Ve a **Window > Asset Store** dentro de Unity.
2. Busca la palabra **"Photon Fusion"**. (Busca específicamente la versión gratuita de Fusion, no PUN 2).
3. Haz clic en **Download** y luego en **Import**.
4. Aparecerá un cuadro de configuración de Photon (PUN Wizard).
5. Se te pedirá un "App ID". Para conseguirlo, ve a la página web gratuita de [Photon Engine](https://dashboard.photonengine.com/), créate una cuenta, dale a "Create a New App", selecciona *Fusion*, copia la clave rara que te dan (el App ID) y pégala de vuelta en Unity.

# 2. Cómo compilar para la Escuela

Tendrás a chicos con gafas y chicos en laptops al mismo tiempo. Necesitas 2 versiones del juego.

### Versión 1: Las Laptops (WebGL)
WebGL compila tu juego entero para que se pueda **jugar en cualquier navegador web moderno** (Chrome, Firefox, Safari) de la escuela, sin que los chicos instalen nada.

1. En Unity, ve a **File > Build Settings**.
2. En la lista de "Platform", selecciona **WebGL**. (Si no te deja hacer clic en el botón *Switch Platform*, significa que debes cerrar Unity, abrir el Unity Hub, ir a Installs -> Add Modules a tu versión y añadir el módulo de WebGL).
3. Dale a **Switch Platform** (esto demorará unos minutos en reimportar todo).
4. Dale a **Build**. Elige una carpeta vacía. Te arrojará una carpeta con un `index.html` y varios archivos web.
5. Sube todos esos archivos a un hosting gratuito como *itch.io* (en formato HTML) o *GitHub Pages*. ¡Listo, envíales la URL a los alumnos!

### Versión 2: Las Meta Quest (Android APK)
Esta es la versión inmersiva.
1. En **File > Build Settings**, regresa la plataforma a **Android**.
2. Asegúrate de que tu "Texture Compression" esté en ASTC.
3. Conecta las gafas Quest por cable, asegúrate de tener activado el Modo Desarrollador (Developer Mode) en la App del móvil de Meta.
4. En Build Settings, selecciona tu gafa en "Run Device".
5. Dale a **Build and Run**.
6. ¡La experiencia se instalará magnéticamente en las gafas y podrán verse con los chicos de PC!
