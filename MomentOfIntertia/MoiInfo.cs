using UnityEngine;

namespace MomentOfIntertia
{
    public class MoiInfo : PartModule
    {
        [KSPField(guiActive = true, guiName = "Moment of Inertia", guiUnits = "kg-m^2")]
        public Vector3 MomentOfInertia;

        public Vector3 CalculateMomentOfInertia(Vessel v)
        {
            var inertiaTensor = InertiaTensor.Compute(v);
            return inertiaTensor.Trace();
        }
    }
}