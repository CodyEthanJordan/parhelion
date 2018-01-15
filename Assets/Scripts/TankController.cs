using Assets.Scripts.Stats;
using Assets.Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    [System.Serializable]
    public class UnityFloatEvent : UnityEvent<float, float>
    {
    }

    [System.Serializable]
    public class UnityResourceTankEvent : UnityEvent<Dictionary<ResourceType, float>, float>
    {
    }

    public enum TankSystem
    {
        Engine,
        Cannon,
        Forge
    }

    public class TankController : Unit
    {
        public float RotationSpeed = 150.0f;
        public float CollectionTime = 0.1f;
        public Dictionary<ResourceType, float> ResourceTanks;
        public Dictionary<ResourceType, Dictionary<TankSystem, bool>> SystemGrid;
        public float TankCapacity = 6;
        public float ReloadDelay = 0.5f;
        public GameObject BulletPrefab;


        [SerializeField] private TankSystemPowerups powerupStats;

        public UnityFloatEvent HPChanged;
        public UnityResourceTankEvent ResourcesChanged;

        public GameObject TownPortalPrefab;


        private GameObject turret;
        private Transform bulletSpawnPoint;
        private LineRenderer lr;
        private Rigidbody2D rb;
        private SpriteRenderer sr;
        private float timeOnResource = 0f;
        private float lastShot;

        private UIController uiController;

        private GameObject _homeBase;
        private GameObject HomeBase
        {
            get
            {
                if(_homeBase == null)
                {
                    _homeBase = GameObject.Find("HomeBase");
                }
                return _homeBase;
            }
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            turret = transform.GetChild(0).gameObject; //assume turret is only child
            bulletSpawnPoint = turret.transform.GetChild(0);
            sr = GetComponent<SpriteRenderer>();
            lr = turret.GetComponent<LineRenderer>();

            HPChanged = new UnityFloatEvent();
            ResourcesChanged = new UnityResourceTankEvent();

            // initialize tanks and set to 0
            ResourceTanks = new Dictionary<ResourceType, float>();
            foreach (ResourceType r in Enum.GetValues(typeof(ResourceType)))
            {
                ResourceTanks.Add(r, 6);
            }

            SystemGrid = new Dictionary<ResourceType, Dictionary<TankSystem, bool>>();
            foreach (ResourceType r in Enum.GetValues(typeof(ResourceType)))
            {
                SystemGrid.Add(r, new Dictionary<TankSystem, bool>());
                foreach (TankSystem sys in Enum.GetValues(typeof(TankSystem)))
                {
                    SystemGrid[r].Add(sys, false);
                }
            }
        }

        // Use this for initialization
        void Start()
        {
            if (isLocalPlayer)
            {
                var cinemachine = GameObject.FindGameObjectWithTag("Cinemachine");
                cinemachine.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = transform;

                var canvas = GameObject.FindGameObjectWithTag("UI");
                uiController = canvas.GetComponent<UIController>();
                HPChanged.AddListener(uiController.HPUpdate);
                HPChanged.Invoke(Health, MaxHealth);

                ResourcesChanged.AddListener(uiController.ResourceUpdate);
                ResourcesChanged.Invoke(ResourceTanks, TankCapacity);
            }
        }

        private bool SystemActive(TankSystem tankSystem, bool blue, bool red, bool green)
        {
            return SystemGrid[ResourceType.Blue][tankSystem] == blue &&
                SystemGrid[ResourceType.Red][tankSystem] == red &&
                SystemGrid[ResourceType.Green][tankSystem] == green;
        }

        private bool SpendResources(ResourceType resourceType, float amount)
        {
            if (ResourceTanks[resourceType] < amount)
            {
                return false;
            }
            ResourceTanks[resourceType] -= amount;
            ResourcesChanged.Invoke(ResourceTanks, TankCapacity);
            ValidateSuffecientResourceCounts();
            return true;
        }

        private bool SpendResources(float amount, bool blue, bool red, bool green)
        {
            if (blue && ResourceTanks[ResourceType.Blue] < amount)
            {
                return false;
            }
            if (red && ResourceTanks[ResourceType.Red] < amount)
            {
                return false;
            }
            if (green && ResourceTanks[ResourceType.Green] < amount)
            {
                return false;
            }

            if (blue)
            {
                ResourceTanks[ResourceType.Blue] -= amount;
            }
            if (red)
            {
                ResourceTanks[ResourceType.Red] -= amount;
            }
            if (green)
            {
                ResourceTanks[ResourceType.Green] -= amount;
            }

            ResourcesChanged.Invoke(ResourceTanks, TankCapacity);
            ValidateSuffecientResourceCounts();
            return true;
        }

        // Update is called once per frame
        void Update()
        {
            lastShot += Time.deltaTime;
            if (!isLocalPlayer)
            {
                return;
            }

            ValidateSuffecientResourceCounts();

            var x = Input.GetAxis("Horizontal") * Time.deltaTime * RotationSpeed;
            var z = Input.GetAxis("Vertical") * Time.deltaTime * Speed;

            // POWERUP: scanner
            if (SystemActive(TankSystem.Engine, blue: true, red: false, green: false)
               && SpendResources(ResourceType.Blue, Time.deltaTime * powerupStats.EngineBlueCost))
            {
                var directionToBase = (HomeBase.transform.position - this.transform.position).normalized;
                Debug.Log(directionToBase);
                uiController.DirectionToBase(directionToBase);
            }
            else
            {
                //disable overlay
                uiController.DisableScanner();
            }

            // POWERUP: speed boost
            if (SystemActive(TankSystem.Engine, blue: false, red: true, green: false)
                && z != 0
                && SpendResources(ResourceType.Red, Time.deltaTime * powerupStats.EngineRedCost))
            {
                z *= powerupStats.SpeedBoostMultiplier;
            }

            // POWERUP: repair
            if (SystemActive(TankSystem.Engine, blue: false, red: false, green: true) && Health < MaxHealth)
            {
                SpendResources(ResourceType.Green, powerupStats.EngineGreenCost * Time.deltaTime);
                Health = Math.Min(MaxHealth, Health + Time.deltaTime * powerupStats.SelfRepairAmount);
            }

            // POWERUP: hovertank
            if (SystemActive(TankSystem.Engine, blue: true, red: true, green: true)
                && SpendResources(Time.deltaTime * powerupStats.EngineRedGreenBlueCost, blue: true, red: true, green: true))
            {

                var y = Input.GetAxis("Vertical") * Time.deltaTime * powerupStats.HoverSpeed;
                x = Input.GetAxis("Horizontal") * Time.deltaTime * powerupStats.HoverSpeed;
                rb.AddForce(new Vector2(x, y), ForceMode2D.Force);
            }
            else
            {
                //regular movement
                rb.AddRelativeForce(new Vector2(0, z), ForceMode2D.Impulse);
                rb.AddTorque(-x, ForceMode2D.Impulse);
            }


            // move turret
            var mosPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            var faceDirection = mosPos - turret.transform.position;
            turret.transform.LookAt(turret.transform.position + Vector3.forward, new Vector3(faceDirection.x, faceDirection.y, turret.transform.position.z));

            if (Input.GetMouseButton(0)) //fire the lasers!
            {
                if (SystemActive(TankSystem.Cannon, blue: false, red: true, green: false)
                    && lastShot >= powerupStats.LaserReloadSpeed
                    && SpendResources(ResourceType.Red, powerupStats.CannonRedCost))
                {
                    lastShot = 0f;
                    CmdFireLaser();
                }
                else if (SystemActive(TankSystem.Cannon, blue: false, red: false, green: true)
                    && lastShot >= powerupStats.HealSprayReloadTime
                    && SpendResources(ResourceType.Green, powerupStats.CannonGreenCost))
                {
                    lastShot = 0f;
                    CmdHealBeam();
                }
                else if (SystemActive(TankSystem.Cannon, blue: true, red: false, green: false)
                    && lastShot >= powerupStats.MachineGunReloadSpeed
                    && SpendResources(ResourceType.Green, powerupStats.CannonBlueCost))
                {
                    lastShot = 0f;
                    CmdMachineGun();
                }
                else if (SystemActive(TankSystem.Cannon, blue: false, red: true, green: true)
                    && lastShot >= powerupStats.RocketReloadSpeed
                    && SpendResources(powerupStats.CannonRedGreenCost, blue: false, red: true, green: true))
                {
                    lastShot = 0f;
                    CmdFireRocket();
                }
                else if (SystemActive(TankSystem.Cannon, blue: true, red: true, green: false)
                    && lastShot >= powerupStats.BaseCannonReloadSpeed
                    && SpendResources(powerupStats.CannonRedGreenCost, blue: true, red: true, green: false))
                {
                    lastShot = 0f;
                    CmdTripleShot();
                }
                else if (SystemActive(TankSystem.Cannon, blue: true, red: false, green: true)
                    && lastShot >= powerupStats.DrillBeamReloadSpeed
                    && SpendResources(powerupStats.CannonRedGreenCost, blue: true, red: false, green: true))
                {
                    lastShot = 0f;
                    CmdDrillBeam();
                }
                else if (SystemActive(TankSystem.Cannon, blue: true, red: true, green: true)
                    && lastShot >= powerupStats.DeathRayReloadSpeed
                    && SpendResources(powerupStats.CannonRedGreenCost, blue: true, red: true, green: true))
                {
                    lastShot = 0f;
                    CmdDeathRay();
                }
                else if (SystemActive(TankSystem.Cannon, blue: false, red: false, green: false)
                    && lastShot >= powerupStats.BaseCannonReloadSpeed)
                {
                    lastShot = 0f;
                    CmdFire();
                }

            }

            #region System Button Input Processing
            if (Input.GetButtonDown("BlueEngine"))
            {
                SystemGrid[ResourceType.Blue][TankSystem.Engine] = !SystemGrid[ResourceType.Blue][TankSystem.Engine];
                uiController.UpdateSystemDisplay(SystemGrid);
            }
            else if (Input.GetButtonDown("RedEngine"))
            {
                SystemGrid[ResourceType.Red][TankSystem.Engine] = !SystemGrid[ResourceType.Red][TankSystem.Engine];
                uiController.UpdateSystemDisplay(SystemGrid);
            }
            else if (Input.GetButtonDown("GreenEngine"))
            {
                SystemGrid[ResourceType.Green][TankSystem.Engine] = !SystemGrid[ResourceType.Green][TankSystem.Engine];
                uiController.UpdateSystemDisplay(SystemGrid);
            }
            else if (Input.GetButtonDown("BlueCannon"))
            {
                SystemGrid[ResourceType.Blue][TankSystem.Cannon] = !SystemGrid[ResourceType.Blue][TankSystem.Cannon];
                uiController.UpdateSystemDisplay(SystemGrid);
            }
            else if (Input.GetButtonDown("RedCannon"))
            {
                SystemGrid[ResourceType.Red][TankSystem.Cannon] = !SystemGrid[ResourceType.Red][TankSystem.Cannon];
                uiController.UpdateSystemDisplay(SystemGrid);
            }
            else if (Input.GetButtonDown("GreenCannon"))
            {
                SystemGrid[ResourceType.Green][TankSystem.Cannon] = !SystemGrid[ResourceType.Green][TankSystem.Cannon];
                uiController.UpdateSystemDisplay(SystemGrid);
            }
            else if (Input.GetButtonDown("BlueForge"))
            {
                SystemGrid[ResourceType.Blue][TankSystem.Forge] = !SystemGrid[ResourceType.Blue][TankSystem.Forge];
                uiController.UpdateSystemDisplay(SystemGrid);
            }
            else if (Input.GetButtonDown("RedForge"))
            {
                SystemGrid[ResourceType.Red][TankSystem.Forge] = !SystemGrid[ResourceType.Red][TankSystem.Forge];
                uiController.UpdateSystemDisplay(SystemGrid);
            }
            else if (Input.GetButtonDown("GreenForge"))
            {
                SystemGrid[ResourceType.Green][TankSystem.Forge] = !SystemGrid[ResourceType.Green][TankSystem.Forge];
                uiController.UpdateSystemDisplay(SystemGrid);
            }
            #endregion

            // use forge
            if (Input.GetButtonDown("UseForge") || Input.GetMouseButtonDown(1))
            {
                if (SystemActive(TankSystem.Forge, blue: false, green: true, red: false)
                    && (Physics2D.OverlapCircle(mosPos, 0.5f, LayerMask.GetMask("Terrain")) == null)
                    && SpendResources(ResourceType.Green, powerupStats.ForgeGreenCost)) //TODO: can teleport in to walls
                {
                    transform.position = mosPos;
                }
                else if (SystemActive(TankSystem.Forge, blue: true, green: true, red: true)
                    && SpendResources(powerupStats.ForgeRedGreenBlueCost, blue: true, green: true, red: true))
                {
                    CmdSpawnTownPortal();
                }
                else if (SystemActive(TankSystem.Forge, blue: true, green: true, red: false)
                    && (Physics2D.OverlapCircle(mosPos, 0.5f, LayerMask.GetMask("Terrain")) == null
                    && Physics2D.Raycast(this.transform.position, turret.transform.up, Vector2.Distance(this.transform.position, mosPos), LayerMask.GetMask("Terrain")).collider == null
                    && SpendResources(powerupStats.ForgeGreenBlueCost, blue: true, green: true, red: false)))
                {
                    CmdMakeTurret(mosPos);
                }

            }

        }

        #region Cannon Commands
        [Command]
        private void CmdDeathRay()
        {
            ShootLaser(powerupStats.DeathRayRange, powerupStats.DeathRayDamage, 0.33f, Color.magenta);
        }

        [Command]
        private void CmdDrillBeam()
        {
            ShootLaser(powerupStats.DrillBeamRange, powerupStats.DrillBeamDamage, 0.22f, Color.blue);
        }

        [Server]
        private void ShootLaser(float range, float damage, float width, Color color)
        {
            var hit = Physics2D.Raycast(bulletSpawnPoint.position, turret.transform.up, range, LayerMask.GetMask("Unit", "Terrain"));
            if (hit.collider != null)
            {
                RpcLaserEffects(bulletSpawnPoint.position, hit.point, color, width);
                var unit = hit.collider.gameObject.GetComponent<Unit>();
                if (unit != null)
                {
                    unit.TakeDamage(damage); //TODO: laser damage amount
                }
            }
            else
            {
                RpcLaserEffects(bulletSpawnPoint.position, bulletSpawnPoint.position + range * bulletSpawnPoint.up, color, width);
            }
        }

        [Command]
        private void CmdTripleShot()
        {
            lastShot = 0f;
            for (int i = -1; i < 2; i++)
            {
                var bullet = Instantiate(powerupStats.BulletPrefab, bulletSpawnPoint.position + i * turret.transform.right, turret.transform.rotation);
                var bulletRB = bullet.GetComponent<Rigidbody2D>();
                bullet.GetComponent<BulletController>().Damage = powerupStats.BaseCannonDamage;
                bulletRB.AddRelativeForce(new Vector2(i * powerupStats.BaseCannonBulletVelocity / 4, powerupStats.BaseCannonBulletVelocity), ForceMode2D.Impulse);
                NetworkServer.Spawn(bullet);
            }

        }

        [Command]
        private void CmdFireRocket()
        {
            var bullet = Instantiate(powerupStats.RocketPrefab, bulletSpawnPoint.position, turret.transform.rotation);
            var bulletRB = bullet.GetComponent<Rigidbody2D>();
            var rocketScript = bullet.GetComponent<RocketController>();
            rocketScript.Damage = powerupStats.RocketDamage;
            rocketScript.AOERadius = powerupStats.RocketAOERadius;
            bulletRB.AddRelativeForce(new Vector2(0, powerupStats.BaseCannonBulletVelocity), ForceMode2D.Impulse);
            NetworkServer.Spawn(bullet);
        }

        [Command]
        private void CmdMachineGun()
        {
            lastShot = 0f;
            var bullet = Instantiate(powerupStats.MachineGunPrefab, bulletSpawnPoint.position, turret.transform.rotation);
            var bulletRB = bullet.GetComponent<Rigidbody2D>();
            bullet.GetComponent<BulletController>().Damage = powerupStats.MachineGunDamage;
            bulletRB.AddRelativeForce(new Vector2(0, powerupStats.BaseCannonBulletVelocity), ForceMode2D.Impulse);
            NetworkServer.Spawn(bullet);
        }

        [Command]
        private void CmdHealBeam()
        {
            lastShot = 0f;
            var bullet = Instantiate(powerupStats.HealSprayPrefab, bulletSpawnPoint.position, turret.transform.rotation);
            var bulletRB = bullet.GetComponent<Rigidbody2D>();
            bullet.GetComponent<BulletController>().Damage = -powerupStats.HealAmount;
            bulletRB.AddRelativeForce(new Vector2(UnityEngine.Random.Range(-powerupStats.BaseCannonBulletVelocity / 2, powerupStats.BaseCannonBulletVelocity / 2), powerupStats.BaseCannonBulletVelocity), ForceMode2D.Impulse);
            NetworkServer.Spawn(bullet);
        }



        [Command]
        private void CmdFire()
        {
            lastShot = 0f;
            var bullet = Instantiate(powerupStats.BulletPrefab, bulletSpawnPoint.position, turret.transform.rotation);
            var bulletRB = bullet.GetComponent<Rigidbody2D>();
            bullet.GetComponent<BulletController>().Damage = powerupStats.BaseCannonDamage;
            bulletRB.AddRelativeForce(new Vector2(0, powerupStats.BaseCannonBulletVelocity), ForceMode2D.Impulse);
            NetworkServer.Spawn(bullet);
        }

        [Command]
        private void CmdFireLaser()
        {
            ShootLaser(powerupStats.LaserRange, powerupStats.LaserDamage, 0.11f, Color.red);
        }

        [ClientRpc]
        private void RpcLaserEffects(Vector3 start, Vector3 end, Color color, float width)
        {
            lr.startColor = color;
            lr.endColor = color;
            lr.startWidth = width;
            lr.endWidth = width;
            lr.enabled = true;
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            StartCoroutine(DisableLaser(0.1f));
        }
        #endregion

        #region Forge Commands
        [Command]
        private void CmdMakeTurret(Vector3 mosPos)
        {
            var turret = Instantiate(powerupStats.TurretPrefab, new Vector3(mosPos.x, mosPos.y, 0), Quaternion.identity);
            NetworkServer.Spawn(turret);
        }

        [Command]
        void CmdSpawnTownPortal()
        {
            var portal = Instantiate(TownPortalPrefab, this.transform.position, Quaternion.identity);
            NetworkServer.Spawn(portal);
        }

        [ClientRpc]
        public void RpcTeleportTo(Vector3 pos)
        {
            this.transform.position = pos;
        }
        #endregion

        private void ValidateSuffecientResourceCounts()
        {
            //check resource counts
            if (ResourceTanks[ResourceType.Blue] <= 0)
            {
                ResourceTanks[ResourceType.Blue] = 0;
                SystemGrid[ResourceType.Blue][TankSystem.Engine] = false;
                SystemGrid[ResourceType.Blue][TankSystem.Cannon] = false;
                SystemGrid[ResourceType.Blue][TankSystem.Forge] = false;
                uiController.UpdateSystemDisplay(SystemGrid);
            }
            if (ResourceTanks[ResourceType.Green] <= 0)
            {
                ResourceTanks[ResourceType.Green] = 0;
                SystemGrid[ResourceType.Green][TankSystem.Engine] = false;
                SystemGrid[ResourceType.Green][TankSystem.Cannon] = false;
                SystemGrid[ResourceType.Green][TankSystem.Forge] = false;
                uiController.UpdateSystemDisplay(SystemGrid);
            }
            if (ResourceTanks[ResourceType.Red] <= 0)
            {
                ResourceTanks[ResourceType.Red] = 0;
                SystemGrid[ResourceType.Red][TankSystem.Engine] = false;
                SystemGrid[ResourceType.Red][TankSystem.Cannon] = false;
                SystemGrid[ResourceType.Red][TankSystem.Forge] = false;
                uiController.UpdateSystemDisplay(SystemGrid);
            }
        }


        private IEnumerator DisableLaser(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            lr.enabled = false;
        }


        public override void TakeDamage(float amount)
        {
            if (!isServer)
            {
                return;
            }

            Health -= amount;

            if (Health <= 0)
            {
                Health = MaxHealth;
                RpcRespawn();
            }
            else if (Health > MaxHealth)
            {
                Health = MaxHealth;
            }
        }

        protected override void OnChangedHealth(float currentHealth)
        {
            sr.color = new Color(1, currentHealth / MaxHealth, currentHealth / MaxHealth);
            HPChanged.Invoke(currentHealth, MaxHealth);
        }

        [ClientRpc]
        void RpcRespawn()
        {
            if (isLocalPlayer)
            {
                // move back to zero location
                transform.position = Vector3.zero;
                foreach (var resource in ResourceTanks)
                {
                    ResourceTanks[resource.Key] = 0;
                }
                ResourcesChanged.Invoke(ResourceTanks, TankCapacity);
            }
        }

        [ClientRpc]
        public void RpcEmptyEtherium()
        {
            if (isLocalPlayer)
            {
                ResourceTanks[ResourceType.Etherium] = 0;
                ResourcesChanged.Invoke(ResourceTanks, TankCapacity);
            }
        }

        private bool IsFull(ResourceType r)
        {
            return ResourceTanks[r] >= TankCapacity;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (!isLocalPlayer)
            {
                return; //only local players handle gathering resources
            }
            if (collision.gameObject.CompareTag("Resource"))
            {
                timeOnResource += Time.deltaTime;

                if (timeOnResource >= CollectionTime)
                {
                    timeOnResource = 0f;
                    var resource = collision.gameObject.GetComponent<Resource>();

                    if (!IsFull(resource.Type))
                    {
                        resource.CollectResouce();
                        ResourceTanks[resource.Type] += 1;
                        ResourcesChanged.Invoke(ResourceTanks, TankCapacity);
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            timeOnResource = 0f;
        }

    }
}