﻿using Assets.Scripts.UI;
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

        public UnityFloatEvent HPChanged;
        public UnityResourceTankEvent ResourcesChanged;

        private GameObject turret;
        private TurretControl turretControl;
        private Rigidbody2D rb;
        private SpriteRenderer sr;
        private float timeOnResource = 0f;

        private UIController uiController;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            turret = transform.GetChild(0).gameObject; //assume turret is only child
            turretControl = turret.GetComponent<TurretControl>();
            sr = GetComponent<SpriteRenderer>();


            HPChanged = new UnityFloatEvent();
            ResourcesChanged = new UnityResourceTankEvent();

            // initialize tanks and set to 0
            ResourceTanks = new Dictionary<ResourceType, float>();
            foreach (ResourceType r in Enum.GetValues(typeof(ResourceType)))
            {
                ResourceTanks.Add(r, 0);
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

        // Update is called once per frame
        void Update()
        {
            if (!isLocalPlayer)
            {
                return;
            }

            var x = Input.GetAxis("Horizontal") * Time.deltaTime * RotationSpeed;
            var z = Input.GetAxis("Vertical") * Time.deltaTime * Speed;

            rb.AddRelativeForce(new Vector2(0, z), ForceMode2D.Impulse);
            rb.AddTorque(-x, ForceMode2D.Impulse);

            // move turret
            var mosPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //turret.transform.LookAt(new Vector3(mosPos.x, mosPos.y, turret.transform.position.z), Vector3.fo);
            var faceDirection = mosPos - turret.transform.position;
            turret.transform.LookAt(turret.transform.position + Vector3.forward, new Vector3(faceDirection.x, faceDirection.y, turret.transform.position.z));


            if (Input.GetMouseButton(0)) //fire the lasers!
            {
                CmdFire();
            }

            if (Input.GetButtonDown("BlueEngine"))
            {
                SystemGrid[ResourceType.Blue][TankSystem.Engine] = !SystemGrid[ResourceType.Blue][TankSystem.Engine];
                uiController.ToggleIndicator("BlueEngine", SystemGrid[ResourceType.Blue][TankSystem.Engine]);
            }
            else if (Input.GetButtonDown("RedEngine"))
            {
                SystemGrid[ResourceType.Red][TankSystem.Engine] = !SystemGrid[ResourceType.Red][TankSystem.Engine];
                uiController.ToggleIndicator("RedEngine", SystemGrid[ResourceType.Red][TankSystem.Engine]);
            }
            else if (Input.GetButtonDown("GreenEngine"))
            {
                SystemGrid[ResourceType.Green][TankSystem.Engine] = !SystemGrid[ResourceType.Green][TankSystem.Engine];
                uiController.ToggleIndicator("GreenEngine", SystemGrid[ResourceType.Green][TankSystem.Engine]);
            }
            else if (Input.GetButtonDown("BlueCannon"))
            {
                SystemGrid[ResourceType.Blue][TankSystem.Cannon] = !SystemGrid[ResourceType.Blue][TankSystem.Cannon];
                uiController.ToggleIndicator("BlueCannon", SystemGrid[ResourceType.Blue][TankSystem.Cannon]);
            }
            else if (Input.GetButtonDown("RedCannon"))
            {
                SystemGrid[ResourceType.Red][TankSystem.Cannon] = !SystemGrid[ResourceType.Red][TankSystem.Cannon];
                uiController.ToggleIndicator("RedCannon", SystemGrid[ResourceType.Red][TankSystem.Cannon]);
            }
            else if (Input.GetButtonDown("GreenCannon"))
            {
                SystemGrid[ResourceType.Green][TankSystem.Cannon] = !SystemGrid[ResourceType.Green][TankSystem.Cannon];
                uiController.ToggleIndicator("GreenCannon", SystemGrid[ResourceType.Green][TankSystem.Cannon]);
            }
            else if (Input.GetButtonDown("BlueForge"))
            {
                SystemGrid[ResourceType.Blue][TankSystem.Forge] = !SystemGrid[ResourceType.Blue][TankSystem.Forge];
                uiController.ToggleIndicator("BlueForge", SystemGrid[ResourceType.Blue][TankSystem.Forge]);
            }
            else if (Input.GetButtonDown("RedForge"))
            {
                SystemGrid[ResourceType.Red][TankSystem.Forge] = !SystemGrid[ResourceType.Red][TankSystem.Forge];
                uiController.ToggleIndicator("RedForge", SystemGrid[ResourceType.Red][TankSystem.Forge]);
            }
            else if (Input.GetButtonDown("GreenForge"))
            {
                SystemGrid[ResourceType.Green][TankSystem.Forge] = !SystemGrid[ResourceType.Green][TankSystem.Forge];
                uiController.ToggleIndicator("GreenForge", SystemGrid[ResourceType.Green][TankSystem.Forge]);
            }

        }

        private void GridUpdate()
        {

        }

        [Command]
        void CmdFire()
        {
            turretControl.FireCannon();
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