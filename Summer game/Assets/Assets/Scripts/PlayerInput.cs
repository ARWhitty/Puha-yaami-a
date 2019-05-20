using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{ 
    /// <summary>
    /// Master input manager 
    /// </summary>

    public PlayerMovementManager movementManager;

	// Use this for initialization
	void Start ()
    {

    }
	
	// Update is called once per frame
	void Update ()
    {
        //Checks if specific action buttons are in use and calls appropriate methods
        if(Input.GetKeyDown(KeyCode.Space))
        {
            movementManager.Jump();
        }
        if(Input.GetAxis("Dash") > 0)
        {
            movementManager.Dash(Input.GetAxis("Horizontal"));
        }
    }
}
