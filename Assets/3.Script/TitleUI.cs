using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    public Button StartButton;
    public GameObject readyTurn;
    public Button ExitButton;
    
    // Start is called before the first frame update
    void Start()
    {
        StartButton.onClick.AddListener(GameStart);
        ExitButton.onClick.AddListener(ExitGame);
        
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

    private void ExitGame()
    {
        Application.Quit();
    }
}
