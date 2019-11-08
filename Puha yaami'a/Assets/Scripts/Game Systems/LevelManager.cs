using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public string nextLevelName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
        {
            if (nextLevelName != null)
            {
                //StartCoroutine(LoadNextLevel(col.gameObject));
                SceneManager.LoadScene(nextLevelName);
            }
            //note to self: can provide "additive" property to add next level to stack of loaded stuff, good for main map?
            else
                Debug.LogError("PLEASE PROVIDE THE NAME OF THE SCCENE TO LOAD IN INSPECTOR");
        }
    }

    IEnumerator LoadNextLevel(GameObject player)
    {
        player.transform.position = Vector3.Lerp(player.transform.position, player.transform.position + new Vector3(15, 0f, 0f), 0.5f);
        yield return new WaitForSeconds(0.8f);
        //SceneManager.LoadScene(nextLevelName);
    }
}
