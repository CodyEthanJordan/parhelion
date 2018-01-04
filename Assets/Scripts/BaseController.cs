using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    class BaseController : Unit
    {
        public Text EtheriumText;
        [SyncVar(hook = "OnChangedEtherium")]
        public float Etherium = 0;

        internal void TankAtHopper(GameObject hit)
        {
            if(!isServer)
            {
                return; //only do this on single server instance
            }

            if(hit.CompareTag("Player"))
            {
                var player = hit.GetComponent<TankController>();
                Etherium += player.ResourceTanks[ResourceType.Etherium];
                player.RpcEmptyEtherium();
            }
        }

        private void Start()
        {
            OnChangedEtherium(0);
        }

        public override void TakeDamage(float damage)
        {
            if (!isServer)
            {
                return;
            }

            Health -= damage;

            if (Health <= 0)
            {
                Destroy(gameObject);
            }
        }

        protected override void OnChangedHealth(float currentHealth)
        {

        }

        private void OnChangedEtherium(float current)
        {
            EtheriumText.text = current + "/";
        }
    }
}
