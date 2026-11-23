using System;
using Unity.VisualScripting;
using UnityEngine;

public class MazeGameStation : BaseStation
{
    [SerializeField] private BallInteractible ball = null;
    [SerializeField] private StationButton sendButton = null;

    private void Update()
    {
        if (HasProcessedSignal)
        {
            sendButton.SetEnabled();
        }
        else
        {
            sendButton.SetDisabled();
        }
    }

    private void OnEnable()
    {
        sendButton.OnActivated += SendSignal;
        ball.onWinEnter += ProcessSignal;
    }

    private void OnDisable()
    {
        sendButton.OnActivated -= SendSignal;
        ball.onWinEnter -= ProcessSignal;
    }
}