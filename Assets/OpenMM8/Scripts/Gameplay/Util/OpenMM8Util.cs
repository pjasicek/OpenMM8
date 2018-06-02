using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.OpenMM8.Scripts
{
    public class OpenMM8Util
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
                return default(T);
            }
            else
            {
                return go.GetComponent<T>();
            }
        }
    }
}
