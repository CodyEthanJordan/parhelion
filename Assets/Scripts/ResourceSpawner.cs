using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    class ResourceSpawner : MonoBehaviour
    {
        public GameObject ResourceToSpawn;
        public float SpawnRate = 1f;
        public int MaxSpawns = 5;
        public float SpawnRadius = 5f;

        private float timer = 0f;

        private void Update()
        {
            timer += Time.deltaTime;
            if(timer > SpawnRadius && transform.childCount < MaxSpawns)
            {
                timer = 0;
                var randomVect = SpawnRadius * UnityEngine.Random.insideUnitCircle;

                var r = Instantiate(ResourceToSpawn, randomVect, Quaternion.identity, this.transform);
                NetworkServer.Spawn(r);

            }
        }


    }
}
