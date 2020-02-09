using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Poem Page", menuName = "Poem Page")]
public class PoemPage : ScriptableObject
{
    public CompletedPoem top;
    public CompletedPoem mid;
    public CompletedPoem bot;
}

[System.Serializable]
public struct CompletedPoem
{
    public string poemTitle;
    public List<JournalPoemEntry> chunks;
}
