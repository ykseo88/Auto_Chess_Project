using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CombatTurn : MonoBehaviour
{

    public Image countDownImage;
    public Image resultImage;

    public Sprite ThreeSprite;
    public Sprite TwoSprite;
    public Sprite OneSprite;
    public Sprite startSprite;
    
    public RoundManager roundManager;
    
    public Sprite victorySprite;
    public Sprite defeatSprite;

    public Button backToTitle;
    
    public TMPro.TMP_Text RoundScore;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        backToTitle.onClick.AddListener(BackToTitle);
        roundManager = GameObject.Find("RoundManager").GetComponent<RoundManager>();
    }

    private void BackToTitle()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCountDown();
        UpdateRoundScore();
    }

    private void OnEnable()
    {
        countDownImage.gameObject.SetActive(true);
    }

    private void UpdateCountDown()
    {
        switch (GameManager.Instance.startCountDown)
        {
            case 3:
                countDownImage.sprite = ThreeSprite;
                break;
            case 2:
                countDownImage.sprite = TwoSprite;
                break;
            case 1:
                countDownImage.sprite = OneSprite;
                break;
            case 0:
                countDownImage.sprite = startSprite;
                break;
        }
    }

    private void UpdateRoundScore()
    {
        RoundScore.text = "Round Score: " + roundManager.currentRoundIndex.ToString();
    }
}
