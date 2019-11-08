using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lockout : MonoBehaviour
{
    public float lockoutTime = 2f;
    private float timerInternal;
    public bool lockedOut;
    private SpriteRenderer renderer;
    private bool particlesEmitted;
    public GameObject pSys;
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
            if (!particlesEmitted)
            {
                Instantiate(pSys, transform.position, Quaternion.identity);
                particlesEmitted = true;
            }
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
            particlesEmitted = false;
            Color temp = renderer.color;
            if (temp.a != 1f)
            {
                temp.a = 1f;
                renderer.color = temp;
            }
        }
    }
}
