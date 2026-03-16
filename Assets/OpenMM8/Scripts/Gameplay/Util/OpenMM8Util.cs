using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.OpenMM8.Scripts
{
    public static class MmWorldUtil
    {
        private const float UnitsScale = 100.0f;
        private const float FullTurnDegrees = 360.0f;
        private const float FullTurnDirection = 2048.0f;
        private const float UnityYawOffsetDegrees = 90.0f;

        public static Vector3 ToMmCoordinates(Vector3 unityPosition)
        {
            return new Vector3(
                -unityPosition.x * UnitsScale,
                -unityPosition.z * UnitsScale,
                unityPosition.y * UnitsScale);
        }

        public static Vector3 ToUnityCoordinates(Vector3 mmPosition)
        {
            return new Vector3(
                -mmPosition.x / UnitsScale,
                mmPosition.z / UnitsScale,
                -mmPosition.y / UnitsScale);
        }

        public static float ToMmDirection(float unityYawDegrees)
        {
            return Mathf.Repeat(
                (unityYawDegrees - UnityYawOffsetDegrees) / FullTurnDegrees * FullTurnDirection,
                FullTurnDirection);
        }

        public static float ToUnityYawDegrees(float mmDirection)
        {
            return Mathf.Repeat(
                mmDirection / FullTurnDirection * FullTurnDegrees + UnityYawOffsetDegrees,
                FullTurnDegrees);
        }
    }

    public static class OpenMM8Util
    {
        // Returns a GameObject at specified scene path, e.g. /PartyCanvas/GoldFood. 
        // Can search from root - specified GameObject.
        static public GameObject GetGameObjAtScenePath(string path, GameObject origin = null, char delim = '/')
        {
            if (path.Length > 0 && path[0] == delim)
            {
                path = path.Remove(0, 1);
            }

            GameObject go = origin;

            string[] tree = path.Split(delim);
            foreach (string goName in tree)
            {
                if (go == null)
                {
                    go = GameObject.Find(goName);
                }
                else
                {
                    go = go.transform.Find(goName).gameObject;
                }

                if (go == null)
                {
                    return null;
                }
            }

            return go;
        }

        static public T GetComponentAtScenePath<T>(string path, GameObject origin = null, char delim = '/')
        {
            GameObject go = GetGameObjAtScenePath(path, origin, delim);
            if (go == null)
            {
                Debug.LogError("No gameobject found: " + path);
                return default(T);
            }
            else
            {
                T component = go.GetComponent<T>();
                if (component == null)
                {
                    Debug.LogError("No component found: " + path);
                }

                return component;
            }
        }

        static public void AppendResourcesToMap<TValue>(Dictionary<string, TValue> map, string path, bool keyToLower = true) 
            where TValue : UnityEngine.Object
        {
            TValue[] resources = Resources.LoadAll<TValue>(path);
            foreach (TValue res in resources)
            {
                map.Add(res.name.ToLower(), res);
            }
        }

        
        static public K GetRandomKey<K, V>(Dictionary<K, V> dict)
        {
            int numElements = dict.Count;
            return dict.Keys.ToList()[UnityEngine.Random.Range(0, numElements)];
        }

        public static long ElapsedNanoSeconds(this System.Diagnostics.Stopwatch watch)
        {
            return watch.ElapsedTicks * 1000000000 / System.Diagnostics.Stopwatch.Frequency;
        }

        public static long ElapsedMicroSeconds(this System.Diagnostics.Stopwatch watch)
        {
            return watch.ElapsedTicks * 1000000 / System.Diagnostics.Stopwatch.Frequency;
        }
    }
}
