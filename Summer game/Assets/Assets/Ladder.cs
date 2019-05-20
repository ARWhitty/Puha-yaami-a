using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    private bool canClimb;
    private GameObject player;
    public float climbSpeed;

	// Use this for initialization
	void Start ()
    {
        canClimb = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(canClimb)
        {
            if (Input.GetKey(KeyCode.W))
            {
                player.transform.Translate(new Vector3(0, 1, 0) * Time.deltaTime * climbSpeed);
            }
            if(Input.GetKey(KeyCode.S))
            {
                player.transform.Translate(new Vector3(0, -1, 0) * Time.deltaTime * climbSpeed);
            }
        }
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            player = collision.gameObject;
            canClimb = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canClimb = false;
        player = null;
    }
}
