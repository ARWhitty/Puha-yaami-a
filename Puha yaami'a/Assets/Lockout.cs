using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lockout : MonoBehaviour
{
    public float lockoutTime = 2f;
    private float timerInternal;
    public bool lockedOut;
    private SpriteRenderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        timerInternal = lockoutTime;
        renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(lockedOut)
        {
            if(timerInternal > 0)
            {
                timerInternal -= Time.deltaTime;
            }
            else
            {
                timerInternal = lockoutTime;
                lockedOut = false;
            }
            Color temp = renderer.color;
            if(temp.a != 0.5f)
            {
                temp.a = 0.5f;
                renderer.color = temp;
            }
        }
        else
        {
            Color temp = renderer.color;
            if (temp.a != 1f)
            {
                temp.a = 1f;
                renderer.color = temp;
            }
        }
    }
}
