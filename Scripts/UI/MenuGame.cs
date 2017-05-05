using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuGame : MonoBehaviour
{
  
    public Button restart,exit, mainMenu, restartLose,continueButton,mainMenuLose;   
    public GameObject panelLose,panelHelp,panelExit,panelRestart;

    // Use this for initialization
    void Start () {
        if (restart != null)
            restart.onClick.AddListener(RestatGame);
        if (exit != null)
            exit.onClick.AddListener(QuitGame);
        if (restartLose != null)
            restartLose.onClick.AddListener(RestatGame);
        if (mainMenu != null)
            mainMenu.onClick.AddListener(MainMenu);
        if (mainMenuLose != null)
            mainMenuLose.onClick.AddListener(MainMenuLose);
        if (continueButton != null)
        {
            if (LoadManager.Instance.needLoad)
            {
                continueButton.interactable = true;
            }
            else
            {
                continueButton.interactable = false;
            }           
            continueButton.onClick.AddListener(ContinueGame);
        }
    }

    public void OpenRestart()
    {
        SoundManager.Instance.PlayPauseOn();
        panelRestart.SetActive(true);
    }

    public void OpenExit()
    {
        SoundManager.Instance.PlayPauseOn();
        panelExit.SetActive(true);
    }


    public void OpenHelp()
    {
        SoundManager.Instance.PlayQuestion();
        panelHelp.SetActive(true);
    }

    public void PlayPauseOff()
    {
        SoundManager.Instance.PlayPauseOff();
    }

    private void ContinueGame()
    {
        SceneManager.LoadScene("Game");
    }

    private void MainMenuLose()
    {
        LoadManager.Instance.ClearSave();
        SceneManager.LoadScene("MainMenu");
    }

    private void MainMenu() 
    {        
        LoadManager.Instance.SaveGame();
        SceneManager.LoadScene("MainMenu");
    }
	
	private void RestatGame()
    {
        LoadManager.Instance.ClearSave();
        SceneManager.LoadScene("Game");
    }

    private void QuitGame()
    {
        Application.Quit();
    }
}
