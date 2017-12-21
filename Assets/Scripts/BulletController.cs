using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.Scripts
{
    class BulletController : MonoBehaviour
    {
        public float Lifetime = 3;

        private float ttl = 0;

        private void Start()
        {
            
        }

        private void FixedUpdate()
        {
            ttl += Time.deltaTime;

            if(ttl >= Lifetime)
            {
                Destroy(gameObject);
            }
        }
    }
}
