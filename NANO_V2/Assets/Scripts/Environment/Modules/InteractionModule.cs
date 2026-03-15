// ═══════════════════════════════════════════════════════════════════════════
// InteractionModule.cs — Validación de XR Rig
// ═══════════════════════════════════════════════════════════════════════════
// Módulo de interacción que valida la presencia del XR Rig.
// En el futuro aquí se conectarán las interacciones de mano VR.
// ═══════════════════════════════════════════════════════════════════════════

using System.Collections.Generic;
using UnityEngine;

namespace VRNanoProject.Environment
{
    public class InteractionModule : EnvironmentModule
    {
        [Header("XR")]
        [Tooltip("Activar para requerir un XR Rig asignado.")]
        public bool       requireXrRig = false;

        [Tooltip("Referencia al GameObject raíz del XR Rig en la escena.")]
        public GameObject xrRigRoot;

        public override void Build()
        {
            // Módulo de validación. Las interacciones XR se configuran
            // a través del XR Interaction Toolkit directamente.
        }

        public override List<string> Validate()
        {
            var warnings = new List<string>();
            if (requireXrRig && !xrRigRoot)
                warnings.Add("InteractionModule: XR rig is required but not assigned.");
            return warnings;
        }
    }
}
