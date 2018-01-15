﻿using Assets.Scripts.Stats;
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
        public GameObject BulletSpawnPoint;
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
            if (isServer == false)
            {
                return; //only run AI on server
            }

            lastShot += Time.deltaTime;
            var stuff = Physics2D.OverlapCircleAll(this.transform.position, stats.AimDistance, LayerMask.GetMask("Unit"));
            if (stuff != null && stuff.Length > 0)
            {
                var closestFoe = stuff.OrderBy(s => Vector2.Distance(this.transform.position, s.transform.position))
                .FirstOrDefault(s => s.GetComponent<Unit>().Side != this.Side);

                if (closestFoe != null && lastShot >= stats.ReloadSpeed)
                {
                    lastShot = 0;
                    CmdShootAt(closestFoe.transform.position);
                }
            }

        }

        [Command]
        private void CmdShootAt(Vector3 position)
        {
            Turret.transform.LookAt(Turret.transform.position + Vector3.up, position);
            var bullet = Instantiate(BulletPrefab, BulletSpawnPoint.transform.position, Turret.transform.rotation);
            var bulletRB = bullet.GetComponent<Rigidbody2D>();
            bullet.GetComponent<BulletController>().Damage = stats.BaseDamage;
            bulletRB.AddRelativeForce(new Vector2(0, stats.BaseCannonBulletVelocity), ForceMode2D.Impulse);
            NetworkServer.Spawn(bullet);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.transform.position, stats.AimDistance);
        }
    }
}
