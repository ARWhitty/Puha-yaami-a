using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalItem : MonoBehaviour
{
    public JournalEntry data;

    private void Start()
    {
        if(data == null)
        {
            Debug.LogError("PLEASE PROVIDE A JOURNAL ENTRY");
        }
        if(data.GetType() == typeof(JournalPlantEntry))
        {
            SetupPlantObject(data);
        }
        else
        {
            SetupPoemObject(data);
        }
    }

    private void SetupPoemObject(JournalEntry toCast)
    {
        JournalPoemEntry entry = (JournalPoemEntry)toCast;
    }

    private void SetupPlantObject(JournalEntry toCast)
    {
        JournalPlantEntry entry = (JournalPlantEntry)toCast;
        this.GetComponent<SpriteRenderer>().sprite = entry.unlockedImage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            data.unlocked = true;
        }
    }
}
