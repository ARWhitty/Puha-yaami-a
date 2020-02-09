using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashTimer : MonoBehaviour
{
    public Player player;
    public Image timerImg;
    private float maxTime;
    private float timeRemaining;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<Player>().GetComponent<Player>();
        timerImg = GetComponent<Image>();
        if (player.GetDashUnlocked())
        {
            timerImg.enabled = true;
        }
        maxTime = player.dashCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        if(player.startDashCd == false)
        {
            timerImg.fillAmount = 1;
        }
        else
        {
            float timeRemaining = maxTime - player.GetDashCD();
            float ratio = timeRemaining / maxTime;
            timerImg.fillAmount = ratio;
        }
    }

    private void FixedUpdate()
    {
        if (timerImg.enabled == false && player.GetDashUnlocked())
        {
            timerImg.enabled = true;
        }
    }
}
