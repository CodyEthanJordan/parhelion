using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankAgent : Agent
{

    private GameObject destination;

    public override void InitializeAgent()
    {
        base.InitializeAgent();
        destination = GameObject.FindGameObjectWithTag("Finish");
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
        if (act.Length > 0)
        {
            Debug.Log(act.Length + " " + act[0]);
        }
    }

    public override void AgentReset()
    {
        transform.position = Vector3.zero;
    }

    public override void AgentOnDone()
    {

    }
}
