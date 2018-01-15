using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.Scripts
{
    class RocketController : MonoBehaviour
    {
        public float Lifetime = 3;
        public float Damage = 30;
        public float AOERadius = 2;

        private float ttl = 0;

        private SpriteRenderer sr;
        private GameObject explosion;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
        }

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

        void OnCollisionEnter2D(Collision2D collision)
        {
            sr.enabled = false;
            var unitsInRange = Physics2D.OverlapCircleAll(this.transform.position, AOERadius, LayerMask.GetMask("Unit"));
            foreach (var u in unitsInRange)
            {
                u.GetComponent<Unit>().TakeDamage(Damage);
                Destroy(this.gameObject);
            }
        }
    }
}
