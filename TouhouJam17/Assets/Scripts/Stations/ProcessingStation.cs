using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ProcessingStation : BaseStation
{

    [SerializeField] private Image progressImage;
    [SerializeField] private Vector2 minMaxProcessingTime = new Vector2(10f, 20f);

    [SerializeField] private StationButton startButton;
    [SerializeField] private StationButton sendButton;
    
    private Coroutine myProcessingCoroutine = null;


    private void Start()
    {
        progressImage.fillAmount = 0f;
    }

    private void Update()
    {
        if (HasSignal && myProcessingCoroutine == null)
        {
            startButton.isEnabled = true;
        }
        else
        {
            startButton.isEnabled = false;
        }

        if (HasProcessedSignal)
        {
            sendButton.isEnabled = true;
        }
        else
        {
            sendButton.isEnabled = false;
        }
    }

    private void OnEnable()
    {
        startButton.OnActivated += StartProcessing;
        sendButton.OnActivated += SendSignal;
    }

    private void OnDisable()
    {
        startButton.OnActivated -= StartProcessing;
        sendButton.OnActivated -= SendSignal;
    }

    private void StartProcessing()
    {
        if (myProcessingCoroutine == null && HasSignal)
        {
            myProcessingCoroutine = StartCoroutine(Processing());
        }
    }
    
    private IEnumerator Processing()
    {
        float maxTime = Random.Range(minMaxProcessingTime.x, minMaxProcessingTime.y);
        float time = 0;
        
        while (time < maxTime)
        {
            time += Time.deltaTime;
            progressImage.fillAmount = time / maxTime;
            yield return null;
        }

        --unprocessedSignals;
        ++processedSignals;
        myProcessingCoroutine = null;
    }
}