using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour {

    public float RotationSpeed = 150.0f;
    public float Speed = 3.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * RotationSpeed;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * Speed;

        transform.Rotate(0, 0, -x);
        transform.Translate(0, z, 0);
    }
}
