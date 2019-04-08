using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public GameObject FirstPanel;
    public GameObject GameModePanel;
    public GameObject SurvivalDifficultyPanel;
    public Text CommingSoon;
    private DifficultyManager DifficultyManager;
	// Use this for initialization
	void Start () {
        DifficultyManager = FindObjectOfType<DifficultyManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnClickStart()
    {
        FirstPanel.SetActive(false);
        GameModePanel.SetActive(true);
    }

    public void OnClickQuit()
    {
        Application.Quit();
    }

    public void OnClickSurvival()
    {
        GameModePanel.SetActive(false);
        SurvivalDifficultyPanel.SetActive(true);
    }

    public void OnClickTeamFight()
    {
        CommingSoon.gameObject.SetActive(true);
        StartCoroutine(ShowStartingSoon());
        DifficultyManager.SurvivalDifficulty = 1;
        SceneManager.LoadScene("TeamFight");
    }

    IEnumerator ShowStartingSoon()
    {
        yield return new WaitForSeconds(3);
        CommingSoon.gameObject.SetActive(false);
    }
    public void OnClickBackToFirstPanelBtn()
    {
        GameModePanel.SetActive(false);
        FirstPanel.SetActive(true);
    }

    public void OnClickSurvialEasy()
    {
        DifficultyManager.SurvivalDifficulty = 1;
        SceneManager.LoadScene("Survival");
    }

    public void OnClickSurvivalMedium()
    {
        DifficultyManager.SurvivalDifficulty = 2;
        SceneManager.LoadScene("Survival");
    }

    public void OnClickSurvivalHard()
    {
        DifficultyManager.SurvivalDifficulty = 3;
        SceneManager.LoadScene("Survival");
    }

    public void OnClickBackToGameModePanelBtn()
    {
        SurvivalDifficultyPanel.SetActive(false);
        GameModePanel.SetActive(true);
    }
}
