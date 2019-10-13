using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JournalAlbumManager : MonoBehaviour
{
    public List<JournalPage> pages;
    public int currPage = 0;

    private JournalUIEntry[] allEntries;

    private string undiscovered = "???";

    struct JournalUIEntry
    {
        public string name;
        public string description;
        public Sprite image;

        public JournalUIEntry(string _name, string _desc, Sprite _img)
        {
            name = _name;
            description = _desc;
            image = _img;
        }
    }

    private void OnEnable()
    {
        UpdatePlantPageUI();
    }

    void Start()
    {
        //TODO: turn me back on when save system is ready
        //LoadData();
        UpdatePlantPageUI();
        FillUIArray();
    }

    private void FillUIArray()
    {
        allEntries = new JournalUIEntry[pages.Count * 3];
        int entryIdx = 0;
        foreach(JournalPage page in pages)
        {
            allEntries[entryIdx] = new JournalUIEntry(page.top.name, page.top.text, page.top.lockedImage);
            allEntries[entryIdx + 1] = new JournalUIEntry(page.mid.name, page.mid.text, page.mid.lockedImage);
            allEntries[entryIdx + 2] = new JournalUIEntry(page.bottom.name, page.bottom.text, page.bottom.lockedImage);

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
        
        //JournalPlantEntry top = pages[currPage].top;
        //JournalPlantEntry mid = pages[currPage].mid;
        //JournalPlantEntry bottom = pages[currPage].bottom;

        //if (!top.unlocked)
        //{
        //    image_top.sprite = top.lockedImage;
        //    name_top.text = undiscovered;
        //    desc_top.text = undiscovered;
        //}
        //else
        //{
        //    image_top.sprite = top.unlockedImage;
        //    name_top.text = top.name;
        //    desc_top.text = top.text;
        //}

        //if (!mid.unlocked)
        //{
        //    image_mid.sprite = mid.lockedImage;
        //    name_mid.text = undiscovered;
        //    desc_mid.text = undiscovered;
        //}
        //else
        //{
        //    image_mid.sprite = mid.unlockedImage;
        //    name_mid.text = mid.name;
        //    desc_mid.text = mid.text;
        //}

        //if (!bottom.unlocked)
        //{
        //    image_bot.sprite = bottom.lockedImage;
        //    name_bot.text = undiscovered;
        //    desc_bot.text = undiscovered;
        //}
        //else
        //{
        //    image_bot.sprite = bottom.unlockedImage;
        //    name_bot.text = bottom.name;
        //    desc_bot.text = bottom.text;
        //}
    }


}
