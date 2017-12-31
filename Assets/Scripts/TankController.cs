using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    public class TankController : NetworkBehaviour
    {
        public float RotationSpeed = 150.0f;
        public float Speed = 3.0f;
        public float MaxHealth = 100.0f;
        [SyncVar(hook = "OnChangeHealth")]
        public float Health = 100.0f;
        public float CollectionTime = 0.1f;

        private GameObject turret;
        private TurretControl turretControl;
        private Rigidbody2D rb;
        private SpriteRenderer sr;
        private float timeOnResource = 0f;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            turret = transform.GetChild(0).gameObject; //assume turret is only child
            turretControl = turret.GetComponent<TurretControl>();
            sr = GetComponent<SpriteRenderer>();
        }

        // Use this for initialization
        void Start()
        {
            if (isLocalPlayer)
            {
                var cinemachine = GameObject.FindGameObjectWithTag("Cinemachine");
                cinemachine.GetComponent<Cinemachine.CinemachineVirtualCamera>().Follow = transform;
            }
        }

        // Update is called once per frame
        void FixedUpdate()
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


            if (Input.GetMouseButtonDown(0)) //fire the lasers!
            {
                CmdFire();
            }
        }

        [Command]
        void CmdFire()
        {
            turretControl.FireCannon();
        }

        public void TakeDamage(float amount)
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

        void OnChangeHealth(float currentHealth)
        {
            sr.color = new Color(1, currentHealth / MaxHealth, currentHealth / MaxHealth); //TODO: make baased off max hp, make better UX
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

        private void OnTriggerEnter2D(Collider2D collision)
        {

        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Resource"))
            {
                timeOnResource += Time.deltaTime;

                if (timeOnResource >= CollectionTime)
                {
                    timeOnResource = 0f;
                    var resource = collision.gameObject.GetComponent<Resource>();
                    resource.CollectResouce();
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            timeOnResource = 0f;
        }

    }
}