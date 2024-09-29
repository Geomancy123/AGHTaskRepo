using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //globally accessable game manager instance
    public static GameManager instance;

    public bool bIsPaused = false;


    [SerializeField] GameObject HUDobj;
    


    private void Awake()
    {
        //Singleton
        if (instance == null )
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1.0f;
        bIsPaused = false;

        if(FindFirstObjectByType<HUD>())
        {
            HUDobj = FindFirstObjectByType<HUD>().gameObject;
        }
      
    }


    //Pause the game, setting timescale to 0, and fading sound effects out
    public void PauseGame()
    {
        //returns if game is already paused
        if (bIsPaused) { return; }
    
        //pauses the game
        bIsPaused = true;
        Time.timeScale = 0;

        //If HUD exists, toggle the pause overlay/button
        if (HUDobj)
        {
            HUDobj.GetComponent<HUD>().TogglePausePanel(true);
        }
    }

    //Play the game, setting timescale to 1, and fading sound effects in
    public void PlayGame()
    {
        //returns if game is already paused
        if (!bIsPaused) { return; }

        //pauses the game
        bIsPaused = false;
        Time.timeScale = 1;

        //If HUD exists, toggle the pause overlay/button
        if (HUDobj)
        {
            HUDobj.GetComponent<HUD>().TogglePausePanel(false);
        }
    }
}
