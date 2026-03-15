# Backend Server - Nano VR Environment V2

Este es el servidor REST API desarrollado en Node.js para alimentar la base de datos de contenido del Entorno VR Nano.

## Requisitos
- Node.js (v14 o superior)

## Instalación y Ejecución
1. Abre una terminal en esta carpeta (`Backend_Server`).
2. Instala las dependencias ejecutando:
   ```bash
   npm install
   ```
3. Inicia el servidor con:
   ```bash
   npm start
   ```
   *Nota: Si estás desarrollando, puedes usar `npm run dev` para usar nodemon (se reinicia automáticamente si detecta cambios).*

El servidor correrá por defecto en `http://localhost:3000`.

## Base de Datos
Se utiliza **SQLite** para maximizar la portabilidad.
Un archivo llamado `database.sqlite` se creará automáticamente en la raíz de esta carpeta la primera vez que se ejecute el código.
La base de datos viene con tablas (`scientists` y `nanotopics`) precargadas con ejemplos si el archivo se genera desde cero.

## Endpoints Principales
- `GET /api/scientists`: Retorna la lista de científicos (para los hologramas).
- `POST /api/scientists`: Añade un nuevo científico.
- `GET /api/nanotopics`: Retorna la lista de temas nano (para los paneles informativos).
- `POST /api/nanotopics`: Añade un nuevo tema nano.
