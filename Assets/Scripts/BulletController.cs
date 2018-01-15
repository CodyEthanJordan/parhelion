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
        public float Damage = 30;

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

        void OnCollisionEnter2D(Collision2D collision)
        {
            var hit = collision.gameObject;
            var unit = collision.gameObject.GetComponent<Unit>();
            if(unit != null)
            {
                Destroy(this.gameObject);
                hit.GetComponent<Unit>().TakeDamage(Damage);
            }

            var otherBullet = collision.gameObject.GetComponent<BulletController>();
            if(otherBullet != null)
            {
                Destroy(this.gameObject);
                Destroy(hit);
            }
        }
    }
}
