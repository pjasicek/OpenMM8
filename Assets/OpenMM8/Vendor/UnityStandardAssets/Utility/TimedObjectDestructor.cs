using System;
using UnityEngine;

namespace UnityStandardAssets.Utility
{
    public class TimedObjectDestructor : MonoBehaviour
    {
        public float TimeOut = 1.0f;
        public bool DetachChildren = false;


        private void Awake()
        {
            Invoke("DestroyNow", TimeOut);
        }


        private void DestroyNow()
        {
            if (DetachChildren)
            {
                transform.DetachChildren();
            }
            Destroy(gameObject);
        }
    }
}
