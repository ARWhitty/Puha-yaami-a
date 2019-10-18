using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JournalAlbumManager : MonoBehaviour
{
    public List<JournalPage> pages;
    public int currPage = 0;
    public List<Image> imgSlots;
    public List<Text> nameSlots;
    public List<Text> descSlots;

    private JournalUIEntry[] allEntries = new JournalUIEntry[0];

    public Sprite lockedSprite;
    private string undiscovered = "???";

    struct JournalUIEntry
    {
        public string name;
        public string description;
        public Sprite image;
        public bool unlocked;

        public JournalUIEntry(string _name, string _desc, Sprite _img, bool _unlocked)
        {
            name = _name;
            description = _desc;
            image = _img;
            unlocked = _unlocked;
        }
    }

    private void OnEnable()
    {
        if (allEntries.Length == 0)
            FillUIArray();
        UpdatePlantPageUI();
    }

    void Start()
    {
        //TODO: turn me back on when save system is ready
        //LoadData();
        UpdatePlantPageUI();
    }

    private void FillUIArray()
    {
        //NOTE: may need to fix the foreach loop a bit if entries per page is not 3
        allEntries = new JournalUIEntry[pages.Count * 3];
        int entryIdx = 0;
        foreach(JournalPage page in pages)
        {
            allEntries[entryIdx] = new JournalUIEntry(page.top.title, page.top.text, page.top.unlockedImage, page.top.unlocked);
            allEntries[entryIdx + 1] = new JournalUIEntry(page.mid.title, page.mid.text, page.mid.unlockedImage, page.mid.unlocked);
            allEntries[entryIdx + 2] = new JournalUIEntry(page.bottom.title, page.bottom.text, page.bottom.unlockedImage, page.bottom.unlocked);

            entryIdx += 3;
        }
    }

    private void LoadData()
    {
        JournalData jData = SaveSystem.LoadJournal();
        //populate unlocks front to back
        int pageCount = 0;
        foreach(JournalPage page in pages)
        {
            //top is first bool, then mid, then bot, multiply by 3 to move to corresponding chunk of array
            page.top.unlocked = jData.unlockedData[pageCount * 3];
            page.mid.unlocked = jData.unlockedData[(pageCount * 3) + 1];
            page.bottom.unlocked = jData.unlockedData[(pageCount * 3) + 2];

            pageCount++;
        }
    }

    public void NextPage()
    {
        if(currPage < pages.Count - 1)
        {
            currPage += 1;
            UpdatePlantPageUI();
        }
    }

    public void PreviousPage()
    {
        if (currPage > 0)
        {
            currPage -= 1;
            UpdatePlantPageUI();
        }
    }

    private void UpdatePlantPageUI()
    {
        for(int i = 0; i < 3; i++)
        {
            JournalUIEntry curr = allEntries[currPage + i];
            if(curr.unlocked)
            {
                imgSlots[i].sprite = curr.image;
                nameSlots[i].text = curr.name;
                descSlots[i].text = curr.description;
            }
            else
            {
                imgSlots[i].sprite = lockedSprite;
                nameSlots[i].text = undiscovered;
                descSlots[i].text = "";
            }
        }

    }


}
