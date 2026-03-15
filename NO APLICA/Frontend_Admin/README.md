# Frontend Admin - Entorno VR Nano V2

Este es el panel de control web utilizado para manipular la base de datos de contenido del museo desde cualquier navegador.

## Características
- **Uso de CDN:** No requiere Node.js ni instalación de paquetes (`npm install`). Descarga Tailwind CSS, FontAwesome y SweetAlert2 directamente de la web al abrir el archivo.
- **Single Page Application (SPA):** Cambia entre las tablas de *Hologramas* y *Paneles Nano* sin recargar la página empleando Javascript Vanilla.

## Requisitos Previos
- Interfaz conectada a internet (para cargar estilos CDN).
- El **Backend Server** debe estar en ejecución (`npm start` o `node server.js` en la carpeta `Backend_Server`).

## Cómo usar
Simplemente haz **doble clic** en el archivo `index.html` para abrirlo en tu navegador favorito, o utiliza una extensión tipo "Live Server" en VS Code.

Desde esta interfaz podrás agregar, modificar y borrar el contenido que será descargado posteriormente en tiempo real por el cliente Unity VR.
