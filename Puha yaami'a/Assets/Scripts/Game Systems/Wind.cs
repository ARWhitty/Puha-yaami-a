using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    //0 is up, 1 is down, 2 is left, 3 is right
    [Header("dir Settings: 0 is up, 1 is down, 2 is left, 3 is right")]
    [SerializeField] private int dir;
    [SerializeField] private float strength;

    private Vector2 windForce;
    // Start is called before the first frame update
    void Start()
    {
        switch(dir)
        {
            case 0:
                windForce = new Vector2(0f, strength);
                break;
            case 1:
                windForce = new Vector2(0f, -strength);
                break;
            case 2:
                windForce = new Vector2(-strength, 0f);
                break;
            case 3:
                windForce = new Vector2(strength, 0f);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector2 getForce()
    {
        return windForce;
    }
}
