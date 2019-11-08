using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class JournalCollectible : MonoBehaviour
{
    public JournalEntry data;
    public GameObject particles;

    private void OnValidate()
    {
        if(data != null)
        {
            if (data.GetType() == typeof(JournalPlantEntry))
            {
                SetupPlantObject(data);
            }
            else
            {
                SetupPoemObject(data);
            }
        }
    }

    //TODO: REMEMBER TO REMOVE ME!!!!!!!!!!!!!!!!
    private void OnApplicationQuit()
    {
        data.unlocked = false;
    }

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
            if(!data.unlocked)
                Unlock();
        }
    }

    private void Unlock()
    {
        Instantiate(particles, transform.position, Quaternion.identity);
        data.unlocked = true;
        //dim the image for the collectible a touch
        this.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.7f);
    }
}
