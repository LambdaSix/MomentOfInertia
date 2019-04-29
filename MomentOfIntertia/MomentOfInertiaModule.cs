using UnityEngine;

namespace MomentOfIntertia
{
    public class MomentOfInertiaModule : PartModule
    {
        [KSPField(guiActive = true, guiName = "Moment of Inertia", guiUnits = "kg-m^2")]
        public Vector3 MomentOfInertia;

        public Vector3 CalculateMomentOfInertia(Vessel v)
        {
            var inertiaTensor = InertiaTensor.Compute(v);
            return inertiaTensor.Trace();
        }

        private int ticks = 0;

        public override void OnFixedUpdate()
        {
            if (ticks > 100)
            {
                //Update I
                var I = CalculateMomentOfInertia(vessel);
                MomentOfInertia = I;
                ticks = 0;
            }
            else
            {
                ticks++;
            }
        }
    }
}