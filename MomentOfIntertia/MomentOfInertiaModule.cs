using UnityEngine;

namespace MomentOfIntertia
{
    public class MomentOfInertiaModule : PartModule
    {
        [KSPField(guiActive = true, guiName = "Moment of Inertia")]
        public string MomentOfInertia;

        public Vector3 CalculateMomentOfInertia(Vessel v)
        {
            var inertiaTensor = InertiaTensor.Compute(v);
            return inertiaTensor.Trace();
        }


        /// <inheritdoc />
        public void Start()
        {
            var I = CalculateMomentOfInertia(vessel);
        }

        private int ticks = 0;

        public void FixedUpdate()
        {
            if (vessel.state == Vessel.State.ACTIVE && vessel.loaded)
            {

                if (ticks >= 100)
                {
                    //Update I
                    var I = CalculateMomentOfInertia(vessel);
                    MomentOfInertia = FormatI(I);
                    ticks = 0;
                }
                else
                {
                    ticks++;
                }
            }
        }

        public static string FormatI(Vector3 I)
        {
            return $"({I.x},{I.y},{I.z})";
        }
    }
}