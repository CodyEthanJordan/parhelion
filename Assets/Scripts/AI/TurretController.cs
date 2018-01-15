using Assets.Scripts.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.AI
{
    class TurretController : Unit
    {
        private SpriteRenderer sr;
        private Rigidbody2D rb;

        public GameObject Turret;
        public GameObject ShootPoint;
        public GameObject BulletPrefab;
        public TurretStats stats;

        private float lastShot = 0f;


        private void Awake()
        {
            Side = Alignment.GoodGuy;
            sr = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            Health = stats.MaxHealth;
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
            sr.color = new Color(1, currentHealth / MaxHealth, currentHealth / MaxHealth);
        }

        private void Update()
        {
            lastShot += Time.deltaTime;
            var stuff = Physics2D.OverlapCircleAll(this.transform.position, stats.AimDistance);
            var closestFoe = stuff.OrderBy(s => Vector2.Distance(this.transform.position, s.transform.position))
                .FirstOrDefault(s => s.GetComponent<Unit>().Side != this.Side);

            if (closestFoe != null && lastShot >= stats.ReloadSpeed)
            {
                CmdShootAt(closestFoe.transform.position);
            }
        }

        [Command]
        private void CmdShootAt(Vector3 position)
        {
            
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.position, stats.AimDistance);
        }
    }
}
