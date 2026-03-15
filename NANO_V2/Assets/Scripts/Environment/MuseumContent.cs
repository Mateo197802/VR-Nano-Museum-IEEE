// ═══════════════════════════════════════════════════════════════════════════
// MuseumContent.cs — ScriptableObject: Datos del Museo
// ═══════════════════════════════════════════════════════════════════════════
// Define los contenidos científicos y nanotecnológicos que se muestran
// en el museo. Incluye datos de 6 científicos pioneros y 6 temas nano.
// Uso: Crear asset vía menú VRNanoProject → Museum Content dentro de Unity.
// ═══════════════════════════════════════════════════════════════════════════

using System;
using UnityEngine;

namespace VRNanoProject.Environment
{
    /// <summary>
    /// ScriptableObject que alberga todo el contenido textual del museo:
    /// texto del banner, científicos y temas de nanotecnología.
    /// </summary>
    [CreateAssetMenu(menuName = "VRNanoProject/Museum Content", fileName = "MuseumContent")]
    public class MuseumContent : ScriptableObject
    {
        [Tooltip("Texto principal del banner de bienvenida del museo.")]
        public string bannerText = "LABORATORIO DE NANOTECNOLOGIA";

        [Tooltip("Lista de científicos pioneros expuestos en el museo.")]
        public ScientistEntry[] scientists = Array.Empty<ScientistEntry>();

        [Tooltip("Lista de temas de nanotecnología expuestos en pergaminos.")]
        public NanoTopic[] nanoTopics = Array.Empty<NanoTopic>();
    }

    // ── Estructuras de Datos ──────────────────────────────────────────────

    /// <summary>Entrada de un científico con nombre, biografía y foto.</summary>
    [Serializable]
    public struct ScientistEntry
    {
        public string     shortName;
        public string     fullName;
        [TextArea(3, 8)]
        public string     bio;
        public Texture2D  photo;
    }

    /// <summary>Tema de nanotecnología con título y descripción.</summary>
    [Serializable]
    public struct NanoTopic
    {
        public string     title;
        [TextArea(3, 8)]
        public string     description;
    }

    /// <summary>Contenedor plano para datos del museo (usado en runtime).</summary>
    public struct MuseumContentData
    {
        public string           bannerText;
        public ScientistEntry[] scientists;
        public NanoTopic[]      nanoTopics;
    }

    // ── Datos por Defecto ─────────────────────────────────────────────────

    /// <summary>
    /// Provee datos hardcoded de 6 científicos y 6 nanotemas como fallback
    /// cuando no se asigna un ScriptableObject o el JSON está vacío.
    /// </summary>
    public static class MuseumContentDefaults
    {
        public static MuseumContentData Get(MuseumContent content)
        {
            var data = Default;
            if (content)
            {
                if (!string.IsNullOrWhiteSpace(content.bannerText))
                    data.bannerText = content.bannerText;
                if (content.scientists != null && content.scientists.Length > 0)
                    data.scientists = content.scientists;
                if (content.nanoTopics != null && content.nanoTopics.Length > 0)
                    data.nanoTopics = content.nanoTopics;
            }
            return data;
        }

        public static readonly MuseumContentData Default = new MuseumContentData
        {
            bannerText = "LABORATORIO DE NANOTECNOLOGIA",
            scientists = new[]
            {
                new ScientistEntry
                {
                    shortName = "Feynman",
                    fullName  = "Richard Feynman",
                    bio       = "Richard Feynman\n(1918-1988)\n\nPionero del orden atomico.\nPropuso la vision de\nmanipular la materia."
                },
                new ScientistEntry
                {
                    shortName = "Taniguchi",
                    fullName  = "Norio Taniguchi",
                    bio       = "Norio Taniguchi\n(1912-1999)\n\nAcuno el termino\nnanotecnologia en 1974.\nImpulso la precision\nultra fina."
                },
                new ScientistEntry
                {
                    shortName = "Iijima",
                    fullName  = "Sumio Iijima",
                    bio       = "Sumio Iijima\n(1939 - Presente)\n\nDescubrio los nanotubos\nde carbono en 1991.\nBase de nuevos materiales."
                },
                new ScientistEntry
                {
                    shortName = "Drexler",
                    fullName  = "K. Eric Drexler",
                    bio       = "K. Eric Drexler\n(1955 - Presente)\n\nImpulso la vision de\nnanotecnologia molecular\ny sistemas de ensamblaje."
                },
                new ScientistEntry
                {
                    shortName = "Binnig",
                    fullName  = "Gerd Binnig",
                    bio       = "Gerd Binnig\n(1947 - Presente)\n\nCo-invento el microscopio\nde tunel (STM), clave\npara visualizar atomos."
                },
                new ScientistEntry
                {
                    shortName = "Dresselhaus",
                    fullName  = "Mildred Dresselhaus",
                    bio       = "Mildred Dresselhaus\n(1930-2017)\n\nPionera en nanocarbono\ny grafito. Su trabajo\nabono el grafeno."
                }
            },
            nanoTopics = new[]
            {
                new NanoTopic { title = "NANOMEDICINA",         description = "Tratamiento con precision\ncelular y entrega dirigida\nde farmacos inteligentes." },
                new NanoTopic { title = "NANOTUBOS DE CARBONO", description = "Cilindros de carbono con\nresistencia extrema y\nconductividad superior." },
                new NanoTopic { title = "GRAFENO",              description = "Lamina de un solo atomo\nde espesor con fuerza y\nconductividad increibles." },
                new NanoTopic { title = "AUTOENSAMBLAJE",       description = "Organizacion autonoma de\nmoleculas en estructuras\nordenadas y complejas." },
                new NanoTopic { title = "NANOSENSORES",         description = "Deteccion ultra sensible\nde gases, biomarcadores\ny cambios del entorno." },
                new NanoTopic { title = "PUNTOS CUANTICOS",     description = "Emision y control de luz\nen nanoescala para\nimagenes y displays." }
            }
        };
    }
}
