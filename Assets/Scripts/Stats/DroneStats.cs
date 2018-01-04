using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Assets.Scripts.Stats
{
    [CreateAssetMenu(menuName = "parhelion/EnemyStats/DroneType")]
    class DroneStats : EnemyStats
    {
        public float ChaseThreshold;
        public float WanderTime;
        public float GiveUpDistanceThreshold;
        public float CollisionDamage;
        public float CollisionSelfDamage;
    }
}
