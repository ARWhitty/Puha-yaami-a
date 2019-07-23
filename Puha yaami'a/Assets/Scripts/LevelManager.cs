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
                //note to self: can provide "additive" property to add next level to stack of loaded stuff, good for main map?
                SceneManager.LoadScene(nextLevelName);
            else
                Debug.LogError("PLEASE PROVIDE THE NAME OF THE SCCENE TO LOAD IN INSPECTOR");
        }
    }
}
