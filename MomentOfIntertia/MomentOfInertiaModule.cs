using System;
using System.Collections.Generic;
using System.Linq;
using kOS.Utilities;
using KSP.IO;
using UnityEngine;

namespace MomentOfIntertia
{
    public class MomentOfInertiaModule : PartModule
    {
        [KSPField(guiActiveEditor = true, guiName = "Moment of Inertia: ")]
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

        public IEnumerable<ThrustVector> CalculateMomentsOfThrust(Vessel v)
        {
            foreach (var vPart in v.Parts)
            {
                yield return new ThrustVector(v, vPart);
            }
        }

        public VesselInformation CalculateVesselPhysics(Vessel v)
        {
            var mW = v.GetWetMass();
            var mD = v.GetDryMass();

            // Assuming VAB by default..
            Vector3 size = ShipConstruction.CalculateCraftSize(vessel.parts, vessel.rootPart);

            return new VesselInformation()
            {
                WetMass = mW,
                DryMass = mD,
                Height = size.y,
                Diameter = size.x,
                Width = size.z
            };

        }

        [KSPEvent(guiActiveEditor = true, guiName = "Dump Vessel Data")]
        public void OnDumpVesselTensors()
        {
            ScreenMessages.PostScreenMessage("Processing vessel..", 5.0f, ScreenMessageStyle.UPPER_CENTER);

            // Calculate I for the entire vessel
            var I = CalculateMomentOfInertia(vessel);

            // Calculate the thrust location + part position for all engines
            var M = CalculateMomentsOfThrust(vessel);

            // Calculate the Length, Height, Width, Diameter and mass for the vessel
            var vesselInfo = CalculateVesselPhysics(vessel);

            vesselInfo.ThrustVectors = M.ToList();
            vesselInfo.MomentOfInertia = I;
            vesselInfo.VesselName = vessel.GetDisplayName();
            vesselInfo.PartCount = vessel.parts.Count;
            vesselInfo.RootPart = vessel.rootPart.partName;

            // turn I,M,R into a json of { Vessel = { "I": {}, "M": {}, r[..]: {} } }
            var jsonBlob = JsonUtility.ToJson(vesselInfo);
            var outputName = $"{GameDatabase.Instance.PluginDataFolder}/Ships/VesselData/{vesselInfo.VesselName}.json";
            File.AppendAllText<MomentOfInertiaModule>(jsonBlob, outputName);
        }

        public static string FormatI(Vector3 I)
        {
            return $"({I.x},{I.y},{I.z})";
        }
    }

    [Serializable]
    public class VesselInformation
    {
        public string VesselName { get; set; }

        public int PartCount { get; set; }

        public float WetMass { get; set; }
        public float DryMass { get; set; }
        public float Height { get; set; }
        public float Diameter { get; set; }
        public float Width { get; set; }

        public List<ThrustVector> ThrustVectors { get; set; }
        public Vector3 MomentOfInertia { get; set; }
        public string RootPart { get; set; }
    }

    public class ThrustVector
    {
        public string PartName { get; set; }
        public Vector3 PartPosition { get; set; }
        public Vector3 ThrustPosition { get; set; }

        public ThrustVector(Vessel vessel, Part part)
        {
            PartName = part.partName;
            if (part.Modules.Contains<ModuleEngines>())
            {
                ThrustPosition = AccumulateVectors(part.Modules.GetModule<ModuleEngines>().thrustTransforms);
            }

            // Should be local position relative to the vessels CoM
            PartPosition = part.transform.position - vessel.CoMD;
        }

        private Vector3 AccumulateVectors(List<Transform> vectors)
        {
            // Summing all the thrust vectors should get us the center of thrust for an engine
            // this will probably break on multi-bell RCS engines (??)
            return vectors.Aggregate(Vector3.zero, (a, c) => a + c.localPosition);
        }
    }
}