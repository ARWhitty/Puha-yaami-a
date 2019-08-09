using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Journal Entry", menuName = "Journal Entry/Poem")]

public class JournalPoemEntry : ScriptableObject
{
    public string title;

    [TextArea(1, 25)]
    public string text;

    public Sprite image;
}
