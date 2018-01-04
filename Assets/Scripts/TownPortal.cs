using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    class TownPortal : MonoBehaviour
    {
        public float Timer = 0f;
        public float PortalWindupTime = 1f;
        Vector3 destination;

        private void Start()
        {
            destination = GameObject.Find("TownPortalTarget").transform.position;
        }

        private void Update()
        {
            Timer += Time.deltaTime;

            if (Timer >= PortalWindupTime)
            {
                var stuff = Physics2D.OverlapCircle(this.transform.position, 2, LayerMask.GetMask("Unit"));
                while (stuff != null)
                {
                    stuff.transform.position = destination + (Vector3)UnityEngine.Random.insideUnitCircle;
                    stuff = Physics2D.OverlapCircle(this.transform.position, 2, LayerMask.GetMask("Unit"));
                }
                Destroy(gameObject);
            }
        }
    }
}
