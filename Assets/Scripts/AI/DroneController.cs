using Assets.Scripts.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.AI
{
    enum DroneState
    {
        Wander,
        Chase
    }

    class DroneController : Unit
    {
        private SpriteRenderer sr;
        private Rigidbody2D rb;
        [SerializeField] private DroneState state = DroneState.Wander;
        private float xMove = 0f;
        private float yMove = 0f;
        public DroneStats stats;

        private void Awake()
        {
            sr = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            Health = stats.MaxHealth;
        }

        public override void TakeDamage(float damage)
        {
            if (!isServer)
            {
                return;
            }

            Health -= damage;

            if (Health <= 0)
            {
                Destroy(gameObject);
            }
        }

        protected override void OnChangedHealth(float currentHealth)
        {
            sr.color = new Color(1, currentHealth / MaxHealth, currentHealth / MaxHealth);
        }

        private float wanderTimer = 0f;

        private void Update()
        {
            //Think about what to do
            //state transitions? actions
            var playerArray = GameObject.FindGameObjectsWithTag("Player");
            if(playerArray == null || playerArray.Length == 0)
            {
                return;
            }
            var players = playerArray.ToList();
            players.OrderBy(p => Vector3.Distance(this.transform.position, p.transform.position));
            var closestPlayer = players.First();
            var distToClosest = Vector3.Distance(this.transform.position, closestPlayer.transform.position);

            switch(state)
            {
                case DroneState.Wander:
                    if(distToClosest <= stats.ChaseThreshold)
                    {
                        state = DroneState.Chase;
                    }
                    else
                    {
                        wanderTimer += Time.deltaTime;
                        if(wanderTimer > stats.WanderTime)
                        {
                            wanderTimer = 0;
                            var newDirection = UnityEngine.Random.insideUnitCircle;
                            xMove = newDirection.x;
                            yMove = newDirection.y;
                        }
                    }
                    break;
                case DroneState.Chase:
                    if(distToClosest >= stats.GiveUpDistanceThreshold)
                    {
                        state = DroneState.Wander;
                        break;
                    }
                    var dirToPlayer = (closestPlayer.transform.position - this.transform.position).normalized;
                    xMove = dirToPlayer.x;
                    yMove = dirToPlayer.y;
                

                    break;
            }

            //actually move
            xMove = Mathf.Clamp(xMove, -1, 1);
            yMove = Mathf.Clamp(yMove, -1, 1);
            Debug.Log(xMove + " " + yMove);
            rb.AddForce(new Vector2(xMove, yMove) * stats.Speed * Time.deltaTime, ForceMode2D.Force);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, stats.ChaseThreshold);
        }
    }
}
