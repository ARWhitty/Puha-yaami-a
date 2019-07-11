using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCrumble : MonoBehaviour
{
    [SerializeField] private float crumbleTime;
    [SerializeField] private float resetTime;

    private float crumbleCountdown;
    private float resetCountdown;

    private Vector3 moveOff;
    private Vector2 initialPos;

    private bool isCrumbling = false;
    private bool isCrumbled = false;

    // Start is called before the first frame update
    void Start()
    {
        initialPos = transform.position;
        crumbleCountdown = crumbleTime;
        resetCountdown = resetTime;
        moveOff = new Vector3(0, 200f);
    }

    // Update is called once per frame
    void Update()
    {
        //if we are crumbling, time it, crumble if necessary
        if(isCrumbling)
        {
            crumbleCountdown -= Time.deltaTime;
            if(crumbleCountdown <= 0)
            {
                Crumble();
            }
        }

        //if we are crumbled, time it, reset if necessary
        if(isCrumbled)
        {
            resetCountdown -= Time.deltaTime;
            if(resetCountdown <= 0)
            {
                ResetLocation();
            }
        }
    }

    void OnCollisionStay2D(Collision2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            Vector2 contactPt = col.contacts[0].point;
            Vector2 center = col.collider.bounds.center;
            if(contactPt.y < center.y)
            {
                isCrumbling = true;
            }
        }
    }

    private void Crumble()
    {
        //send it to egypt
        this.transform.position += moveOff;
        isCrumbling = false;
        isCrumbled = true;
        crumbleCountdown = crumbleTime;
    }

    private void ResetLocation()
    {
        //send it back
        this.transform.position = initialPos;
        isCrumbled = false;
        resetCountdown = resetTime;
    }
}
