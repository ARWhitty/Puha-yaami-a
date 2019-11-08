using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JournalEntry : ScriptableObject
{
    public string title;

    [TextArea(1, 25)]
    public string text;

    public bool unlocked;
}
