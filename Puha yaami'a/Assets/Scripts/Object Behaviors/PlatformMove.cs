﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMove : MonoBehaviour
{
    private Vector2 initialPos;
    [SerializeField]private Vector2 endPos;
    private Vector2 currentPos;

    //-1 is left/down, 1 is right/up, 0 is not moving in that direction
    [SerializeField]private int dirHoriz;
    [SerializeField]private int dirVert;

    [SerializeField] private float moveSpeed;
    private Vector3 moveVec;

    private void OnEnable()
    {
        GameManager.StandardLevelFail += ResetToStartLocation;
    }

    private void OnDisable()
    {
        GameManager.StandardLevelFail -= ResetToStartLocation;
    }

    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.position;
        currentPos = initialPos;
        moveVec = new Vector3(moveSpeed, 0f, 0f);

        if(this.transform.parent != null && this.transform.parent.transform.position != Vector3.zero)
        {
            Debug.LogError("PLEASE RESET ORGANIZER GAMEOBJECT TRANSFORM TO 0");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(PauseMenu.gamePaused)
        {
            return;
        }
        //get where we currently are
        currentPos = transform.position;

        //check horizontal movement
        switch(dirHoriz)
        {
            case 0:
                break;
            case -1:
                //if it starts on the right and moves left
                if(initialPos.x > endPos.x)
                {
                    //check if where it currently is is less than the end x
                    if(currentPos.x <= endPos.x)
                    {
                        //if so, swap it to moving right
                        dirHoriz = 1;
                    }
                    //if it isn't, move it left
                    else
                    {
                        transform.position -= moveVec;
                    }
                }
                //otherwise it starts left and moves right
                else
                {
                    //check if it's moved back to the initialpos
                    if(currentPos.x <= initialPos.x)
                    {
                        //if so, swap it to moving right
                        dirHoriz = 1;
                    }
                    else
                    {
                        transform.position -= moveVec;
                    }
                }
                break;
            case 1:
                //if it starts left and moves right
                if(initialPos.x < endPos.x)
                {
                    //check if where it currently is is greater than the end
                    if(currentPos.x >= endPos.x)
                    {
                        //if so, swap it to moving left
                        dirHoriz = -1;
                    }
                    //if it isn't, move it right
                    else
                    {
                        transform.position += moveVec;
                    }
                }
                //otherwise it starts right and moves left
                else
                {
                    //check if it moved back to start
                    if(currentPos.x >= initialPos.x)
                    {
                        //if so, swap
                        dirHoriz = -1;
                    }
                    //otherwise move right
                    else
                    {
                        transform.position += moveVec;
                    }
                }
                break;
        }

        //TODO: VERTICAL PLATFORMS
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        //TODO: chck if it's a player specifically in case we have overlapping platforms
        col.transform.SetParent(this.transform);
    }

    void OnCollisionExit2D(Collision2D col)
    {
        //translate back to worldspace from loclaspace
        Vector3 leavePos = col.transform.position;
        col.transform.SetParent(null);
        col.transform.position = leavePos;
    }

    void ResetToStartLocation()
    {
        this.transform.position = initialPos;
    }
}
