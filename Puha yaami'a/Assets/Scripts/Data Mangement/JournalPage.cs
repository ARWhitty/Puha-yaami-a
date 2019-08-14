using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Journal Page", menuName = "Journal Page")]
public class JournalPage : ScriptableObject
{
    public JournalPlantEntry top;
    public JournalPlantEntry mid;
    public JournalPlantEntry bottom;
}
