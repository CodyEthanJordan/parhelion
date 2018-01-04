using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    class TownPortal : NetworkBehaviour
    {
        public float Timer = 0f;
        public float PortalWindupTime = 1f;
        public float PortalSize = 2f;
        Vector3 destination;

        private void Start()
        {
            destination = GameObject.Find("TownPortalTarget").transform.position;
        }

        private void Update()
        {
            if (!isServer)
            {
                return; // have s
            }

            Timer += Time.deltaTime;

            if (Timer >= PortalWindupTime)
            {
                var AllStuff = Physics2D.OverlapCircleAll(this.transform.position, PortalSize, LayerMask.GetMask("Unit"));
                foreach (var stuff in AllStuff)
                {
                    var networkIdentity = stuff.GetComponent<NetworkIdentity>();
                    if (networkIdentity == null)
                    {
                        Debug.LogError("Why doesn't this object have a network identity? " + stuff.gameObject.name);
                    }
                    else if (networkIdentity.hasAuthority)
                    {
                        stuff.transform.position = destination + stuff.transform.position - this.transform.position;
                    }
                    else
                    {
                        // this is probably a player then
                        var tankController = stuff.GetComponent<TankController>();
                        if (tankController == null)
                        {
                            Debug.LogError("what doesn't the server have authority over which isn't a player? " + stuff.gameObject.name);
                        }
                        else
                        {
                            tankController.RpcTeleportTo(destination + stuff.transform.position - this.transform.position);
                        }
                    }
                }
                Destroy(gameObject);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(this.transform.position, PortalSize);
        }
    }
}
