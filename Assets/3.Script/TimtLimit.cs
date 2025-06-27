using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimtLimit : MonoBehaviour
{
    private ReadyTurn readyTurn;
    private TMP_Text timeText;
    public float inputTimeLimitSecond = 30f;
    public float tempTimeLimitSecond;
    private int minute;
    private int second;

    private void Start()
    {
        transform.root.TryGetComponent(out readyTurn);
        transform.TryGetComponent(out timeText);
        tempTimeLimitSecond = inputTimeLimitSecond;
        SecondToMinute();
    }

    private void Update()
    {
        SecondToMinute();
        UpdateTimer();
        FlowTime();
        TimeOver();
    }

    private void UpdateTimer()
    {
        if (minute >= 0 || second >= 0)
        {
            timeText.text = minute > 9 ? $"{minute}:" : $"0{minute}:" +
                            (second > 9 ? $"{second}" : $"0{second}");
        }
    }

    private void FlowTime()
    {
        tempTimeLimitSecond -= Time.deltaTime;
    }
    
    private void SecondToMinute()
    {
        minute = Mathf.FloorToInt(tempTimeLimitSecond / 60);
        second = Mathf.FloorToInt(tempTimeLimitSecond % 60);
    }

    private void TimeOver()
    {
        if(tempTimeLimitSecond < 0)
        {
            readyTurn.CombatStart();
            tempTimeLimitSecond = inputTimeLimitSecond; // Reset timer
            Debug.Log("Time Over! Turn Ended.");
        }
    }
}
