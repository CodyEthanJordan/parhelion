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
        public float EngineGreenCost;
        public float EngineBlueCost;
        public float EngineRedGreenCost;
        public float EngineRedBlueCost;
        public float EngineGreenBlueCost;
        public float EngineRedGreenBlueCost;
        public float CannonRedCost;
        public float CannonGreenCost;
        public float CannonBlueCost;
        public float CannonRedGreenCost;
        public float CannonRedBlueCost;
        public float CannonGreenBlueCost;
        public float CannonRedGreenBlueCost;
        public float ForgeRedCost;
        public float ForgeGreenCost;
        public float ForgeBlueCost;
        public float ForgeRedGreenCost;
        public float ForgeRedBlueCost;
        public float ForgeGreenBlueCost;
        public float ForgeRedGreenBlueCost;

        public float BaseCannonDamage;
        public float BaseCannonReloadSpeed;
        public float BaseCannonBulletVelocity;
        public GameObject BulletPrefab;

        public float MachineGunDamage;
        public float MachineGunReloadSpeed;

        public GameObject HealSprayPrefab;
        public float HealAmount;
        public float HealSprayReloadTime;

        public GameObject RocketPrefab;
        public float RocketAOERadius;
        public float RocketDamage;

        public float DrillBeamReloadSpeed;
        public float DrillBeamDamage;
        public float DrillBeamRange;

        public float SelfRepairAmount;
        public float SpeedBoostMultiplier;
        public float LaserDamage;
        public float LaserReloadSpeed;
        public float LaserRange;

        public GameObject TurretPrefab;
    }
}
