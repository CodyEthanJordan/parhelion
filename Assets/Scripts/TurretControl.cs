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

        private Transform bulletSpawnPoint;

        private void Awake()
        {
            bulletSpawnPoint = transform.GetChild(0);
        }

        private void Update()
        {
        }

        public void FireCannon()
        {
            var bullet = Instantiate(Bullet, bulletSpawnPoint.position, transform.rotation);
            var bulletRB = bullet.GetComponent<Rigidbody2D>();
            bulletRB.AddRelativeForce(new Vector2(0, BulletVelocity), ForceMode2D.Impulse);
            NetworkServer.Spawn(bullet);
        }
    }
}
