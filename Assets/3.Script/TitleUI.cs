using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    public Button StartButton;
    public GameObject readyTurn;
    public Button ExitButton;
    public Button ContinueButton;
    public Button LogButton;
    
    public GameObject LogPanel;

    public bool isLogPanelOn = false;
    
    // Start is called before the first frame update
    void Start()
    {
        StartButton.onClick.AddListener(GameStart);
        ExitButton.onClick.AddListener(ExitGame);
        ContinueButton.onClick.AddListener(GameContinue);
        LogButton.onClick.AddListener(OnOffLogPanel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GameStart()
    {
        GameManager.Instance.isReadyTurn = true;
        readyTurn.SetActive(true);
        gameObject.SetActive(false);
    }

    private void GameContinue()
    {
        GameManager.Instance.isReadyTurn = true;
        GameManager.Instance.isContinue = true;
        GameManager.Instance.pesonalIDNum = LoadManager.Instance.loadData.battleSave.savePersonalIdNum;
        readyTurn.SetActive(true);
        gameObject.SetActive(false);
    }

    private void ExitGame()
    {
        Application.Quit();
    }

    private void OnOffLogPanel()
    {
        if (isLogPanelOn)
        {
            LogPanel.SetActive(false);
            isLogPanelOn = false;
        }
        else
        {
            LogPanel.SetActive(true);
            isLogPanelOn = true;
        }
    }
}
