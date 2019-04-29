using kOS;
using kOS.AddOns;
using kOS.Safe.Exceptions;
using kOS.Safe.Encapsulation;
using kOS.Safe.Encapsulation.Suffixes;
using kOS.Safe.Utilities;
using kOS.Suffixed;
using UnityEngine;

namespace MomentOfIntertia
{
    [kOSAddon("MOI")]
    [KOSNomenclature("MOIAddon")]
    public class MoIAddon : kOS.Suffixed.Addon
    {
        public MoIAddon(SharedObjects shared) : base(shared)
        {
            InitializeSuffixes();
        }

        private void InitializeSuffixes()
        {
            AddSuffix("I", new Suffix<Vector>(MomentOfInertia, "Calculated moment of inertia for current vessel"));
        }

        public override BooleanValue Available() => true;

        private Vector MomentOfInertia()
        {
            if (shared.Vessel != FlightGlobals.ActiveVessel)
                throw new KOSException("You may only call addons:MOI:I from the active vessel");
            if (Available())
            {
                var I = InertiaTensor.Compute(shared.Vessel).Trace();
                return new Vector(I);
            }

            throw new KOSUnavailableAddonException("I", "MomentOfInertia");
        }
    }
}