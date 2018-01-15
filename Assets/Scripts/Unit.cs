using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    public enum Alignment
    {
        BadGuys,
        GoodGuy
    }

    public abstract class Unit : NetworkBehaviour
    {
        [SyncVar(hook = "OnChangedHealth")]
        public float Health = 100.0f;
        public float Speed = 3.0f;
        public float MaxHealth = 100.0f;
        public Alignment Side;


        protected abstract void OnChangedHealth(float currentHealth);
        public abstract void TakeDamage(float damage);
    }
}
