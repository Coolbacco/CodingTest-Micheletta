using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DelayedStart : MonoBehaviour
{
    private float delay;
    private float currentTime = 0;
    private float pauseTime;
    private AudioSource startSE;

    public Text countdownText;

    private bool timeOut = false;

    private void Awake()
    {
        TextAsset file = Resources.Load("GameManager") as TextAsset;
        string json = file.ToString();
        DelayData loadedDelayData = JsonUtility.FromJson<DelayData>(json);
        delay = loadedDelayData.delayToStartInSeconds;
    }

    void Start()
    {
        startSE = GetComponent<AudioSource>();
        StartCoroutine("StartDelay");
    }

    //updates the text with the time until start
    //when the countdown finishes, the text is deactivated
    void Update()
    {
        if (timeOut)
            return;

        currentTime = pauseTime - Time.realtimeSinceStartup;
        countdownText.text = currentTime.ToString("0");

        if (currentTime <= 0)
        {
            countdownText.gameObject.SetActive(false);
            timeOut = true;
        }
    }

    //stops the time and restarts it after "delay" seconds
    IEnumerator StartDelay()
    {
        Time.timeScale = 0;
        pauseTime = Time.realtimeSinceStartup + delay;
        
        while (Time.realtimeSinceStartup < pauseTime)
        {
            yield return 0;
        }

        startSE.Play(0);
        Time.timeScale = 1;
    }

    private class DelayData
    {
        public float delayToStartInSeconds;
    }
}
