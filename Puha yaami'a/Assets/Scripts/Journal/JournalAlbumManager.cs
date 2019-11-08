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
    public List<GameObject> backgrounds;
    public Text pageNum;
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
        pageNum.text = "1";
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
        SetAllSlotsActive();

        JournalPlantEntry cTop = pages[currPage].top;
        JournalPlantEntry cMid = pages[currPage].mid;
        JournalPlantEntry cBot = pages[currPage].bottom;

        if(cTop != null)
        {
            imgSlots[0].sprite = cTop.unlocked ? cTop.unlockedImage : cTop.lockedImage;
            nameSlots[0].text = cTop.unlocked ? cTop.name : undiscovered;
            descSlots[0].text = cTop.unlocked ? cTop.text : undiscovered;
        }
        else
        {
            imgSlots[0].gameObject.SetActive(false);
            descSlots[0].gameObject.SetActive(false);
            nameSlots[0].gameObject.SetActive(false);
            backgrounds[0].SetActive(false);
        }

        if(cMid != null)
        {
            imgSlots[1].sprite = cMid.unlocked ? cMid.unlockedImage : cMid.lockedImage;
            nameSlots[1].text = cMid.unlocked ? cMid.name : undiscovered;
            descSlots[1].text = cMid.unlocked ? cMid.text : undiscovered;
        }
        else
        {
            imgSlots[1].gameObject.SetActive(false);
            descSlots[1].gameObject.SetActive(false);
            nameSlots[1].gameObject.SetActive(false);
            backgrounds[1].SetActive(false);
        }

        if (cBot != null)
        {
            imgSlots[2].sprite = cBot.unlocked ? cBot.unlockedImage : cBot.lockedImage;
            nameSlots[2].text = cBot.unlocked ? cBot.name : undiscovered;
            descSlots[2].text = cBot.unlocked ? cBot.text : undiscovered;
        }
        else
        {
            imgSlots[2].gameObject.SetActive(false);
            descSlots[2].gameObject.SetActive(false);
            nameSlots[2].gameObject.SetActive(false);
            backgrounds[2].SetActive(false);
        }

        pageNum.text = (currPage+1).ToString();
    }

    private void SetAllSlotsActive()
    {
        foreach(Image img in imgSlots)
        {
            img.gameObject.SetActive(true);
        }
        foreach (GameObject go in backgrounds)
        {
            go.SetActive(true);
        }
        foreach (Text txt in descSlots)
        {
            txt.gameObject.SetActive(true);
        }
        foreach (Text name in nameSlots)
        {
            name.gameObject.SetActive(true);
        }
    }


}
