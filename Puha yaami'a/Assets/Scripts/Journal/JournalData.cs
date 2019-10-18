using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class JournalData
{
    public bool[] unlockedData;

    //TODO: Take in poem manager as well
    public JournalData(JournalAlbumManager albumMgr)
    {
        //3 entries per page
        unlockedData = new bool[albumMgr.pages.Count * 3];

        //pull stuff out to a temp list
        List<JournalEntry> allEntries = new List<JournalEntry>();
        foreach(JournalPage page in albumMgr.pages)
        {
            allEntries.Add(page.top);
            allEntries.Add(page.mid);
            allEntries.Add(page.bottom);
        }

        //populate front to back
        for(int i = 0; i < unlockedData.Length; i++)
        {
            unlockedData[i] = allEntries[i].unlocked;
        }
    }
}
