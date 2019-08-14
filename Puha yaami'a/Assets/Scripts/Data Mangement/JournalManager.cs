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

    private void OnEnable()
    {
        UpdatePageUI();
    }

    void Start()
    {
        UpdatePageUI();
    }

    public void NextPage()
    {
        Debug.Log(pages.Count);
        if(currPage < pages.Count - 1)
        {
            currPage += 1;
            UpdatePageUI();
        }
    }

    public void PreviousPage()
    {
        if (currPage > 0)
        {
            currPage -= 1;
            UpdatePageUI();
        }
    }

    private void UpdatePageUI()
    {
        JournalPlantEntry top = pages[currPage].top;
        JournalPlantEntry mid = pages[currPage].mid;
        JournalPlantEntry bottom = pages[currPage].bottom;

        name_top.text = top.name;
        name_mid.text = mid.name;
        name_bot.text = bottom.name;

        desc_top.text = top.description;
        desc_mid.text = mid.description;
        desc_bot.text = bottom.description;

        if (!top.unlocked)
            image_top.sprite = top.lockedImage;
        else
            image_top.sprite = top.unlockedImage;

        if (!mid.unlocked)
            image_mid.sprite = mid.lockedImage;
        else
            image_mid.sprite = mid.unlockedImage;

        if (!bottom.unlocked)
            image_bot.sprite = bottom.lockedImage;
        else
            image_bot.sprite = bottom.unlockedImage;
    }


}
