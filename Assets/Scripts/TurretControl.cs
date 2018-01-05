using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    class TurretControl : MonoBehaviour
    {
        public GameObject Bullet;
        public float BulletVelocity = 10;
        public float ReloadDelay = 0.5f;

        private Transform bulletSpawnPoint;
        private LineRenderer lr;
        private float lastShot = 0f;

        private void Awake()
        {
            bulletSpawnPoint = transform.GetChild(0);
            lr = GetComponent<LineRenderer>();
        }

        private void Update()
        {
            lastShot += Time.deltaTime;
        }

        //public void FireLaser()
        //{
        //    if (lastShot < ReloadDelay)
        //    {
        //        // still reloading, do nothing
        //        return;
        //    }

        //    var hit = Physics2D.Raycast(bulletSpawnPoint.position, transform.up, 30);
        //    if(hit != null)
        //    {
        //        Debug.Log("shooting laser");
        //        Debug.Log(bulletSpawnPoint.position);
        //        Debug.Log(hit.point);
        //        //RpcLaserEffects(bulletSpawnPoint.position, hit.point);
        //        RpcLaserEffects();
        //        var unit = hit.collider.gameObject.GetComponent<Unit>();
        //        if(unit != null)
        //        {
        //            unit.TakeDamage(10); //TODO: laser damage amount
        //        }
        //    }
        //}

        //[ClientRpc]
        //private void RpcLaserEffects(Vector3 start, Vector3 end)
        //{
        //    Debug.Log("client calls");
        //    lr.enabled = true;
        //    lr.SetPosition(0, start);
        //    lr.SetPosition(1, end);
        //    StartCoroutine(DisableLaser(0.1f));
        //}

          private IEnumerator DisableLaser(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            lr.enabled = false;
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
