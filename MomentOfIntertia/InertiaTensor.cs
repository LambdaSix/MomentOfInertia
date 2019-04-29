using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Smooth.Algebraics;
using UnityEngine;
using KSP;

namespace MomentOfIntertia
{
    public static class InertiaTensor
    {
        public static Matrix4x4 FromScalar(float val) => val.DiagonalTensor();

        public static Matrix4x4 OuterProduct(Vector3 self, Vector3 other)
        {
            var tensor = Matrix4x4.identity;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    tensor[i, j] = self[i] * other[j];
                }
            }

            return tensor;
        }

        public static Matrix4x4 Compute(Vessel v)
        {
            var inertiaTensor = Matrix4x4.zero;
            Vector3 com = v.findWorldCenterOfMass();
            Transform vesselTransform = v.GetTransform();

            foreach (var part in v.parts)
            {
                if (part.rb != null)
                {
                    var partTensor = part.rb.inertiaTensor.DiagonalTensor();

                    // translate - intertiaTensor frame to part frame, part frame to world frame, world frame to vessel frame
                    // yay translations..
                    var rotation = Quaternion.Inverse(vesselTransform.rotation)
                                     * part.transform.rotation
                                     * part.rb.inertiaTensorRotation;
                    var rotationInverse = Quaternion.Inverse(rotation);

                    var rotationMatrix = Matrix4x4.TRS(Vector3.zero, rotation, Vector3.one);
                    var inverseMatrix = Matrix4x4.TRS(Vector3.zero, rotationInverse, Vector3.one);

                    // add the part inertiaTensor to the ships inertiaTensor
                    inertiaTensor.Accumulate(rotationMatrix * partTensor * inverseMatrix);
                    Vector3 position = vesselTransform.InverseTransformDirection(part.rb.position - com);

                    // Add the part mass to the ships' inertiaTensor
                    inertiaTensor.Accumulate((part.rb.mass * position.sqrMagnitude).DiagonalTensor());

                    // Then offset the part from the ship
                    inertiaTensor.Accumulate(OuterProduct(-part.rb.mass * position, position));
                }
            }

            // Convert to kg-m^2
            return inertiaTensor.Multiply(1000f);
        }
    }
}