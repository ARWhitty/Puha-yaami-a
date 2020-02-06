using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoomTrigger : MonoBehaviour
{
    GameObject player;
    [Range(0, 100)]
    public int zoomAmount;
    private bool triggered = false;
    Camera cam;
    Vector3 newPos;
    [Tooltip("smaller number = slower zoom")]
    public float lerpSpeed = 0.01f;

    private void Start()
    {
        player = FindObjectOfType<Player>().gameObject;
        cam = player.GetComponentInChildren<Camera>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(triggered != true)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                triggered = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (triggered == true)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                triggered = false;
            }
        }
    }

    private void FixedUpdate()
    {   
        if(triggered)
        {
            ZoomOut();
        }
        else
        {
            ZoomIn();
        }
    }

    private void ZoomOut()
    {
        newPos = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y, -50 - zoomAmount);
        cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, newPos, 0.01f);
    }

    private void ZoomIn()
    {
        newPos = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y, -50);
        cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, newPos, 0.01f);
    }
}
