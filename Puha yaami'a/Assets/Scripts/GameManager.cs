using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject playerObj;
    private Player player;
    [SerializeField] private Vector3 lastCheckpoint;
    [SerializeField]private Vector3 playerStartPos;
    [SerializeField] private int score = 0;
    public Text scoreText;

    [SerializeField] private float bouncyModifier = 1.5f;
    [SerializeField] private float stickyModifier = 0.5f;
    //[SerializeField] private List<Vector3> checkpoints;

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
        lastCheckpoint = playerStartPos;
        //checkpoints.Add(playerStartPos);

        LoadData();
    }

    private void LoadData()
    {
        //Load data
        PlayerData pData = SaveSystem.LoadPlayer();

        //Fill in transform
        Vector3 playerLoadedPos;
        playerLoadedPos.x = pData.pos[0];
        playerLoadedPos.y = pData.pos[1];
        playerLoadedPos.z = pData.pos[2];

        //Move player to loaded transform
        player.transform.position = playerLoadedPos;


        //Load gm data
        GameManagerData gmData = SaveSystem.LoadGM();

        //Update score
        score = gmData.score;
        UpdateScoreText();

        //Load last checkpoint and update
        Vector3 loadedLastCheckpoint;
        loadedLastCheckpoint.x = gmData.lastCheckpoint[0];
        loadedLastCheckpoint.y = gmData.lastCheckpoint[1];
        loadedLastCheckpoint.z = gmData.lastCheckpoint[2];

        //Load all stored checkpoints and update
        /*        List<Vector3> loadedCheckpoints = new List<Vector3>();
                foreach(Vec3 pos in gmData.checkpointPositions)
                {
                    Vector3 loadedChkpt;
                    loadedChkpt.x = pos.x;
                    loadedChkpt.y = pos.y;
                    loadedChkpt.z = pos.z;

                    loadedCheckpoints.Add(loadedChkpt);
                }*/

        //checkpoints = loadedCheckpoints;
        lastCheckpoint = loadedLastCheckpoint;
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

    void HandleTriggerCollision(string type, Collider2D checkpointCollider)
    {
        switch(type)
        {
            case "Checkpoint":
                /*                if(!checkpoints.Contains(checkpointCollider.transform.position))
                                {*/
                if (lastCheckpoint != checkpointCollider.transform.position)
                {
                    lastCheckpoint = checkpointCollider.transform.position;
                }
                    //checkpoints.Add(checkpointCollider.transform.position);
                //}
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
        {
            playerObj.transform.position = lastCheckpoint;
            player.ResetCooldowns();
        }

        else
        {
            Debug.LogError("NO CHECKPOINTS FOUND");
        }

    }

    void ScoreLoss()
    {
        score -= 500;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }

    public int GetScore()
    {
        return score;
    }

/*    public List<Vector3> GetCheckpoints()
    {
        return checkpoints;
    }*/

    public Vector3 GetLastCheckpoint()
    {
        return lastCheckpoint;
    }
}
