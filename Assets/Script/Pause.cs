using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public GameObject pausePanel;
    void Start()
    {
        SetPaused(false);
    }

    private bool paused = false;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SetPaused(!paused);
        }
    }

    public void SetPaused(bool newPauseState)
    {
        paused = newPauseState;


        pausePanel.SetActive(paused);
        Time.timeScale = paused ? 0 : 1;
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit!");
    }
}
