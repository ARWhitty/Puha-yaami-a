using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionManager : MonoBehaviour
{
    public PlayerMovementManager movementManager;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Wall_Breakable" && movementManager.isDashing)
        {
            Destroy(collision.gameObject);
            movementManager.isDashing = false;
        }
    }
}
