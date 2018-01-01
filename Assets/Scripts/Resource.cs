using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    public enum ResourceType
    {
        Etherium,
        Blue,
        Red,
        Green
    }

    class Resource : NetworkBehaviour
    {
        [SyncVar(hook = "OnChangeAmount")]
        public int Amount = 3;

        private SpriteRenderer sr;

        private ResourceType _type;
        public ResourceType Type
        {
            get { return _type; }
            set
            {
                _type = value;
                UpdateColor();
            }
        }

        private void UpdateColor()
        {
            if (sr == null)
            {
                return;
            }
            
            switch(Type)
            {
                case ResourceType.Blue:
                    sr.color = Color.blue;
                    break;
                case ResourceType.Red:
                    sr.color = Color.red;
                    break;
                case ResourceType.Green:
                    sr.color = Color.green;
                    break;
                case ResourceType.Etherium:
                    sr.color = Color.magenta;
                    break;
            }
        }

        private Animator anim;

        private void Awake()
        {
            anim = GetComponent<Animator>();
            sr = GetComponent<SpriteRenderer>();
        }

        public void CollectResouce()
        {
            if (!isServer)
            {
                return;
            }

            Amount -= 1;

            if (Amount <= 0)
            {
                Destroy(gameObject);
            }
        }

        void OnChangeAmount(int newAmount)
        {
            anim.SetTrigger("Decay");
        }
    }
}
