using UnityEngine;

namespace MomentOfIntertia
{
    public static class InertiaTensorExtensions
    {
        public static void Accumulate(this ref Matrix4x4 self, Matrix4x4 other)
        {
            for (int i = 0; i < 3; i++)
            {
                self.SetColumn(i, self.GetColumn(i) + other.GetColumn(i));
            }
        }

        public static void Subtract(this ref Matrix4x4 self, Matrix4x4 other)
        {
            for (int i = 0; i < 3; i++)
            {
                self.SetColumn(i, self.GetColumn(i) - other.GetColumn(i));
            }
        }

        public static Matrix4x4 DiagonalTensor(this Vector3 vec)
        {
            var tensor = Matrix4x4.identity;
            for (int i = 0; i < 3; i++)
            {
                tensor[i, i] = vec[i];
            }

            return tensor;
        }

        public static Matrix4x4 DiagonalTensorMatrix(this Vector3 v)
        {
            Matrix4x4 m = Matrix4x4.identity;
            for (int i = 0; i < 3; i++)
            {
                m[i, i] = v[i];
            }
            return m;
        }

        public static Matrix4x4 DiagonalTensor(this float val)
        {
            var tensor = Matrix4x4.identity;
            for (int i = 0; i < 3; i++)
            {
                tensor[i, i] = val;
            }

            return tensor;
        }

        public static Vector3d Trace(this Matrix4x4 tensor)
        {
            Vector3d vec = Vector3d.zero;
            for (int i = 0; i < 3; i++)
            {
                vec[i] = tensor[i, i];
            }

            return vec;
        }

        public static Matrix4x4 Multiply(this Matrix4x4 tensor, float scalar)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    tensor[i, j] = tensor[i, j] * scalar;
                }
            }

            return tensor;
        }
    }
}