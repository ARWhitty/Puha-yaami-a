using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public Dialogue dialogue;

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("trigger");
        if(collision.gameObject.tag == "Player")
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Dialogue trigger");
                TriggerDialogue();
            }
        }
    }
}
