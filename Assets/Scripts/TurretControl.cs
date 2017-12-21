using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    class TurretControl : NetworkBehaviour
    {
        public GameObject Bullet;
        public float BulletVelocity = 10;
        public float ReloadDelay = 0.5f;

        private Transform bulletSpawnPoint;
        private float lastShot = 0f;

        private void Awake()
        {
            bulletSpawnPoint = transform.GetChild(0);
        }

        private void Update()
        {
            lastShot += Time.deltaTime;
        }

        public void FireCannon()
        {
            if(lastShot < ReloadDelay)
            {
                // still reloading, do nothing
                return;
            }
            lastShot = 0f;
            var bullet = Instantiate(Bullet, bulletSpawnPoint.position, transform.rotation);
            var bulletRB = bullet.GetComponent<Rigidbody2D>();
            bulletRB.AddRelativeForce(new Vector2(0, BulletVelocity), ForceMode2D.Impulse);
            NetworkServer.Spawn(bullet);
        }
    }
}
