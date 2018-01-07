using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    class UnitSpawner : MonoBehaviour
    {
        public GameObject UnitToSpawn;
        public float SpawnRate = 1f;
        public int MaxSpawns = 5;
        public float SpawnRadius = 5f;

        private float timer = 0f;

        private void Update()
        {
            if (transform.childCount < MaxSpawns)
            {
                timer += Time.deltaTime;
            }

            if (timer > SpawnRadius && transform.childCount < MaxSpawns)
            {
                timer = 0;
                var randomVect = SpawnRadius * UnityEngine.Random.insideUnitCircle;

                var r = Instantiate(UnitToSpawn, (Vector2)this.transform.position + randomVect, Quaternion.identity, this.transform);
                NetworkServer.Spawn(r);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.grey;
            Gizmos.DrawWireSphere(this.transform.position, SpawnRadius);
        }
    }
}
