using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashTimer : MonoBehaviour
{
    public Player player;
    public Text timerText;
    public Image timerBar;
    private float maxTime;
    private float timeRemaining;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<Player>().GetComponent<Player>();
        timerBar = GetComponent<Image>();
        if (player.GetDashUnlocked())
        {
            timerBar.enabled = true;
            timerText.enabled = true;
        }
        maxTime = player.dashCooldown;
        timeRemaining = maxTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(timeRemaining > 0)
        {
            if(player.startDashCd)
            {
                timeRemaining -= Time.deltaTime;
                timerBar.fillAmount = timeRemaining / maxTime;
            }
        }
        else
        {
            timeRemaining = maxTime;
            timerBar.fillAmount = 1;
        }
    }
}
