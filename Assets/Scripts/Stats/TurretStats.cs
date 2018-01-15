using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Assets.Scripts.Stats
{
    [CreateAssetMenu(menuName = "parhelion/EnemyStats/TurretType")]
    class TurretStats : EnemyStats
    {
        public float AimDistance;
        public float ReloadSpeed;
        public float BaseDamage;
        public float BaseCannonBulletVelocity;
    }
}
