using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int startCountDown = 3;
    public bool isFightStart = false;
    
    private void Awake()
    {
        Instance = this;
    }
    
    private void Start()
    {
        StartCoroutine(CountDown());
    }

    private IEnumerator CountDown()
    {
        WaitForSeconds wfs = new WaitForSeconds(1f);

        while (startCountDown >= 0)
        {
            yield return wfs;
            Debug.Log(startCountDown);
            startCountDown--;
        }
        
        isFightStart = true;
    }
}
