using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject playerObj;
    private Player player;
    private GameObject lastCheckpoint;
    private Vector2 playerStartPos;
    [SerializeField] private int score;
    public Text scoreText;

    [SerializeField] private float bouncyModifier = 1.5f;
    [SerializeField] private float stickyModifier = 0.5f;
    [SerializeField] private List<GameObject> checkpoints;

    void OnEnable()
    {
        Player.OnCollide += HandlePlatformCollide;
        Player.OnTrigger += HandleTriggerCollision;
    }

    void OnDisable()
    {
        Player.OnCollide -= HandlePlatformCollide;
        Player.OnTrigger -= HandleTriggerCollision;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = playerObj.GetComponent<Player>();
        UpdateScoreText();
        playerStartPos = player.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void HandlePlatformCollide(int type)
    {
        switch(type)
        {
            //normal
            case 0: 
                break;
            //bouncy
            case 1:
                player.SetJumpForce(player.GetDefaultJumpForce() * bouncyModifier);
                break;
            //sticky
            case 2:
                player.SetJumpForce(player.GetDefaultJumpForce() * stickyModifier);
                break;
            //fail
            case 3:
                OnFail();
                break;
            //score loss
            case 4:
                ScoreLoss();
                break;
        }
    }

    void HandleTriggerCollision(string type, GameObject obj)
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
                player.UnlockAbility(0);
                break;
            case "Dash_Unlock":
                player.UnlockAbility(1);
                break;
            case "Glide_Unlock":
                player.UnlockAbility(2);
                break;
        }
    }

    void OnFail()
    {
        if (lastCheckpoint != null)
            playerObj.transform.position = lastCheckpoint.transform.position;
        else
            playerObj.transform.position = playerStartPos;
    }

    void ScoreLoss()
    {
        score += 500;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }
}
