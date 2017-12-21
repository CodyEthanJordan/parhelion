using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TankController : NetworkBehaviour
{

    public float RotationSpeed = 150.0f;
    public float Speed = 3.0f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
        //transform.Rotate(0, 0, -x);
        //transform.Translate(0, z, 0);
    }
}
