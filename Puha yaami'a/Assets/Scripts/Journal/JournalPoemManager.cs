using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JournalPoemManager : MonoBehaviour
{
    public List<PoemPage> pages;
    public Text pageNum;
    public List<Text> textAreas;
    private string placeholderUndiscovered = "...........................";
    public int currPage = 0;
    private void OnEnable()
    {
        UpdatePoemPage();
    }

    private void Start()
    {
        //TODO: TURN BACK ON WHEN SAVE SYSTEM FUNCTIONING
        //LoadData();
        UpdatePoemPage();
    }

    public void NextPage()
    {
        if (currPage < pages.Count - 1)
        {
            currPage += 1;
            UpdatePoemPage();
        }
    }

    public void PreviousPage()
    {
        if (currPage > 0)
        {
            currPage -= 1;
            UpdatePoemPage();
        }
    }

    private void UpdatePoemPage()
    {
        //stylistically better to use loops here, but the max entires per page is 3 so
        SetAllSlotsActive();
        CompletedPoem poemTop = pages[currPage].top;
        CompletedPoem poemMid = pages[currPage].mid;
        CompletedPoem poemBot = pages[currPage].bot;

        if(poemTop.poemTitle != "")
        {
            foreach(JournalPoemEntry chunk in poemTop.chunks)
            {
                if(chunk.unlocked)
                {
                    textAreas[0].text += chunk.text;
                }
                else
                {
                    textAreas[0].text += placeholderUndiscovered;
                }
            }
        }
        else
        {
            textAreas[0].gameObject.SetActive(false);
        }

        if (poemMid.poemTitle != "")
        {
            foreach (JournalPoemEntry chunk in poemMid.chunks)
            {
                if (chunk.unlocked)
                {
                    textAreas[1].text += chunk.text;
                }
                else
                {
                    textAreas[1].text += placeholderUndiscovered;
                }
            }
        }
        else
        {
            textAreas[1].gameObject.SetActive(false);
        }

        if (poemBot.poemTitle != "")
        {
            foreach (JournalPoemEntry chunk in poemBot.chunks)
            {
                if (chunk.unlocked)
                {
                    textAreas[2].text += chunk.text;
                }
                else
                {
                    textAreas[2].text += placeholderUndiscovered;
                }
            }
        }
        else
        {
            textAreas[2].gameObject.SetActive(false);
        }

        pageNum.text = (currPage + 1).ToString();
    }

    private void SetAllSlotsActive()
    {
        foreach (Text txt in textAreas)
        {
            txt.gameObject.SetActive(true);
            txt.text = "";
        }
    }
}
