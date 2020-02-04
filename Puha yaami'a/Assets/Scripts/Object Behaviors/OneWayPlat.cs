using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlat : MonoBehaviour
{
    public BoxCollider2D platform;
    public bool oneWay = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        platform.enabled = !oneWay;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        oneWay = true;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        oneWay = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        oneWay = false;
    }
}