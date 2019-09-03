using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JournalManager : MonoBehaviour
{
    public List<JournalPage> pages;
    public int currPage = 0;

    public Text name_top;
    public Text name_mid;
    public Text name_bot;

    public Text desc_top;
    public Text desc_mid;
    public Text desc_bot;

    public Image image_top;
    public Image image_mid;
    public Image image_bot;

    private string undiscovered = "???";

    private void OnEnable()
    {
        UpdatePlantPageUI();
    }

    void Start()
    {
        //TODO: turn me back on when save system is ready
        //LoadData();
        UpdatePlantPageUI();
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
        JournalPlantEntry top = pages[currPage].top;
        JournalPlantEntry mid = pages[currPage].mid;
        JournalPlantEntry bottom = pages[currPage].bottom;

        if (!top.unlocked)
        {
            image_top.sprite = top.lockedImage;
            name_top.text = undiscovered;
            desc_top.text = undiscovered;
        }
        else
        {
            image_top.sprite = top.unlockedImage;
            name_top.text = top.name;
            desc_top.text = top.text;
        }

        if (!mid.unlocked)
        {
            image_mid.sprite = mid.lockedImage;
            name_mid.text = undiscovered;
            desc_mid.text = undiscovered;
        }
        else
        {
            image_mid.sprite = mid.unlockedImage;
            name_mid.text = mid.name;
            desc_mid.text = mid.text;
        }

        if (!bottom.unlocked)
        {
            image_bot.sprite = bottom.lockedImage;
            name_bot.text = undiscovered;
            desc_bot.text = undiscovered;
        }
        else
        {
            image_bot.sprite = bottom.unlockedImage;
            name_bot.text = bottom.name;
            desc_bot.text = bottom.text;
        }
    }


}
