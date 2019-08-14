using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Journal Entry", menuName = "Journal Entry/Plant")]
public class JournalPlantEntry : ScriptableObject
{
    public string title;
    public string description;

    public Sprite lockedImage;
    public Sprite unlockedImage;
}
