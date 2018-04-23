using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class Gaussian
    {
        private static float _Gaussian()
        {
            float u, v, S;

            do
            {
                u = 2.0f * UnityEngine.Random.Range(0.0f, 1.0f) - 1.0f;
                v = 2.0f * UnityEngine.Random.Range(0.0f, 1.0f) - 1.0f;
                S = u * u + v * v;
            }
            while (S >= 1.0);

            float fac = UnityEngine.Mathf.Sqrt(-2.0f * UnityEngine.Mathf.Log(S) / S);
            return u * fac;
        }

        public static float Random()
        {
            float sigma = 1.0f / 6.0f; // or whatever works.
            while (true)
            {
                float z = _Gaussian() * sigma + 0.5f;
                if (z >= 0.0 && z <= 1.0)
                {
                    return z;
                }
            }
        }

        public static float RandomRange(float min, float max)
        {
            float delta = max - min;
            float rnd = delta * Random();
            return rnd + min;
        }

        public static int RandomRange(int min, int max)
        {
            int delta = max - min;
            int rnd = (int)((float)delta * Random());
            return rnd + min;
        }
    }
}
