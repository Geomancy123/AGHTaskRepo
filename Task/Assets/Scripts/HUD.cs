using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{

    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject pauseButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //toggles the visibility of the paused overlay, and the pause button
    public void TogglePausePanel(bool _bVisible)
    {
        if (_bVisible)
        {
            pausePanel.SetActive(true);
            pauseButton.SetActive(false);
        }
        else
        {
            pausePanel.SetActive(false);
            pauseButton.SetActive(true);
        }
    }
}
