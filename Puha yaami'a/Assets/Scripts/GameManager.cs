using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject playerObj;
    private Player player;
    private GameObject lastCheckpoint;
    [SerializeField] private int score;
    public Text scoreText;

    [SerializeField] private float bouncyModifier = 1.5f;
    [SerializeField] private float stickyModifier = 0.5f;

    [SerializeField] private List<GameObject> checkpoints;

    void OnEnable()
    {
        Player.OnCollide += handlePlatformCollide;
        Player.OnTrigger += handleTriggerCollision;
    }

    void OnDisable()
    {
        Player.OnCollide -= handlePlatformCollide;
        Player.OnTrigger -= handleTriggerCollision;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = playerObj.GetComponent<Player>();
        updateScoreText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void handlePlatformCollide(int type)
    {
        switch(type)
        {
            //normal
            case 0: 
                break;
            //bouncy
            case 1:
                player.setJumpForce(player.getDefaultJumpForce() * bouncyModifier);
                break;
            //sticky
            case 2:
                player.setJumpForce(player.getDefaultJumpForce() * stickyModifier);
                break;
            //fail
            case 3:
                onFail();
                break;
            //score loss
            case 4:
                scoreLoss();
                break;
        }
    }

    void handleTriggerCollision(string type, GameObject obj)
    {
        switch(type)
        {
            case "Checkpoint":
                if(!checkpoints.Contains(obj))
                {
                    lastCheckpoint = obj;
                    checkpoints.Add(obj);
                }
                break;
            case "Double_Jump_Unlock":
                player.unlockAbility(0);
                break;
            case "Dash_Unlock":
                player.unlockAbility(1);
                break;
            case "Glide_Unlock":
                player.unlockAbility(2);
                break;
        }
    }

    void onFail()
    {
        playerObj.transform.position = lastCheckpoint.transform.position;
    }

    void scoreLoss()
    {
        score += 500;
        updateScoreText();
    }

    void updateScoreText()
    {
        scoreText.text = "Score: " + score;
    }
}
