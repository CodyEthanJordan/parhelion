using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Assets.Scripts.Stats
{
    [CreateAssetMenu(menuName = "parhelion/Tank/TankSystemPowerups")]
    public class TankSystemPowerups : ScriptableObject
    {
        public float EngineRedCost;
        public float SpeedBoostMultiplier;
        public float EngineGreenCost;
        public float SelfRepairAmount;
    }
}
