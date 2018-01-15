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

        protected SpriteRenderer sr;

        protected virtual void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
        }

        public virtual void TakeDamage(float damage)
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
            else if (Health > MaxHealth)
            {
                Health = MaxHealth;
            }
        }

        protected virtual void OnChangedHealth(float currentHealth)
        {
            sr.color = new Color(1, currentHealth / MaxHealth, currentHealth / MaxHealth);
        }

        protected GameObject FindUnit(float distance, Func<GameObject, bool> filter)
        {
            var stuff = Physics2D.OverlapCircleAll(this.transform.position, distance, LayerMask.GetMask("Unit"));
            if (stuff != null && stuff.Length > 0)
            {
                var closestFoe = stuff.OrderBy(s => Vector2.Distance(this.transform.position, s.transform.position))
                .FirstOrDefault(c => filter(c.gameObject));

                if (closestFoe == null)
                {
                    return null;
                }

                return closestFoe.gameObject;
            }
            else
            {
                return null;
            }
        }
    }
}
