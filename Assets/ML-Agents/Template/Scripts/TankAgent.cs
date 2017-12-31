using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankAgent : Agent
{
    public float Speed = 40;
    public float RotationSpeed = 9;

    private GameObject destination;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private GameObject turret;
    private TurretControl turretControl;


    public override void InitializeAgent()
    {
        base.InitializeAgent();
        destination = GameObject.FindGameObjectWithTag("Finish");

        rb = GetComponent<Rigidbody2D>();
        turret = transform.GetChild(0).gameObject; //assume turret is only child
        turretControl = turret.GetComponent<TurretControl>();
        sr = GetComponent<SpriteRenderer>();
    }

    public override List<float> CollectState()
    {
        List<float> state = new List<float>();
        state.Add(transform.position.x);
        state.Add(transform.position.y);
        state.Add(destination.transform.position.x);
        state.Add(destination.transform.position.y);

        return state;
    }

    public override void AgentStep(float[] act)
    {
        var horizontal = act[2] + act[3];
        var vertical = act[0] + act[1];
        var x = horizontal * Time.deltaTime * RotationSpeed;
        var z = vertical * Time.deltaTime * Speed;
        rb.AddRelativeForce(new Vector2(0, z), ForceMode2D.Impulse);
        rb.AddTorque(-x, ForceMode2D.Impulse);

        //close to destination
    }

    public override void AgentReset()
    {
        transform.position = Vector3.zero;
    }

    public override void AgentOnDone()
    {

    }
}
