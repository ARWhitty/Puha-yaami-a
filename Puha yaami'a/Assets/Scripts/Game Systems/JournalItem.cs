using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalItem : MonoBehaviour
{
    public JournalPlantEntry plantData;
    public JournalPoemEntry poemData;

    private void Start()
    {
        if(plantData == null)
        {

        }
        else if(poemData == null)
        {
            SetupPlantObject();
        }
        else
        {
            Debug.LogError("Please provide either a plant or a poem!");
        }
    }

    private void SetupPoemObject()
    {

    }

    private void SetupPlantObject()
    {
        this.GetComponent<SpriteRenderer>().sprite = plantData.unlockedImage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            plantData.unlocked = true;
        }
    }
}
