# VR Nano Museum - IEEE Nanocouncil Chapter

[![Unity 2022.3 LTS](https://img.shields.io/badge/Unity-2022.3%20LTS-black.svg?style=for-the-badge&logo=unity)](https://unity.com/)
[![Platform: Meta Quest](https://img.shields.io/badge/Platform-Meta%20Quest%202%2F3%2FPro-blue.svg?style=for-the-badge&logo=meta)](https://www.meta.com/quest/)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg?style=for-the-badge)](LICENSE)

---

## 🇺🇸 English Version

An immersive virtual museum designed for nanotechnology education, developed for the **IEEE Nanocouncil Student Branch**. The project combines a dynamic modular architecture with an interactive laboratory environment.

### 🏛️ Key Features
*   **Modular Architecture:** Orchestrator-based system (`MuseumMaster`) that allows procedural and flexible environment generation.
*   **Gallery of Scientists:** Interactive exhibition featuring pioneers like Richard Feynman, Norio Taniguchi, Sumio Iijima, and Eric Drexler.
*   **Interactive Laboratory:** Functional workstations for AuNPs (Gold Nanoparticles) study, including:
    *   **Gold Reactor:** Equipped with emissive visual effects.
    *   **Nanodrops & Bio-safety Stations:** High-fidelity clinical equipment.
    *   **Lab Guide Bot:** Interactive assistant with movement tracking and task guidance.
*   **Visual Fidelity:** High-resolution marble and carpet textures, dynamic lab banners, and Brownian-motion-simulated nanoparticles.

### 🛠️ Project Structure
*   📂 `Assets/Assets/Art/`: Textures and high-resolution photography.
*   📂 `Assets/Assets/EnvironmentData/`: `ScriptableObjects` for data-driven design.
*   📂 `Assets/Assets/Scripts/Environment/`: Modular code and Editor tools.

### 🚀 Implementation Guide (Standard Setup)
1.  **Clone** this repository or download the source code.
2.  **Unity Version:** Open the project using **Unity 2022.3.x LTS** (Universal Render Pipeline - URP).
3.  **Dependencies:** Ensure **Meta XR SDK** and **XR Interaction Toolkit** are installed via Package Manager.
4.  **Scene Setup:**
    *   Open the `SampleScene` (or your main museum scene).
    *   Locate the `MuseumMaster` object in the hierarchy.
    *   Assign the `ContenidoMuseo` and `MaterialesMuseo` assets from `Assets/Assets/EnvironmentData/`.
5.  **Generation:** Click the **[Generate]** button in the Inspector to build the environment.

---

## 🇪🇸 Versión en Español

Un museo virtual inmersivo diseñado para la educación en nanotecnología, desarrollado para la **Rama Estudiantil IEEE Nanocouncil**. El proyecto combina una arquitectura modular dinámica con un entorno de laboratorio interactivo.

### 🏛️ Características Principales
*   **Arquitectura Modular:** Sistema basado en orquestadores (`MuseumMaster`) que permite generar el entorno de forma procedural y flexible.
*   **Galería de Científicos:** Exhibición interactiva de pioneros como Richard Feynman, Norio Taniguchi, Sumio Iijima y Eric Drexler.
*   **Laboratorio Interactivo:** Estaciones de trabajo funcionales para el estudio de AuNPs (Nanopartículas de Oro), incluyendo:
    *   **Reactor de Oro:** Equipado con efectos visuales de emisión.
    *   **Nanodrops y Estaciones de Bioseguridad:** Equipamiento clínico de alta fidelidad.
    *   **Bot Guía de Laboratorio:** Asistente interactivo con seguimiento y guía de tareas.
*   **Fidelidad Visual:** Texturas de mármol y alfombra de alta resolución, banners dinámicos y nanopartículas con movimiento browniano simulado.

### 🛠️ Estructura del Proyecto
*   📂 `Assets/Assets/Art/`: Texturas y fotografías de alta resolución.
*   📂 `Assets/Assets/EnvironmentData/`: `ScriptableObjects` para un diseño basado en datos.
*   📂 `Assets/Assets/Scripts/Environment/`: Código modular y herramientas de Editor.

### 🚀 Guía de Implementación (Configuración Estándar)
1.  **Clonar** este repositorio o descargar el código fuente.
2.  **Versión de Unity:** Abrir el proyecto usando **Unity 2022.3.x LTS** (Universal Render Pipeline - URP).
3.  **Dependencias:** Asegurarse de tener instalado **Meta XR SDK** y **XR Interaction Toolkit** mediante el Package Manager.
4.  **Configuración de Escena:**
    *   Abrir la escena principal (o `SampleScene`).
    *   Localizar el objeto `MuseumMaster` en la jerarquía.
    *   Asignar los activos `ContenidoMuseo` y `MaterialesMuseo` desde `Assets/Assets/EnvironmentData/`.
5.  **Generación:** Hacer clic en el botón **[Generate]** en el Inspector para construir el entorno.

---

### 📜 Credits & License / Créditos y Licencia
Developed for the **IEEE Nanocouncil Chapter**. / Desarrollado para el **IEEE Nanocouncil Chapter**.
Textures provided by **ambientCG** under CC0 license. / Texturas proporcionadas por **ambientCG** bajo licencia CC0.
Historical images from public repositories. / Imágenes históricas de repositorios públicos.

*This project is part of a scientific outreach initiative for engineering and science students.*
*Este proyecto es parte de una iniciativa de divulgación científica para estudiantes de ingeniería y ciencias.*
